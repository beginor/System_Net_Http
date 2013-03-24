//
// EntityTagHeaderValue.cs
//
// Authors:
//	Marek Safar  <marek.safar@gmail.com>
//
// Copyright (C) 2011 Xamarin Inc (http://www.xamarin.com)
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

namespace System.Net.Http.Headers {
	public class EntityTagHeaderValue : ICloneable {
		private static readonly EntityTagHeaderValue any = new EntityTagHeaderValue {Tag = "*"};

		public EntityTagHeaderValue(string tag) {
			Parser.Token.CheckQuotedString(tag);
			this.Tag = tag;
		}

		public EntityTagHeaderValue(string tag, bool isWeak)
			: this(tag) {
			this.IsWeak = isWeak;
		}

		internal EntityTagHeaderValue() {
		}

		public static EntityTagHeaderValue Any {
			get {
				return any;
			}
		}

		public bool IsWeak { get; internal set; }
		public string Tag { get; internal set; }

		object ICloneable.Clone() {
			return this.MemberwiseClone();
		}

		public override bool Equals(object obj) {
			var source = obj as EntityTagHeaderValue;
			return source != null && source.Tag == this.Tag &&
			       string.Equals(source.Tag, this.Tag, StringComparison.Ordinal);
		}

		public override int GetHashCode() {
			return this.IsWeak.GetHashCode() ^ this.Tag.GetHashCode();
		}

		public static EntityTagHeaderValue Parse(string input) {
			EntityTagHeaderValue value;
			if (TryParse(input, out value)) {
				return value;
			}

			throw new FormatException(input);
		}

		public static bool TryParse(string input, out EntityTagHeaderValue parsedValue) {
			parsedValue = null;

			var lexer = new Lexer(input);
			Token t = lexer.Scan();
			bool is_weak = false;

			if (t == Token.Type.Token) {
				if (lexer.GetStringValue(t) != "W" || lexer.PeekChar() != '/') {
					return false;
				}

				is_weak = true;
				lexer.EatChar();
				t = lexer.Scan();
			}

			if (t != Token.Type.QuotedString) {
				return false;
			}

			if (lexer.Scan() != Token.Type.End) {
				return false;
			}

			parsedValue = new EntityTagHeaderValue();
			parsedValue.Tag = lexer.GetStringValue(t);
			parsedValue.IsWeak = is_weak;
			return true;
		}

		public override string ToString() {
			return this.IsWeak ?
				       "W/" + this.Tag :
				       this.Tag;
		}
	}
}
