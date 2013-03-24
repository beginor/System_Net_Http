//
// Lexer.cs
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

using System.Globalization;

namespace System.Net.Http.Headers {
	internal struct Token {
		public enum Type {
			Error,
			End,
			Token,
			QuotedString,
			SeparatorEqual,
			SeparatorSemicolon,
			SeparatorSlash,
			SeparatorDash,
			SeparatorComma,
			OpenParens,
		}

		private readonly Type type;

		public Token(Type type, int startPosition, int endPosition)
			: this() {
			this.type = type;
			this.StartPosition = startPosition;
			this.EndPosition = endPosition;
		}

		public int StartPosition { get; private set; }
		public int EndPosition { get; private set; }

		public Type Kind {
			get {
				return this.type;
			}
		}

		public static implicit operator Type(Token token) {
			return token.type;
		}

		public override string ToString() {
			return this.type.ToString();
		}
	}

	internal class Lexer {
		// any CHAR except CTLs or separators
		private static readonly bool[] token_chars = {
			                                             /*0*/	false, false, false, false, false, false, false, false, false, false,
			                                                  	/*10*/	false, false, false, false, false, false, false, false, false, false,
			                                                  	/*20*/	false, false, false, false, false, false, false, false, false, false,
			                                                  	/*30*/	false, false, false, true, false, true, true, true, true, true,
			                                                  	/*40*/	false, false, true, true, false, true, true, false, true, true,
			                                                  	/*50*/	true, true, true, true, true, true, true, true, false, false,
			                                                  	/*60*/	false, false, false, false, false, true, true, true, true, true,
			                                                  	/*70*/	true, true, true, true, true, true, true, true, true, true,
			                                                  	/*80*/	true, true, true, true, true, true, true, true, true, true,
			                                                  	/*90*/	true, false, false, false, true, true, true, true, true, true,
			                                                  	/*100*/	true, true, true, true, true, true, true, true, true, true,
			                                                  	/*110*/	true, true, true, true, true, true, true, true, true, true,
			                                                  	/*120*/	true, true, true, false, true, false
		                                             };

		private static readonly int last_token_char = token_chars.Length;

		private static readonly string[] dt_formats = new[] {
			                                                    "r",
			                                                    "dddd, dd'-'MMM'-'yy HH:mm:ss 'GMT'",
			                                                    "ddd MMM d HH:mm:ss yyyy",
			                                                    "d MMM yy H:m:s",
			                                                    "ddd, d MMM yyyy H:m:s zzz"
		                                                    };

		private readonly string s;
		private int pos;

		public Lexer(string stream) {
			this.s = stream;
		}

		public string GetStringValue(Token token) {
			return this.s.Substring(token.StartPosition, token.EndPosition - token.StartPosition);
		}

		public string GetStringValue(Token start, Token end) {
			return this.s.Substring(start.StartPosition, end.EndPosition - start.StartPosition);
		}

		public string GetQuotedStringValue(Token start) {
			return this.s.Substring(start.StartPosition + 1, start.EndPosition - start.StartPosition - 2);
		}

		public string GetRemainingStringValue(int position) {
			return position > this.s.Length ? null : this.s.Substring(position);
		}

		public bool IsStarStringValue(Token token) {
			return (token.EndPosition - token.StartPosition) == 1 && this.s[token.StartPosition] == '*';
		}

		public bool TryGetNumericValue(Token token, out int value) {
			return int.TryParse(this.GetStringValue(token), NumberStyles.None, CultureInfo.InvariantCulture, out value);
		}

		public TimeSpan? TryGetTimeSpanValue(Token token) {
			int seconds;
			if (this.TryGetNumericValue(token, out seconds)) {
				return TimeSpan.FromSeconds(seconds);
			}

			return null;
		}

		public bool TryGetDateValue(Token token, out DateTimeOffset value) {
			string text = token == Token.Type.QuotedString ?
				              this.s.Substring(token.StartPosition + 1, token.EndPosition - token.StartPosition - 2) :
				              this.GetStringValue(token);

			return TryGetDateValue(text, out value);
		}

		public static bool TryGetDateValue(string text, out DateTimeOffset value) {
			const DateTimeStyles DefaultStyles = DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowWhiteSpaces;

			return DateTimeOffset.TryParseExact(text, dt_formats, DateTimeFormatInfo.InvariantInfo, DefaultStyles, out value);
		}

		public bool TryGetDoubleValue(Token token, out double value) {
			string s = this.GetStringValue(token);
			return double.TryParse(s, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value);
		}

		public static bool IsValidToken(string input) {
			int i = 0;
			//
			// any CHAR except CTLs or separator
			//
			for (; i < input.Length; ++i) {
				char s = input[i];
				if (s > last_token_char || !token_chars[s]) {
					return false;
				}
			}

			return i > 0;
		}

		public void EatChar() {
			++this.pos;
		}

		public int PeekChar() {
			return this.pos < this.s.Length ? this.s[this.pos] : -1;
		}

		public bool ScanCommentOptional(out string value) {
			Token t = this.Scan();
			if (t != Token.Type.OpenParens) {
				value = null;
				return t == Token.Type.End;
			}

			while (this.pos < this.s.Length) {
				char ch = this.s[this.pos];
				if (ch == ')') {
					++this.pos;
					int start = t.StartPosition;
					value = this.s.Substring(start, this.pos - start);
					return true;
				}

				// any OCTET except CTLs, but including LWS
				if (ch < 32 || ch > 126) {
					break;
				}

				++this.pos;
			}

			value = null;
			return false;
		}

		public Token Scan() {
			int start = this.pos;
			if (this.s == null) {
				return new Token(Token.Type.Error, 0, 0);
			}

			Token.Type ttype;
			if (this.pos >= this.s.Length) {
				ttype = Token.Type.End;
			}
			else {
				ttype = Token.Type.Error;
				start:
				char ch = this.s[this.pos++];
				switch (ch) {
					case ' ':
					case '\t':
						if (this.pos == this.s.Length) {
							ttype = Token.Type.End;
							break;
						}

						goto start;
					case '=':
						ttype = Token.Type.SeparatorEqual;
						break;
					case ';':
						ttype = Token.Type.SeparatorSemicolon;
						break;
					case '/':
						ttype = Token.Type.SeparatorSlash;
						break;
					case '-':
						ttype = Token.Type.SeparatorDash;
						break;
					case ',':
						ttype = Token.Type.SeparatorComma;
						break;
					case '"':
						// Quoted string
						start = this.pos - 1;
						while (this.pos < this.s.Length) {
							ch = this.s[this.pos];
							if (ch == '"') {
								++this.pos;
								ttype = Token.Type.QuotedString;
								break;
							}

							// any OCTET except CTLs, but including LWS
							if (ch < 32 || ch > 126) {
								break;
							}

							++this.pos;
						}

						break;
					case '(':
						start = this.pos - 1;
						ttype = Token.Type.OpenParens;
						break;
					default:
						if (ch <= last_token_char && token_chars[ch]) {
							start = this.pos - 1;

							ttype = Token.Type.Token;
							while (this.pos < this.s.Length) {
								ch = this.s[this.pos];
								if (ch > last_token_char || !token_chars[ch]) {
									break;
								}

								++this.pos;
							}
						}

						break;
				}
			}

			return new Token(ttype, start, this.pos);
		}
	}
}
