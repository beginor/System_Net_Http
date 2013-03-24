using System.Linq;

namespace System.Net {
	public static class WebHeaderCollectionExtensions {
		public static void AddValue(this WebHeaderCollection headers, string key, string value) {
			if (!headers.AllKeys.Contains(key)) {
				headers[key] = value;
			}
			else {
				headers[key] = headers[key] + ";" + value;
			}
		}

		public static string GetKey(this WebHeaderCollection headers, int index) {
			return headers.AllKeys[index];
		}

		public static string GetValues(this WebHeaderCollection headers, int index) {
			return headers[headers.GetKey(index)];
		}
	}
}
