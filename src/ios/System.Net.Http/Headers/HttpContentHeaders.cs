namespace System.Net.Http.Headers {

	public class HttpContentHeaders : HttpHeaders {

		public string Allow {
			get {
				return this.TryGet(HttpKnownHeaderNames.Allow);
			}
			set {
				this[HttpKnownHeaderNames.Allow] = value;
			}
		}

		public string ContentDisposition {
			get {
				return this.TryGet(HttpKnownHeaderNames.ContentDisposition);
			}
			set {
				this[HttpKnownHeaderNames.ContentDisposition] = value;
			}
		}

		public string ContentEncoding {
			get {
				return this.TryGet(HttpKnownHeaderNames.ContentEncoding);
			}
			set {
				this[HttpKnownHeaderNames.ContentEncoding] = value;
			}
		}

		public string ContentLanguage {
			get {
				return this.TryGet(HttpKnownHeaderNames.ContentLanguage);
			}
			set {
				this[HttpKnownHeaderNames.ContentLanguage] = value;
			}
		}

		public string ContentLength {
			get {
				return this.TryGet(HttpKnownHeaderNames.ContentLength);
			}
			set {
				this[HttpKnownHeaderNames.ContentLength] = value;
			}
		}

		public string ContentLocation {
			get {
				return this.TryGet(HttpKnownHeaderNames.ContentLocation);
			}
			set {
				this[HttpKnownHeaderNames.ContentLocation] = value;
			}
		}

		public string ContentMD5 {
			get {
				return this.TryGet(HttpKnownHeaderNames.ContentMD5);
			}
			set {
				this[HttpKnownHeaderNames.ContentMD5] = value;
			}
		}

		public string ContentRange {
			get {
				return this.TryGet(HttpKnownHeaderNames.ContentRange);
			}
			set {
				this[HttpKnownHeaderNames.ContentRange] = value;
			}
		}

		public string ContentType {
			get {
				return this.TryGet(HttpKnownHeaderNames.ContentType);
			}
			set {
				this[HttpKnownHeaderNames.ContentType] = value;
			}
		}

		public string Expires {
			get {
				return this.TryGet(HttpKnownHeaderNames.Expires);
			}
			set {
				this[HttpKnownHeaderNames.Expires] = value;
			}
		}

		public string LastModified {
			get {
				return this.TryGet(HttpKnownHeaderNames.LastModified);
			}
			set {
				this[HttpKnownHeaderNames.LastModified] = value;
			}
		}

	}

}