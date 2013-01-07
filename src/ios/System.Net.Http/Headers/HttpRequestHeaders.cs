namespace System.Net.Http.Headers {

	public class HttpRequestHeaders : HttpHeaders {

		public string Accept {
			get {
				return this.TryGet(HttpKnownHeaderNames.Accept);
			}
			set {
				this[HttpKnownHeaderNames.Accept] = value;
			}
		}

		public string AcceptCharset {
			get {
				return this.TryGet(HttpKnownHeaderNames.AcceptCharset);
			}
			set {
				this[HttpKnownHeaderNames.AcceptCharset] = value;
			}
		}

		public string AcceptEncoding {
			get {
				return this.TryGet(HttpKnownHeaderNames.AcceptEncoding);
			}
			set {
				this[HttpKnownHeaderNames.AcceptEncoding] = value;
			}
		}

		public string AcceptLanguage {
			get {
				return this.TryGet(HttpKnownHeaderNames.AcceptLanguage);
			}
			set {
				this[HttpKnownHeaderNames.AcceptLanguage] = value;
			}
		}

		public string Authorization {
			get {
				return this.TryGet(HttpKnownHeaderNames.Authorization);
			}
			set {
				this[HttpKnownHeaderNames.Authorization] = value;
			}
		}

		public string Expect {
			get {
				return this.TryGet(HttpKnownHeaderNames.Expect);
			}
			set {
				this[HttpKnownHeaderNames.Expect] = value;
			}
		}

		public string From {
			get {
				return this.TryGet(HttpKnownHeaderNames.From);
			}
			set {
				this[HttpKnownHeaderNames.From] = value;
			}
		}

		public string Host {
			get {
				return this.TryGet(HttpKnownHeaderNames.Host);
			}
			set {
				this[HttpKnownHeaderNames.Host] = value;
			}
		}

		public string IfMatch {
			get {
				return this.TryGet(HttpKnownHeaderNames.IfMatch);
			}
			set {
				this[HttpKnownHeaderNames.IfMatch] = value;
			}
		}

		public string IfModifiedSince {
			get {
				return this.TryGet(HttpKnownHeaderNames.IfModifiedSince);
			}
			set {
				this[HttpKnownHeaderNames.IfModifiedSince] = value;
			}
		}

		public string IfNoneMatch {
			get {
				return this.TryGet(HttpKnownHeaderNames.IfNoneMatch);
			}
			set {
				this[HttpKnownHeaderNames.IfNoneMatch] = value;
			}
		}

		public string IfRange {
			get {
				return this.TryGet(HttpKnownHeaderNames.IfRange);
			}
			set {
				this[HttpKnownHeaderNames.IfRange] = value;
			}
		}

		public string IfUnmodifiedSince {
			get {
				return this.TryGet(HttpKnownHeaderNames.IfUnmodifiedSince);
			}
			set {
				this[HttpKnownHeaderNames.IfUnmodifiedSince] = value;
			}
		}

		public string MaxForwards {
			get {
				return this.TryGet(HttpKnownHeaderNames.MaxForwards);
			}
			set {
				this[HttpKnownHeaderNames.MaxForwards] = value;
			}
		}

		public string ProxyAuthorization {
			get {
				return this.TryGet(HttpKnownHeaderNames.ProxyAuthorization);
			}
			set {
				this[HttpKnownHeaderNames.ProxyAuthorization] = value;
			}
		}

		public string Range {
			get {
				return this.TryGet(HttpKnownHeaderNames.Range);
			}
			set {
				this[HttpKnownHeaderNames.Range] = value;
			}
		}

		public string Referer {
			get {
				return this.TryGet(HttpKnownHeaderNames.Referer);
			}
			set {
				this[HttpKnownHeaderNames.Referer] = value;
			}
		}

		public string TE {
			get {
				return this.TryGet(HttpKnownHeaderNames.TE);
			}
			set {
				this[HttpKnownHeaderNames.TE] = value;
			}
		}
		
		public string UserAgent {
			get {
				return this.TryGet(HttpKnownHeaderNames.UserAgent);
			}
			set {
				this[HttpKnownHeaderNames.UserAgent] = value;
			}
		}

		public string Date {
			get {
				return this.TryGet(HttpKnownHeaderNames.Date);
			}
			set {
				this[HttpKnownHeaderNames.Date] = value;
			}
		}

		public string TransferEncoding {
			get {
				return this.TryGet(HttpKnownHeaderNames.TransferEncoding);
			}
			set {
				this[HttpKnownHeaderNames.TransferEncoding] = value;
			}
		}

		public string Connection {
			get {
				return this.TryGet(HttpKnownHeaderNames.Connection);
			}
			set {
				this[HttpKnownHeaderNames.Connection] = value;
			}
		}
	}

}