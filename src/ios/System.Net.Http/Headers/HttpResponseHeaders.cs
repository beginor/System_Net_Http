namespace System.Net.Http.Headers {

	public class HttpResponseHeaders : HttpHeaders {

		public string AcceptRanges {
			get {
				return this.TryGet(HttpKnownHeaderNames.AcceptRanges);
			}
			set {
				this[HttpKnownHeaderNames.AcceptRanges] = value;
			}
		}

		public string Age {
			get {
				return this.TryGet(HttpKnownHeaderNames.Age);
			}
			set {
				this[HttpKnownHeaderNames.Age] = value;
			}
		}

		public string ETag {
			get {
				return this.TryGet(HttpKnownHeaderNames.ETag);
			}
			set {
				this[HttpKnownHeaderNames.ETag] = value;
			}
		}

		public string Location {
			get {
				return this.TryGet(HttpKnownHeaderNames.Location);
			}
			set {
				this[HttpKnownHeaderNames.Location] = value;
			}
		}

		public string ProxyAuthenticate {
			get {
				return this.TryGet(HttpKnownHeaderNames.ProxyAuthenticate);
			}
			set {
				this[HttpKnownHeaderNames.ProxyAuthenticate] = value;
			}
		}

		public string RetryAfter {
			get {
				return this.TryGet(HttpKnownHeaderNames.RetryAfter);
			}
			set {
				this[HttpKnownHeaderNames.RetryAfter] = value;
			}
		}

		public string Server {
			get {
				return this.TryGet(HttpKnownHeaderNames.Server);
			}
			set {
				this[HttpKnownHeaderNames.Server] = value;
			}
		}

		public string Vary {
			get {
				return this.TryGet(HttpKnownHeaderNames.Vary);
			}
			set {
				this[HttpKnownHeaderNames.Vary] = value;
			}
		}

		public string WWWAuthenticate {
			get {
				return this.TryGet(HttpKnownHeaderNames.WWWAuthenticate);
			}
			set {
				this[HttpKnownHeaderNames.WWWAuthenticate] = value;
			}
		}
	}
}