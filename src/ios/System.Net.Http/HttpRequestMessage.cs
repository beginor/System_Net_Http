using System;
using System.Threading;
using System.Net.Http.Headers;

namespace System.Net.Http {

	public class HttpRequestMessage : Disposable {

		private const int MessageAlreadySent = 1;
		private const int MessageNotYetSent = 0;
		private int _sendStatus;

		public HttpContent Content {
			get;
			set;
		}

		public HttpRequestHeaders Headers {
			get;
			private set;
		}

		public string Method {
			get;
			set;
		}

		public Uri RequestUri {
			get;
			set;
		}

		public Version Version {
			get;
			set;
		}

		public HttpRequestMessage()
			: this(HttpMethods.Get, (Uri)null) {
		}

		public HttpRequestMessage(string method, string requestUri) {
			this.InitializeValues(method, requestUri.IsNullOrEmpty() ? null : new Uri(requestUri, UriKind.RelativeOrAbsolute));
		}

		public HttpRequestMessage(string method, Uri requestUri) {
			this.InitializeValues(method, requestUri);
		}

		private void InitializeValues(string method, Uri requestUri) {
			if (method.IsNullOrEmpty()) {
				throw new ArgumentNullException("method");
			}
			if (requestUri != null && requestUri.IsAbsoluteUri && !requestUri.IsHttp()) {
				throw new ArgumentException("SR.net_http_client_http_baseaddress_required", "requestUri");
			}
			this.Headers = new HttpRequestHeaders();
			this.Method = method;
			this.RequestUri = requestUri;
			this.Version = new Version(1, 1);
		}

		internal bool MarkAsSent() {
			return Interlocked.Exchange(ref this._sendStatus, MessageAlreadySent) == MessageNotYetSent;
		}

	}
}