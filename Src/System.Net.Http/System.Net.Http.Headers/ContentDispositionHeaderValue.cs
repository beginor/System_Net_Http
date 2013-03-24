//
// ContentDispositionHeaderValue.cs
//
// Authors:
//	Marek Safar  <marek.safar@gmail.com>
//
// Copyright (C) 2012 Xamarin Inc (http://www.xamarin.com)
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

using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace System.Net.Http.Headers {
	public class ContentDispositionHeaderValue : ICloneable {
		private string dispositionType;
		private List<NameValueHeaderValue> parameters;

		private ContentDispositionHeaderValue() {
		}

		public ContentDispositionHeaderValue(string dispositionType) {
			this.DispositionType = dispositionType;
		}

		protected ContentDispositionHeaderValue(ContentDispositionHeaderValue source) {
			if (source == null) {
				throw new ArgumentNullException("source");
			}

			this.dispositionType = source.dispositionType;
			if (source.parameters != null) {
				foreach (NameValueHeaderValue item in source.parameters) {
					this.Parameters.Add(new NameValueHeaderValue(item));
				}
			}
		}

		public DateTimeOffset? CreationDate {
			get {
				return this.GetDateValue("creation-date");
			}
			set {
				this.SetDateValue("creation-date", value);
			}
		}

		public string DispositionType {
			get {
				return this.dispositionType;
			}
			set {
				Parser.Token.Check(value);
				this.dispositionType = value;
			}
		}

		public string FileName {
			get {
				string value = this.FindParameter("filename");
				if (value == null) {
					return null;
				}

				return DecodeValue(value, false);
			}
			set {
				if (value != null) {
					value = EncodeBase64Value(value);
				}

				this.SetValue("filename", value);
			}
		}

		public string FileNameStar {
			get {
				string value = this.FindParameter("filename*");
				if (value == null) {
					return null;
				}

				return DecodeValue(value, true);
			}
			set {
				if (value != null) {
					value = EncodeRFC5987(value);
				}

				this.SetValue("filename*", value);
			}
		}

		public DateTimeOffset? ModificationDate {
			get {
				return this.GetDateValue("modification-date");
			}
			set {
				this.SetDateValue("modification-date", value);
			}
		}

		public string Name {
			get {
				return this.FindParameter("name");
			}
			set {
				this.SetValue("name", value);
			}
		}

		public ICollection<NameValueHeaderValue> Parameters {
			get {
				return this.parameters ?? (this.parameters = new List<NameValueHeaderValue>());
			}
		}

		public DateTimeOffset? ReadDate {
			get {
				return this.GetDateValue("read-date");
			}
			set {
				this.SetDateValue("read-date", value);
			}
		}

		public long? Size {
			get {
				string found = this.FindParameter("size");
				long result;
				if (Parser.Long.TryParse(found, out result)) {
					return result;
				}

				return null;
			}
			set {
				if (value == null) {
					this.SetValue("size", null);
					return;
				}

				if (value < 0) {
					throw new ArgumentOutOfRangeException("value");
				}

				this.SetValue("size", value.Value.ToString(CultureInfo.InvariantCulture));
			}
		}

		object ICloneable.Clone() {
			return new ContentDispositionHeaderValue(this);
		}

		public override bool Equals(object obj) {
			var source = obj as ContentDispositionHeaderValue;
			return source != null &&
			       string.Equals(source.dispositionType, this.dispositionType, StringComparison.OrdinalIgnoreCase) &&
			       source.parameters.SequenceEqual(this.parameters);
		}

		private string FindParameter(string name) {
			if (this.parameters == null) {
				return null;
			}

			foreach (NameValueHeaderValue entry in this.parameters) {
				if (string.Equals(entry.Name, name, StringComparison.OrdinalIgnoreCase)) {
					return entry.Value;
				}
			}

			return null;
		}

		private DateTimeOffset? GetDateValue(string name) {
			string value = this.FindParameter(name);
			if (value == null || value == null) {
				return null;
			}

			if (value.Length < 3) {
				return null;
			}

			if (value[0] == '\"') {
				value = value.Substring(1, value.Length - 2);
			}

			DateTimeOffset offset;
			if (Lexer.TryGetDateValue(value, out offset)) {
				return offset;
			}

			return null;
		}

		private static string EncodeBase64Value(string value) {
			for (int i = 0; i < value.Length; ++i) {
				char ch = value[i];
				if (ch > 127) {
					Encoding encoding = Encoding.UTF8;
					return string.Format("\"=?{0}?B?{1}?=\"",
					                     encoding.WebName, Convert.ToBase64String(encoding.GetBytes(value)));
				}
			}

			if (!Lexer.IsValidToken(value)) {
				return "\"" + value + "\"";
			}

			return value;
		}

		private static string EncodeRFC5987(string value) {
			Encoding encoding = Encoding.UTF8;
			var sb = new StringBuilder(value.Length + 11);
			sb.Append(encoding.WebName);
			sb.Append('\'');
			sb.Append('\'');

			for (int i = 0; i < value.Length; ++i) {
				char ch = value[i];
				if (ch > 127) {
					foreach (byte b in encoding.GetBytes(new[] {ch})) {
						sb.Append('%');
						sb.Append(b.ToString("X2"));
					}

					continue;
				}

				sb.Append(ch);
			}

			return sb.ToString();
		}

		private static string DecodeValue(string value, bool extendedNotation) {
			//
			// A short (length <= 78 characters)
			// parameter value containing only non-`tspecials' characters SHOULD be
			// represented as a single `token'.  A short parameter value containing
			// only ASCII characters, but including `tspecials' characters, SHOULD
			// be represented as `quoted-string'.  Parameter values longer than 78
			// characters, or which contain non-ASCII characters, MUST be encoded as
			// specified in [RFC 2184].
			//
			if (value.Length < 2) {
				return value;
			}

			string[] sep;
			Encoding encoding;

			// Quoted string
			if (value[0] == '\"') {
				//
				// Is Base64 encoded ?
				// encoded-word := "=?" charset "?" encoding "?" encoded-text "?="
				//
				sep = value.Split('?');
				if (sep.Length != 5 || sep[0] != "\"=" || sep[4] != "=\"" || (sep[2] != "B" && sep[2] != "b")) {
					return value;
				}

				try {
					encoding = Encoding.GetEncoding(sep[1]);
					return encoding.GetString(Convert.FromBase64String(sep[3]), 0, sep[3].Length);
				}
				catch {
					return value;
				}
			}

			if (!extendedNotation) {
				return value;
			}

			//
			// RFC 5987: Charset/Language Encoding
			//
			sep = value.Split('\'');
			if (sep.Length != 3) {
				return null;
			}

			try {
				encoding = Encoding.GetEncoding(sep[0]);
			}
			catch {
				return null;
			}

			// TODO: What to do with sep[1] language

			value = sep[2];

			int pct_encoded = value.IndexOf('%');
			if (pct_encoded < 0) {
				return value;
			}

			var sb = new StringBuilder();
			byte[] buffer = null;
			int buffer_pos = 0;

			for (int i = 0; i < value.Length;) {
				char ch = value[i];
				if (ch == '%') {
					char unescaped = ch;
					ch = UriHelper.HexUnescape(value, ref i);
					if (ch != unescaped) {
						if (buffer == null) {
							buffer = new byte[value.Length - i + 1];
						}

						buffer[buffer_pos++] = (byte)ch;
						continue;
					}
				}
				else {
					++i;
				}

				if (buffer_pos != 0) {
					sb.Append(encoding.GetChars(buffer, 0, buffer_pos));
					buffer_pos = 0;
				}

				sb.Append(ch);
			}

			if (buffer_pos != 0) {
				sb.Append(encoding.GetChars(buffer, 0, buffer_pos));
			}

			return sb.ToString();
		}

		public override int GetHashCode() {
			return this.dispositionType.ToLowerInvariant().GetHashCode() ^
			       HashCodeCalculator.Calculate(this.parameters);
		}

		public static ContentDispositionHeaderValue Parse(string input) {
			ContentDispositionHeaderValue value;
			if (TryParse(input, out value)) {
				return value;
			}

			throw new FormatException(input);
		}

		private void SetDateValue(string key, DateTimeOffset? value) {
			this.SetValue(key, value == null ? null : ("\"" + value.Value.ToString("r", CultureInfo.InvariantCulture)) + "\"");
		}

		private void SetValue(string key, string value) {
			if (this.parameters == null) {
				this.parameters = new List<NameValueHeaderValue>();
			}

			this.parameters.SetValue(key, value);
		}

		public override string ToString() {
			return this.dispositionType + CollectionExtensions.ToString(this.parameters);
		}

		public static bool TryParse(string input, out ContentDispositionHeaderValue parsedValue) {
			parsedValue = null;

			var lexer = new Lexer(input);
			Token t = lexer.Scan();
			if (t.Kind != Token.Type.Token) {
				return false;
			}

			List<NameValueHeaderValue> parameters = null;
			string type = lexer.GetStringValue(t);

			t = lexer.Scan();

			switch (t.Kind) {
				case Token.Type.SeparatorSemicolon:
					if (!NameValueHeaderValue.ParseParameters(lexer, out parameters)) {
						return false;
					}
					break;
				case Token.Type.End:
					break;
				default:
					return false;
			}

			parsedValue = new ContentDispositionHeaderValue {
				                                                dispositionType = type,
				                                                parameters = parameters
			                                                };

			return true;
		}
	}
}
