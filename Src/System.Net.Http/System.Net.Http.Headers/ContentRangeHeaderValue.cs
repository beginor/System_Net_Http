//
// ContentRangeHeaderValue.cs
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

using System.Globalization;
using System.Text;

namespace System.Net.Http.Headers {
	public class ContentRangeHeaderValue : ICloneable {
		private string unit = "bytes";

		private ContentRangeHeaderValue() {
		}

		public ContentRangeHeaderValue(long length) {
			if (length < 0) {
				throw new ArgumentOutOfRangeException("length");
			}

			this.Length = length;
		}

		public ContentRangeHeaderValue(long from, long to) {
			if (from < 0 || from > to) {
				throw new ArgumentOutOfRangeException("from");
			}

			this.From = from;
			this.To = to;
		}

		public ContentRangeHeaderValue(long from, long to, long length)
			: this(from, to) {
			if (length < 0) {
				throw new ArgumentOutOfRangeException("length");
			}

			if (to > length) {
				throw new ArgumentOutOfRangeException("to");
			}

			this.Length = length;
		}

		public long? From { get; private set; }

		public bool HasLength {
			get {
				return this.Length != null;
			}
		}

		public bool HasRange {
			get {
				return this.From != null;
			}
		}

		public long? Length { get; private set; }
		public long? To { get; private set; }

		public string Unit {
			get {
				return this.unit;
			}
			set {
				if (value == null) {
					throw new ArgumentNullException("Unit");
				}

				Parser.Token.Check(value);

				this.unit = value;
			}
		}

		object ICloneable.Clone() {
			return this.MemberwiseClone();
		}

		public override bool Equals(object obj) {
			var source = obj as ContentRangeHeaderValue;
			if (source == null) {
				return false;
			}

			return source.Length == this.Length && source.From == this.From && source.To == this.To &&
			       string.Equals(source.unit, this.unit, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode() {
			return this.Unit.GetHashCode() ^ this.Length.GetHashCode() ^
			       this.From.GetHashCode() ^ this.To.GetHashCode() ^
			       this.unit.ToLowerInvariant().GetHashCode();
		}

		public static ContentRangeHeaderValue Parse(string input) {
			ContentRangeHeaderValue value;
			if (TryParse(input, out value)) {
				return value;
			}

			throw new FormatException(input);
		}

		public static bool TryParse(string input, out ContentRangeHeaderValue parsedValue) {
			parsedValue = null;

			var lexer = new Lexer(input);
			Token t = lexer.Scan();
			if (t != Token.Type.Token) {
				return false;
			}

			var value = new ContentRangeHeaderValue();
			value.unit = lexer.GetStringValue(t);

			t = lexer.Scan();
			if (t != Token.Type.Token) {
				return false;
			}

			int nvalue;
			if (!lexer.IsStarStringValue(t)) {
				if (!lexer.TryGetNumericValue(t, out nvalue)) {
					string s = lexer.GetStringValue(t);
					if (s.Length < 3) {
						return false;
					}

					string[] sep = s.Split('-');
					if (sep.Length != 2) {
						return false;
					}

					if (!int.TryParse(sep[0], NumberStyles.None, CultureInfo.InvariantCulture, out nvalue)) {
						return false;
					}

					value.From = nvalue;

					if (!int.TryParse(sep[1], NumberStyles.None, CultureInfo.InvariantCulture, out nvalue)) {
						return false;
					}

					value.To = nvalue;
				}
				else {
					value.From = nvalue;

					t = lexer.Scan();
					if (t != Token.Type.SeparatorDash) {
						return false;
					}

					t = lexer.Scan();

					if (!lexer.TryGetNumericValue(t, out nvalue)) {
						return false;
					}

					value.To = nvalue;
				}
			}

			t = lexer.Scan();

			if (t != Token.Type.SeparatorSlash) {
				return false;
			}

			t = lexer.Scan();

			if (!lexer.IsStarStringValue(t)) {
				if (!lexer.TryGetNumericValue(t, out nvalue)) {
					return false;
				}

				value.Length = nvalue;
			}

			t = lexer.Scan();

			if (t != Token.Type.End) {
				return false;
			}

			parsedValue = value;

			return true;
		}

		public override string ToString() {
			var sb = new StringBuilder(this.unit);
			sb.Append(" ");
			if (this.From == null) {
				sb.Append("*");
			}
			else {
				sb.Append(this.From.Value.ToString(CultureInfo.InvariantCulture));
				sb.Append("-");
				sb.Append(this.To.Value.ToString(CultureInfo.InvariantCulture));
			}

			sb.Append("/");
			sb.Append(this.Length == null ? "*" :
				          this.Length.Value.ToString(CultureInfo.InvariantCulture));

			return sb.ToString();
		}
	}
}
