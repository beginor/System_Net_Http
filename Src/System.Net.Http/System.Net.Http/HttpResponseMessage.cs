//
// HttpResponseMessage.cs
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

using System.Net.Http.Headers;
using System.Text;

namespace System.Net.Http {
	public class HttpResponseMessage : IDisposable {
		private bool disposed;
		private HttpResponseHeaders headers;
		private HttpStatusCode statusCode;
		private Version version;

		public HttpResponseMessage()
			: this(HttpStatusCode.OK) {
		}

		public HttpResponseMessage(HttpStatusCode statusCode) {
			this.StatusCode = statusCode;
		}

		public HttpContent Content { get; set; }

		public HttpResponseHeaders Headers {
			get {
				return this.headers ?? (this.headers = new HttpResponseHeaders());
			}
		}

		public bool IsSuccessStatusCode {
			get {
				// Successful codes are 2xx
				return this.statusCode >= HttpStatusCode.OK && this.statusCode < HttpStatusCode.MultipleChoices;
			}
		}

		public string ReasonPhrase { get; set; }

		public HttpRequestMessage RequestMessage { get; set; }

		public HttpStatusCode StatusCode {
			get {
				return this.statusCode;
			}
			set {
				if (value < 0) {
					throw new ArgumentOutOfRangeException();
				}

				this.statusCode = value;
			}
		}

		public Version Version {
			get {
				return this.version /* ?? HttpVersion.Version11*/;
			}
			set {
				if (value == null) {
					throw new ArgumentNullException("Version");
				}

				this.version = value;
			}
		}

		public void Dispose() {
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing && !this.disposed) {
				this.disposed = true;

				if (this.Content != null) {
					this.Content.Dispose();
				}
			}
		}

		public HttpResponseMessage EnsureSuccessStatusCode() {
			if (this.IsSuccessStatusCode) {
				return this;
			}

			throw new HttpRequestException(string.Format("{0} ({1})", (int)this.statusCode, this.ReasonPhrase));
		}

		public override string ToString() {
			var sb = new StringBuilder();
			sb.Append("StatusCode: ").Append((int)this.StatusCode);
			sb.Append(", ReasonPhrase: '").Append(this.ReasonPhrase ?? "<null>");
			sb.Append("', Version: ").Append(this.Version);
			sb.Append(", Content: ").Append(this.Content != null ? this.Content.ToString() : "<null>");
			sb.Append(", Headers:\r\n{\r\n").Append(this.Headers);
			if (this.Content != null) {
				sb.Append(this.Content.Headers);
			}

			sb.Append("}");

			return sb.ToString();
		}
	}
}
