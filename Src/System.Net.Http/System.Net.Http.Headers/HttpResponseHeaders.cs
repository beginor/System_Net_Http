//
// HttpResponseHeaders.cs
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

namespace System.Net.Http.Headers {
	public sealed class HttpResponseHeaders : HttpHeaders {
		internal HttpResponseHeaders()
			: base(HttpHeaderKind.Response) {
		}

		public HttpHeaderValueCollection<string> AcceptRanges {
			get {
				return this.GetValues<string>("Accept-Ranges");
			}
		}

		public TimeSpan? Age {
			get {
				return this.GetValue<TimeSpan?>("Age");
			}
			set {
				this.AddOrRemove("Age", value, l => ((long)((TimeSpan)l).TotalSeconds).ToString());
			}
		}

		public CacheControlHeaderValue CacheControl {
			get {
				return this.GetValue<CacheControlHeaderValue>("Cache-Control");
			}
			set {
				AddOrRemove("Cache-Control", value);
			}
		}

		public HttpHeaderValueCollection<string> Connection {
			get {
				return this.GetValues<string>("Connection");
			}
		}

		public bool? ConnectionClose {
			get {
				if (this.connectionclose == true || this.Connection.Find(l => string.Equals(l, "close", StringComparison.OrdinalIgnoreCase)) != null) {
					return true;
				}

				return this.connectionclose;
			}
			set {
				if (this.connectionclose == value) {
					return;
				}

				this.Connection.Remove("close");
				if (value == true) {
					this.Connection.Add("close");
				}

				this.connectionclose = value;
			}
		}

		public DateTimeOffset? Date {
			get {
				return this.GetValue<DateTimeOffset?>("Date");
			}
			set {
				this.AddOrRemove("Date", value, Parser.DateTime.ToString);
			}
		}

		public EntityTagHeaderValue ETag {
			get {
				return this.GetValue<EntityTagHeaderValue>("ETag");
			}
			set {
				AddOrRemove("ETag", value);
			}
		}

		public Uri Location {
			get {
				return this.GetValue<Uri>("Location");
			}
			set {
				AddOrRemove("Location", value);
			}
		}

		public HttpHeaderValueCollection<NameValueHeaderValue> Pragma {
			get {
				return this.GetValues<NameValueHeaderValue>("Pragma");
			}
		}

		public HttpHeaderValueCollection<AuthenticationHeaderValue> ProxyAuthenticate {
			get {
				return this.GetValues<AuthenticationHeaderValue>("Proxy-Authenticate");
			}
		}

		public RetryConditionHeaderValue RetryAfter {
			get {
				return this.GetValue<RetryConditionHeaderValue>("Retry-After");
			}
			set {
				AddOrRemove("Retry-After", value);
			}
		}

		public HttpHeaderValueCollection<ProductInfoHeaderValue> Server {
			get {
				return this.GetValues<ProductInfoHeaderValue>("Server");
			}
		}

		public HttpHeaderValueCollection<string> Trailer {
			get {
				return this.GetValues<string>("Trailer");
			}
		}

		public HttpHeaderValueCollection<TransferCodingHeaderValue> TransferEncoding {
			get {
				return this.GetValues<TransferCodingHeaderValue>("Transfer-Encoding");
			}
		}

		public bool? TransferEncodingChunked {
			get {
				if (this.transferEncodingChunked.HasValue) {
					return this.transferEncodingChunked;
				}

				TransferCodingHeaderValue found = this.TransferEncoding.Find(l => StringComparer.OrdinalIgnoreCase.Equals(l.Value, "chunked"));
				return found != null ? true : (bool?)null;
			}
			set {
				if (value == this.transferEncodingChunked) {
					return;
				}

				this.TransferEncoding.Remove(l => l.Value == "chunked");
				if (value == true) {
					this.TransferEncoding.Add(new TransferCodingHeaderValue("chunked"));
				}

				this.transferEncodingChunked = value;
			}
		}

		public HttpHeaderValueCollection<ProductHeaderValue> Upgrade {
			get {
				return this.GetValues<ProductHeaderValue>("Upgrade");
			}
		}

		public HttpHeaderValueCollection<string> Vary {
			get {
				return this.GetValues<string>("Vary");
			}
		}

		public HttpHeaderValueCollection<ViaHeaderValue> Via {
			get {
				return this.GetValues<ViaHeaderValue>("Via");
			}
		}

		public HttpHeaderValueCollection<WarningHeaderValue> Warning {
			get {
				return this.GetValues<WarningHeaderValue>("Warning");
			}
		}

		public HttpHeaderValueCollection<AuthenticationHeaderValue> WwwAuthenticate {
			get {
				return this.GetValues<AuthenticationHeaderValue>("WWW-Authenticate");
			}
		}
	}
}
