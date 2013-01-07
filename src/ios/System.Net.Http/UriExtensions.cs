namespace System.Net.Http {

	public static class UriExtensions {
		
		public static bool IsHttp(this Uri uri) {
			var scheme = uri.Scheme;
			return scheme.EqualsIgnoreCase("http") || scheme.EqualsIgnoreCase("https");
		}
	}
}