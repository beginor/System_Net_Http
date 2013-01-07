using System;
using System.Collections.Generic;
using System.Text;

namespace System.Net.Http {

	public class FormUrlEncodedContent : ByteArrayContent {

		public FormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> nameValues) : base(GetContentByteArray(nameValues)) {
			this.Headers.ContentType = HttpMediaTypeNames.ApplicationFormUrlEncoded;
		}

		private static byte[] GetContentByteArray(IEnumerable<KeyValuePair<string, string>> nameValues) {
			if (nameValues == null) {
				throw new ArgumentNullException("nameValues");
			}
			var stringBuilder = new StringBuilder();
			foreach (var current in nameValues) {
				if (stringBuilder.Length > 0) {
					stringBuilder.Append('&');
				}
				stringBuilder.Append(Encode(current.Key));
				stringBuilder.Append('=');
				stringBuilder.Append(Encode(current.Value));
			}
			return DefaultEncoding.GetBytes(stringBuilder.ToString());
		}

		private static string Encode(string data) {
			return string.IsNullOrEmpty(data) ? string.Empty : Uri.EscapeDataString(data).Replace("%20", "+");
		}
	}
}