using System;
using System.Net;
using System.Net.Http.Headers;

namespace System.Net.Http {

	public class HttpResponseMessage : Disposable {

		public HttpContent Content {
			get;
			set;
		}

		public HttpRequestMessage RequestMessage {
			get;
			set;
		}

		public HttpResponseHeaders Headers {
			get;
			private set;
		}

		public Version Version {
			get;
			set;
		}

		public string ReasonPhrase {
			get;
			set;
		}

		public HttpStatusCode StatusCode {
			get;
			private set;
		}

		public bool IsSuccessStatusCode {
			get {
				return this.StatusCode >= HttpStatusCode.OK && this.StatusCode <= (HttpStatusCode)299;
			}
		}

		public HttpResponseMessage() : this(HttpStatusCode.OK) {
		}

		public HttpResponseMessage(HttpStatusCode statusCode) {
			this.StatusCode = statusCode;
			this.Headers = new HttpResponseHeaders();
		}

		public void EnsureIsSuccessStatusCode() {
			if (!this.IsSuccessStatusCode) {
				var message = string.Format("Response code is {0} , description is {1} .", this.StatusCode, this.ReasonPhrase);
				throw new InvalidOperationException(message);
			}
		}
	}

}