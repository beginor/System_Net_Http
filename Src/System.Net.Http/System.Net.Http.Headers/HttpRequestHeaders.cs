//
// HttpRequestHeaders.cs
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
	public sealed class HttpRequestHeaders : HttpHeaders {
		private bool? expectContinue;

		internal HttpRequestHeaders()
			: base(HttpHeaderKind.Request) {
		}

		public HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> Accept {
			get {
				return this.GetValues<MediaTypeWithQualityHeaderValue>("Accept");
			}
		}

		public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptCharset {
			get {
				return this.GetValues<StringWithQualityHeaderValue>("Accept-Charset");
			}
		}

		public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptEncoding {
			get {
				return this.GetValues<StringWithQualityHeaderValue>("Accept-Encoding");
			}
		}

		public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptLanguage {
			get {
				return this.GetValues<StringWithQualityHeaderValue>("Accept-Language");
			}
		}

		public AuthenticationHeaderValue Authorization {
			get {
				return this.GetValue<AuthenticationHeaderValue>("Authorization");
			}
			set {
				AddOrRemove("Authorization", value);
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

		internal bool ConnectionKeepAlive {
			get {
				return this.Connection.Find(l => string.Equals(l, "Keep-Alive", StringComparison.OrdinalIgnoreCase)) != null;
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

		public HttpHeaderValueCollection<NameValueWithParametersHeaderValue> Expect {
			get {
				return this.GetValues<NameValueWithParametersHeaderValue>("Expect");
			}
		}

		public bool? ExpectContinue {
			get {
				if (this.expectContinue.HasValue) {
					return this.expectContinue;
				}

				TransferCodingHeaderValue found = this.TransferEncoding.Find(l => string.Equals(l.Value, "100-continue", StringComparison.OrdinalIgnoreCase));
				return found != null ? true : (bool?)null;
			}
			set {
				if (this.expectContinue == value) {
					return;
				}

				this.Expect.Remove(l => l.Name == "100-continue");

				if (value == true) {
					this.Expect.Add(new NameValueWithParametersHeaderValue("100-continue"));
				}

				this.expectContinue = value;
			}
		}

		public string From {
			get {
				return this.GetValue<string>("From");
			}
			set {
#if SILVERLIGHT
				if (!string.IsNullOrEmpty(value)) {
					throw new FormatException();
				}
#else
				if (!string.IsNullOrEmpty (value) && !Parser.EmailAddress.TryParse (value, out value))
					throw new FormatException ();
#endif

				this.AddOrRemove("From", value);
			}
		}

		public string Host {
			get {
				return this.GetValue<string>("Host");
			}
			set {
				this.AddOrRemove("Host", value);
			}
		}

		public HttpHeaderValueCollection<EntityTagHeaderValue> IfMatch {
			get {
				return this.GetValues<EntityTagHeaderValue>("If-Match");
			}
		}

		public DateTimeOffset? IfModifiedSince {
			get {
				return this.GetValue<DateTimeOffset?>("If-Modified-Since");
			}
			set {
				this.AddOrRemove("If-Modified-Since", value, Parser.DateTime.ToString);
			}
		}

		public HttpHeaderValueCollection<EntityTagHeaderValue> IfNoneMatch {
			get {
				return this.GetValues<EntityTagHeaderValue>("If-None-Match");
			}
		}

		public RangeConditionHeaderValue IfRange {
			get {
				return this.GetValue<RangeConditionHeaderValue>("If-Range");
			}
			set {
				AddOrRemove("If-Range", value);
			}
		}

		public DateTimeOffset? IfUnmodifiedSince {
			get {
				return this.GetValue<DateTimeOffset?>("If-Unmodified-Since");
			}
			set {
				this.AddOrRemove("If-Unmodified-Since", value, Parser.DateTime.ToString);
			}
		}

		public int? MaxForwards {
			get {
				return this.GetValue<int?>("Max-Forwards");
			}
			set {
				AddOrRemove("Max-Forwards", value);
			}
		}

		public HttpHeaderValueCollection<NameValueHeaderValue> Pragma {
			get {
				return this.GetValues<NameValueHeaderValue>("Pragma");
			}
		}

		public AuthenticationHeaderValue ProxyAuthorization {
			get {
				return this.GetValue<AuthenticationHeaderValue>("Proxy-Authorization");
			}
			set {
				AddOrRemove("Proxy-Authorization", value);
			}
		}

		public RangeHeaderValue Range {
			get {
				return this.GetValue<RangeHeaderValue>("Range");
			}
			set {
				AddOrRemove("Range", value);
			}
		}

		public Uri Referrer {
			get {
				return this.GetValue<Uri>("Referer");
			}
			set {
				AddOrRemove("Referer", value);
			}
		}

		public HttpHeaderValueCollection<TransferCodingWithQualityHeaderValue> TE {
			get {
				return this.GetValues<TransferCodingWithQualityHeaderValue>("TE");
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

				TransferCodingHeaderValue found = this.TransferEncoding.Find(l => string.Equals(l.Value, "chunked", StringComparison.OrdinalIgnoreCase));
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

		public HttpHeaderValueCollection<ProductInfoHeaderValue> UserAgent {
			get {
				return this.GetValues<ProductInfoHeaderValue>("User-Agent");
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

		internal void AddHeaders(HttpRequestHeaders headers) {
			foreach (var header in headers) {
				TryAddWithoutValidation(header.Key, header.Value);
			}
		}
	}
}
