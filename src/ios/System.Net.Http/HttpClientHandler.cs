using System;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http {

	public class HttpClientHandler : HttpMessageHandler {

		private static readonly Action<object> _onCancel = OnCalcel;

		private readonly Action<object> _startRequest;
		private readonly AsyncCallback _getRequestStreamCallback;
		private readonly AsyncCallback _getResponseCallback;
		private readonly string _connectionGroupName;
		private Uri _lastRequestUri;

		public Func<HttpRequestMessage, string, HttpWebRequest> WebRequestCreator;
		private bool _operationStarted;

		public bool AllowAutoRedirect {
			get;
			set;
		}

#if !SILVERLIGHT
		public DecompressionMethods AutomaticDecompression {
			get;
			set;
		}
#endif

#if !SILVERLIGHT
		public System.Net.Cache.RequestCachePolicy CachePolicy {
			get;
			set;
		}
#endif
		public CookieContainer CookieContainer {
			get;
			set;
		}

		public ICredentials Credentials {
			get;
			set;
		}

		public int MaxAutomaticRedirections {
			get;
			set;
		}

		public long MaxRequestContentBufferSize {
			get;
			set;
		}

		public bool PreAuthenticate {
			get;
			set;
		}

#if !SILVERLIGHT
		public IWebProxy Proxy {
			get;
			set;
		} 
#endif

		public virtual bool SupportsAutomaticDecompression {
			get {
#if SILVERLIGHT
				return false;
#else
				return true;
#endif
			}
		}

		public virtual bool SupportsProxy {
			get {
				return true;
			}
		}

		public virtual bool SupportsRedirectConfiguration {
			get {
#if SILVERLIGHT
				return false;
#else
				return true;
#endif
			}
		}

		public bool UseCookies {
			get;
			set;
		}

		public bool UseDefaultCredentials {
			get;
			set;
		}

		public bool UseProxy {
			get;
			set;
		}

		public HttpClientHandler() {
			this._startRequest = this.StartRequest;
			this._getRequestStreamCallback = this.GetRequestStreamCallback;
			this._getResponseCallback = this.GetResponseCallback;
			this._connectionGroupName = this.GetType().ToString();

			this.AllowAutoRedirect = true;
			this.MaxRequestContentBufferSize = long.MaxValue;
#if !SILVERLIGHT
			this.AutomaticDecompression = DecompressionMethods.None;
			this.Proxy = null;
			this.PreAuthenticate = false;
#endif
			this.CookieContainer = new CookieContainer();
			this.Credentials = null;
			this.MaxAutomaticRedirections = 50;

			this.UseProxy = true;
			this.UseCookies = true;
			this.UseDefaultCredentials = false;
		}

		private void GetResponseCallback(IAsyncResult ar) {
			var state = (HttpRequestState)ar.AsyncState;
			try {
				var response = state.WebRequest.EndGetResponse(ar) as HttpWebResponse;
				var responseMessage = this.CreateResponseMessage(response, state.RequestMessage);
				state.Tcs.TrySetResult(responseMessage);
			}
			catch (Exception ex) {
				this.HandleAsyncException(state, ex);
			}
		}

		private HttpResponseMessage CreateResponseMessage(HttpWebResponse webResponse, HttpRequestMessage request) {
			var httpResponseMessage = new HttpResponseMessage(webResponse.StatusCode) {
				ReasonPhrase = webResponse.StatusDescription,
#if !SILVERLIGHT
				Version = webResponse.ProtocolVersion, 
#endif
				RequestMessage = request,
				Content = new StreamContent(webResponse.GetResponseStream())
			};
			request.RequestUri = webResponse.ResponseUri;

			var contentHeaders = httpResponseMessage.Content.Headers;
#if SILVERLIGHT
			if (webResponse.SupportsHeaders) {
#endif
				var responseHeaders = httpResponseMessage.Headers;
				var headers = webResponse.Headers;
				for (var i = 0; i < headers.Count; i++) {
					var key = headers.AllKeys[i];
					responseHeaders[key] = headers[key];
					contentHeaders[key] = headers[key];
				}
#if SILVERLIGHT
			}
			else {
				contentHeaders.ContentLength = webResponse.ContentLength.ToString(CultureInfo.InvariantCulture);
				contentHeaders.ContentType = webResponse.ContentType;
			}
#endif
			return httpResponseMessage;
		}

		private void GetRequestStreamCallback(IAsyncResult ar) {
			var state = (HttpRequestState)ar.AsyncState;
			try {
				var stream = state.WebRequest.EndGetRequestStream(ar);
				state.RequestStream = stream;
				state.RequestMessage.Content.CopyToAsync(stream).ContinueWith(t => {
					try {
						if (t.IsFaulted) {
							state.Tcs.TrySetException(t.Exception.GetBaseException());
						}
						else {
							if (t.IsCanceled) {
								state.Tcs.TrySetCanceled();
							}
							else {
								state.RequestStream.Close();
								this.StartGettingResponse(state);
							}
						}
					}
					catch (Exception ex) {
						state.Tcs.TrySetException(ex);
					}
				});
			}
			catch (Exception ex) {
				this.HandleAsyncException(state, ex);
			}
		}

		private void StartRequest(object obj) {
			var state = (HttpRequestState)obj;
			try {
				if (state.RequestMessage.Content != null) {
					this.PrepareAndStartContentUpload(state);
				}
				else {
					state.WebRequest.ContentLength = 0L;
					this.StartGettingResponse(state);
				}
			}
			catch (Exception ex) {
				this.HandleAsyncException(state, ex);
			}
		}

		private void StartGettingResponse(HttpRequestState state) {
			state.WebRequest.BeginGetResponse(this._getResponseCallback, state);
		}

		private void PrepareAndStartContentUpload(HttpRequestState state) {
			this.StartGettingRequestStream(state);
		}

		private void StartGettingRequestStream(HttpRequestState state) {
			state.WebRequest.BeginGetRequestStream(this._getRequestStreamCallback, state);
		}

		private static void OnCalcel(object state) {
			var httpWebRequest = state as HttpWebRequest;
			if (httpWebRequest != null) {
				httpWebRequest.Abort();
			}
		}

		protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			if (request == null) {
				throw new ArgumentNullException("request");
			}
			this.CheckDisposed();
			this.SetOperationStarted();
			var tcs = new TaskCompletionSource<HttpResponseMessage>();

			var requestState = new HttpRequestState {
				Tcs = tcs,
				CancellationToken = cancellationToken,
				RequestMessage = request
			};

			this._lastRequestUri = request.RequestUri;

			try {
				var httpWebRequest = this.CreateAndPrepareWebRequest(request);
				requestState.WebRequest = httpWebRequest;
				cancellationToken.Register(_onCancel, httpWebRequest);
				Task.Factory.StartNew(this._startRequest, requestState);
			}
			catch (Exception ex) {
				this.HandleAsyncException(requestState, ex);
			}

			return tcs.Task;
		}

		private HttpWebRequest CreateAndPrepareWebRequest(HttpRequestMessage request) {
			HttpWebRequest httpWebRequest;
			if (this.WebRequestCreator != null) {
				httpWebRequest = this.WebRequestCreator(request, this._connectionGroupName);
				if (httpWebRequest == null) {
					throw new InvalidOperationException("WebRequestCreator return null request!");
				}
			}
			else {
				httpWebRequest = (HttpWebRequest)WebRequest.Create(request.RequestUri);
#if !SILVERLIGHT
				httpWebRequest.ConnectionGroupName = this._connectionGroupName; 
#endif
			}
			httpWebRequest.Method = request.Method;
#if !SILVERLIGHT
			httpWebRequest.ProtocolVersion = request.Version; 
#endif
			this.SetDefaultOptions(httpWebRequest);
			SetConnectionOptions(httpWebRequest, request);
			this.SetServicePointOptions(httpWebRequest, request);
			SetRequestHeaders(httpWebRequest, request);
			SetContentHeaders(httpWebRequest, request);
			this.InitializeWebRequest(request, httpWebRequest);
			return httpWebRequest;
		}

		protected virtual void InitializeWebRequest(HttpRequestMessage request, HttpWebRequest webRequest) {
		}

		private static void SetContentHeaders(HttpWebRequest webRequest, HttpRequestMessage request) {
			if (request.Content == null) {
				return;
			}
			foreach (var header in request.Content.Headers) {
				if (AreEqual(header.Key, HttpKnownHeaderNames.ContentType)) {
					webRequest.ContentType = header.Value;
				}
				else {
					webRequest.Headers[header.Key] = header.Value;
				}
			}
		}

		private static void SetRequestHeaders(HttpWebRequest webRequest, HttpRequestMessage request) {
			var headers = webRequest.Headers;
			var headers2 = request.Headers;
			var flag = headers2.ContainsKey(HttpKnownHeaderNames.Host);
			var flag2 = headers2.ContainsKey(HttpKnownHeaderNames.Expect);
			var flag3 = headers2.ContainsKey(HttpKnownHeaderNames.TransferEncoding);
			var flag4 = headers2.ContainsKey(HttpKnownHeaderNames.Connection);
			var flag5 = headers2.ContainsKey(HttpKnownHeaderNames.Accept);
			var flag6 = headers2.ContainsKey(HttpKnownHeaderNames.Range);
			var flag7 = headers2.ContainsKey(HttpKnownHeaderNames.Referer);
			var flag8 = headers2.ContainsKey(HttpKnownHeaderNames.UserAgent);
			var flag9 = headers2.ContainsKey(HttpKnownHeaderNames.Date);
			var flag10 = headers2.ContainsKey(HttpKnownHeaderNames.IfModifiedSince);

			if (flag9) {
				webRequest.Headers[HttpRequestHeader.Date] = headers2.Date;
			}

#if !SILVERLIGHT
			
			if (flag2) {
				webRequest.Expect = headers2.Expect;
			}
			if (flag3) {
				webRequest.SendChunked = true;
				webRequest.TransferEncoding = headers2.TransferEncoding;
				webRequest.SendChunked = false;
			}
			if (flag4) {
				webRequest.Connection = headers2.Connection;
			}
			if (flag10) {
				DateTimeOffset date;
				if (DateTimeOffset.TryParse(headers2.IfModifiedSince, out date)) {
					webRequest.IfModifiedSince = date.UtcDateTime;
				}
			}
			if (flag6) {
				int range;
				if (int.TryParse(headers2.Range, out range)) {
					webRequest.AddRange(range);
				}
			}
			if (flag7) {
				webRequest.Referer = headers2.Referer;
			}
#endif
			if (flag5) {
				webRequest.Accept = headers2.Accept;
			}
			if (flag8) {
				webRequest.UserAgent = headers2.UserAgent;
			}
			if (flag) {
				webRequest.Headers[HttpRequestHeader.Host] = headers2.Host;
			}

			foreach (var current2 in headers2) {
				var key = current2.Key;
				if ((!flag || !AreEqual(HttpKnownHeaderNames.Host, key))
					&& (!flag2 || !AreEqual(HttpKnownHeaderNames.Expect, key))
					&& (!flag3 || !AreEqual(HttpKnownHeaderNames.TransferEncoding, key))
					&& (!flag5 || !AreEqual(HttpKnownHeaderNames.Accept, key))
					&& (!flag6 || !AreEqual(HttpKnownHeaderNames.Range, key))
					&& (!flag7 || !AreEqual(HttpKnownHeaderNames.Referer, key))
					&& (!flag8 || !AreEqual(HttpKnownHeaderNames.UserAgent, key))
					&& (!flag9 || !AreEqual(HttpKnownHeaderNames.Date, key))
					&& (!flag10 || !AreEqual(HttpKnownHeaderNames.IfModifiedSince, key))
					&& (!flag4 || !AreEqual(HttpKnownHeaderNames.Connection, key))) {
					headers[current2.Key] = current2.Value;
				}
			}

		}

		private static bool AreEqual(string x, string y) {
			return string.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0;
		}

		private void SetServicePointOptions(HttpWebRequest webRequest, HttpRequestMessage request) {
			//throw new NotImplementedException();
		}

		private static void SetConnectionOptions(HttpWebRequest httpWebRequest, HttpRequestMessage request) {
#if !SILVERLIGHT
			if (request.Version <= HttpVersion.Version10) {
				if (request.Headers.ContainsKey(HttpKnownHeaderNames.Connection) && request.Headers[HttpKnownHeaderNames.Connection].Contains("Keep-Alive")) {
					httpWebRequest.KeepAlive = true;
				}
			}
			else {
				if (request.Headers.ContainsKey(HttpKnownHeaderNames.Connection) && request.Headers[HttpKnownHeaderNames.Connection].Contains("Close")) {
					httpWebRequest.KeepAlive = false;
				}
			} 
#endif
		}

		private void SetDefaultOptions(HttpWebRequest webRequest) {
#if !SILVERLIGHT
			webRequest.Timeout = -1;
			webRequest.AutomaticDecompression = this.AutomaticDecompression;
			webRequest.PreAuthenticate = this.PreAuthenticate;
			webRequest.AllowAutoRedirect = this.AllowAutoRedirect;
			webRequest.CachePolicy = this.CachePolicy;
#endif
			if (this.UseDefaultCredentials) {
				webRequest.UseDefaultCredentials = true;
			}
			else {
#if SILVERLIGHT
				if (webRequest.CreatorInstance == System.Net.Browser.WebRequestCreator.ClientHttp) {
#endif
					webRequest.Credentials = this.Credentials;
#if SILVERLIGHT
				}
#endif
			}
#if !SILVERLIGHT
			if (this.SupportsRedirectConfiguration && this.AllowAutoRedirect) {
				webRequest.MaximumAutomaticRedirections = this.MaxAutomaticRedirections;
			}
#endif
#if !SILVERLIGHT
			if (this.UseProxy) {
				if (this.Proxy != null) {
					webRequest.Proxy = this.Proxy;
				}
			}
			else {
				webRequest.Proxy = null;
			}
#endif
			if (this.UseCookies) {
#if SILVERLIGHT
				if (webRequest.SupportsCookieContainer) {
#endif
					webRequest.CookieContainer = this.CookieContainer;
#if SILVERLIGHT
				}
#endif
			}
		}

		private void HandleAsyncException(HttpRequestState state, Exception e) {
			HttpResponseMessage result;
			if (this.TryGetExceptionResponse(e as WebException, state.RequestMessage, out result)) {
				state.Tcs.TrySetResult(result);
				return;
			}
			if (state.CancellationToken.IsCancellationRequested) {
				state.Tcs.TrySetCanceled();
				return;
			}
			state.Tcs.TrySetException(e);
		}

		private bool TryGetExceptionResponse(WebException webException, HttpRequestMessage requestMessage, out HttpResponseMessage httpResponseMessage) {
			if (webException != null && webException.Response != null) {
				var httpWebResponse = webException.Response as HttpWebResponse;
				if (httpWebResponse != null) {
					httpResponseMessage = this.CreateResponseMessage(httpWebResponse, requestMessage);
					return true;
				}
			}
			httpResponseMessage = null;
			return false;
		}

		private void SetOperationStarted() {
			if (!this._operationStarted) {
				this._operationStarted = true;
			}
		}

		protected override void Dispose(bool disposing) {
			if (!this.Disposed && disposing) {
				if (this._lastRequestUri != null) {
#if !SILVERLIGHT
					var servicePoint = ServicePointManager.FindServicePoint(this._lastRequestUri);
					if (servicePoint != null) {
						servicePoint.CloseConnectionGroup(this._connectionGroupName);
					}
#endif
				}
			}
			base.Dispose(disposing);
		}

	}
}