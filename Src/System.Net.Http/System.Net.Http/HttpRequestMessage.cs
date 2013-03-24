//
// HttpRequestMessage.cs
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

using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace System.Net.Http {
	public class HttpRequestMessage : IDisposable {
		private bool disposed;
		private HttpRequestHeaders headers;
		private bool is_used;
		private HttpMethod method;
		private Dictionary<string, object> properties;
		private Uri uri;
		private Version version;

		public HttpRequestMessage() {
			this.method = HttpMethod.Get;
		}

		public HttpRequestMessage(HttpMethod method, string requestUri)
			: this(method, string.IsNullOrEmpty(requestUri) ? null : new Uri(requestUri, UriKind.RelativeOrAbsolute)) {
		}

		public HttpRequestMessage(HttpMethod method, Uri requestUri) {
			this.Method = method;
			this.RequestUri = requestUri;
		}

		public HttpContent Content { get; set; }

		public HttpRequestHeaders Headers {
			get {
				return this.headers ?? (this.headers = new HttpRequestHeaders());
			}
		}

		public HttpMethod Method {
			get {
				return this.method;
			}
			set {
				if (value == null) {
					throw new ArgumentNullException("method");
				}

				this.method = value;
			}
		}

		public IDictionary<string, object> Properties {
			get {
				return this.properties ?? (this.properties = new Dictionary<string, object>());
			}
		}

		public Uri RequestUri {
			get {
				return this.uri;
			}
			set {
				if (value != null && (value.IsAbsoluteUri && value.Scheme != Uri.UriSchemeHttp && value.Scheme != Uri.UriSchemeHttps)) {
					throw new ArgumentException("Only http or https scheme is allowed");
				}

				this.uri = value;
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

		internal bool SetIsUsed() {
			if (this.is_used) {
				return true;
			}

			this.is_used = true;
			return false;
		}

		public override string ToString() {
			var sb = new StringBuilder();
			sb.Append("Method: ").Append(this.method);
			sb.Append(", RequestUri: '").Append(this.RequestUri != null ? this.RequestUri.ToString() : "<null>");
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
