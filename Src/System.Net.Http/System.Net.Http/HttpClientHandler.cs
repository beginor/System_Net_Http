//
// HttpClientHandler.cs
//
// Authors:
//	Marek Safar  <marek.safar@gmail.com>
//
// Copyright (C) 2011 Xamarin Inc (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http {

	public class HttpClientHandler : HttpMessageHandler {

		private bool allowAutoRedirect;
		private DecompressionMethods automaticDecompression;
		private ClientCertificateOption certificate;
		private CookieContainer cookieContainer;
		private ICredentials credentials;
		private int maxAutomaticRedirections;
		private long maxRequestContentBufferSize;
		private bool preAuthenticate;
		private IWebProxy proxy;
		private bool sentRequest;
		private bool useCookies;
		private bool useDefaultCredentials;
		private bool useProxy;

		public HttpClientHandler() {
			this.allowAutoRedirect = true;
			this.maxAutomaticRedirections = 50;
			this.maxRequestContentBufferSize = int.MaxValue;
			this.useCookies = true;
			this.useProxy = true;
		}

		public bool AllowAutoRedirect {
			get {
				return this.allowAutoRedirect;
			}
			set {
				this.EnsureModifiability();
				this.allowAutoRedirect = value;
			}
		}

		public DecompressionMethods AutomaticDecompression {
			get {
				return this.automaticDecompression;
			}
			set {
				this.EnsureModifiability();
				this.automaticDecompression = value;
			}
		}

		public ClientCertificateOption ClientCertificateOptions {
			get {
				return this.certificate;
			}
			set {
				this.EnsureModifiability();
				this.certificate = value;
			}
		}

		public CookieContainer CookieContainer {
			get {
				return this.cookieContainer ?? (this.cookieContainer = new CookieContainer());
			}
			set {
				this.EnsureModifiability();
				this.cookieContainer = value;
			}
		}

		public ICredentials Credentials {
			get {
				return this.credentials;
			}
			set {
				this.EnsureModifiability();
				this.credentials = value;
			}
		}

		public int MaxAutomaticRedirections {
			get {
				return this.maxAutomaticRedirections;
			}
			set {
				this.EnsureModifiability();
				if (value <= 0) {
					throw new ArgumentOutOfRangeException();
				}

				this.maxAutomaticRedirections = value;
			}
		}

		public long MaxRequestContentBufferSize {
			get {
				return this.maxRequestContentBufferSize;
			}
			set {
				this.EnsureModifiability();
				if (value < 0) {
					throw new ArgumentOutOfRangeException();
				}

				this.maxRequestContentBufferSize = value;
			}
		}

		public bool PreAuthenticate {
			get {
				return this.preAuthenticate;
			}
			set {
				this.EnsureModifiability();
				this.preAuthenticate = value;
			}
		}

		public IWebProxy Proxy {
			get {
				return this.proxy;
			}
			set {
				this.EnsureModifiability();
				if (!this.UseProxy) {
					throw new InvalidOperationException();
				}

				this.proxy = value;
			}
		}

		public virtual bool SupportsAutomaticDecompression {
			get {
				return true;
			}
		}

		public virtual bool SupportsProxy {
			get {
				return true;
			}
		}

		public virtual bool SupportsRedirectConfiguration {
			get {
				return true;
			}
		}

		public bool UseCookies {
			get {
				return this.useCookies;
			}
			set {
				this.EnsureModifiability();
				this.useCookies = value;
			}
		}

		public bool UseDefaultCredentials {
			get {
				return this.useDefaultCredentials;
			}
			set {
				this.EnsureModifiability();
				this.useDefaultCredentials = value;
			}
		}

		public bool UseProxy {
			get {
				return this.useProxy;
			}
			set {
				this.EnsureModifiability();
				this.useProxy = value;
			}
		}

		private void EnsureModifiability() {
			if (this.sentRequest) {
				throw new InvalidOperationException(
					"This instance has already started one or more requests. " +
					"Properties can only be modified before sending the first request.");
			}
		}

		protected override void Dispose(bool disposing) {
			// TODO: ?
			base.Dispose(disposing);
		}

		private HttpWebRequest CreateWebRequest(HttpRequestMessage request) {
			var wr = (HttpWebRequest)WebRequest.Create(request.RequestUri)/*new HttpWebRequest (request.RequestUri)*/;
			var requestType = wr.GetType().Name;
			//wr.ThrowOnError = false;

			//wr.ConnectionGroupName = "HttpClientHandler";
			wr.Method = request.Method.Method;
			//wr.ProtocolVersion = request.Version;

			//if (wr.ProtocolVersion == HttpVersion.Version10) {
			//	wr.KeepAlive = request.Headers.ConnectionKeepAlive;
			//} else {
			//	wr.KeepAlive = request.Headers.ConnectionClose != true;
			//}

			//wr.ServicePoint.Expect100Continue = request.Headers.ExpectContinue == true;

			//if (allowAutoRedirect) {
			//	wr.AllowAutoRedirect = true;
			//	wr.MaximumAutomaticRedirections = maxAutomaticRedirections;
			//} else {
			//	wr.AllowAutoRedirect = false;
			//}

			//wr.AutomaticDecompression = automaticDecompression;
			//wr.PreAuthenticate = preAuthenticate;

			if (useCookies && wr.SupportsCookieContainer) {
				wr.CookieContainer = cookieContainer;
			}

			if (requestType.Equals("ClientHttpWebRequest")) {
				if (useDefaultCredentials) {
					wr.UseDefaultCredentials = true;
				}
				else {
					wr.Credentials = credentials;
				}
			}

			//if (useProxy) {
			//	wr.Proxy = proxy;
			//}

			// Add request headers
			wr.Accept = request.Headers.Accept.ToString();
			//if (true || requestType.Equals("ClientHttpWebRequest")) {
				var headers = wr.Headers;
				foreach (var header in request.Headers) {
					if (header.Key == "Accept") {
						continue;
					}
					headers[header.Key] = string.Join(", ", header.Value);
					//foreach (var value in header.Value) {
					//	headers.AddValue(header.Key, value);
					//}
				}
			//}

			return wr;
		}

		private HttpResponseMessage CreateResponseMessage(HttpWebResponse wr, HttpRequestMessage requestMessage) {
			var response = new HttpResponseMessage(wr.StatusCode);
			response.RequestMessage = requestMessage;
			response.ReasonPhrase = wr.StatusDescription;
			response.Content = new StreamContent(wr.GetResponseStream());

			if (wr.SupportsHeaders) {
				WebHeaderCollection headers = wr.Headers;
				for (int i = 0; i < headers.Count; ++i) {
					string key = headers.GetKey(i);
					string value = headers.GetValues(i);

					HttpHeaders item_headers;
					if (HttpHeaders.GetKnownHeaderKind(key) == HttpHeaderKind.Content) {
						item_headers = response.Content.Headers;
					}
					else {
						item_headers = response.Headers;
					}

					item_headers.TryAddWithoutValidation(key, value);
				}
			}
			else {
				//response.Headers.TransferEncoding.;
				response.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(wr.ContentType);
				response.Content.Headers.ContentLength = wr.ContentLength;
			}

			return response;
		}

		protected internal override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			this.sentRequest = true;
			HttpWebRequest wrequest = this.CreateWebRequest(request);
			// if request has content, add content headers first
			if (request.Content != null) {
				wrequest.ContentType = request.Content.Headers.ContentType.ToString();
				//wrequest.ContentLength = request.Content.Headers.ContentLength.GetValueOrDefault();
				if (wrequest.GetType().Name == "ClientHttpWebRequest") {
					var headers = wrequest.Headers;
					foreach (var header in request.Content.Headers) {
						if (header.Key != "Content-Type") {
							headers[header.Key] = string.Join(", ", header.Value);
						}
						//foreach (string value in header.Value) {
						//	headers.AddValue(header.Key, value);
						//}
					}
				}

				var stream = await wrequest.GetRequestStreamAsync();/*wrequest.GetRequestStream ();*/

				await request.Content.CopyToAsync(stream).ConfigureAwait(false);

				stream.Close();
			}

			// FIXME: GetResponseAsync does not accept cancellationToken
			var wresponse = await /*this.DoRequestAsync(wrequest)*/wrequest.GetResponseAsync().ConfigureAwait(false);

			return this.CreateResponseMessage(wresponse as HttpWebResponse, request);
		}

		private Task<HttpWebResponse> DoRequestAsync(HttpWebRequest httpWebRequest) {
			var tcs = new TaskCompletionSource<HttpWebResponse>();
			try {
				httpWebRequest.BeginGetResponse(ar => {
					try {
						if (ar.IsCompleted) {
							var response = (HttpWebResponse)httpWebRequest.EndGetResponse(ar);
							tcs.TrySetResult(response);
						}
					}
					catch (WebException webEx) {
						tcs.TrySetResult((HttpWebResponse)webEx.Response);
					}
					catch (Exception ex) {
						tcs.TrySetException(ex);
					}
				}, null);
			}
			catch (WebException webEx) {
				tcs.TrySetResult((HttpWebResponse)webEx.Response);
			}
			catch (Exception ex) {
				tcs.TrySetException(ex);
			}
			return tcs.Task;
		}
	}
}
