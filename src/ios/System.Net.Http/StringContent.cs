using System;
using System.Text;

namespace System.Net.Http {

	public class StringContent : ByteArrayContent {

		private const string DefaultContentType = "text/plain";

		public StringContent(string content)
			: this(content, null) {
		}

		public StringContent(string content, Encoding encoding)
			: this(content, encoding, null) {
		}

		public StringContent(string content, Encoding encoding, string contentType)
			: base(GetContentByteArray(content, encoding)) {
			this.Headers.ContentType = contentType ?? DefaultContentType;
		}

		private static byte[] GetContentByteArray(string content, Encoding encoding) {
			if (content == null) {
				throw new ArgumentNullException("content");
			}
			if (encoding == null) {
				encoding = DefaultEncoding;
			}
			return encoding.GetBytes(content);
		}
	}
}