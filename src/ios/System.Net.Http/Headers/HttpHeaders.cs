using System.Collections.Generic;

namespace System.Net.Http.Headers {

	public class HttpHeaders : Dictionary<string, string> {

		protected string TryGet(string key) {
			return this.ContainsKey(key) ? this[key] : string.Empty;
		}

	}

}