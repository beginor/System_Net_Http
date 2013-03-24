//
// MediaTypeHeaderValue.cs
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

using System.Collections.Generic;

namespace System.Net.Http.Headers {
	public class MediaTypeHeaderValue : ICloneable {
		private string media_type;
		internal List<NameValueHeaderValue> parameters;

		public MediaTypeHeaderValue(string mediaType) {
			this.MediaType = mediaType;
		}

		protected MediaTypeHeaderValue(MediaTypeHeaderValue source) {
			if (source == null) {
				throw new ArgumentNullException("source");
			}

			this.media_type = source.media_type;
			if (source.parameters != null) {
				foreach (NameValueHeaderValue item in source.parameters) {
					this.Parameters.Add(new NameValueHeaderValue(item));
				}
			}
		}

		internal MediaTypeHeaderValue() {
		}

		public string CharSet {
			get {
				if (this.parameters == null) {
					return null;
				}

				NameValueHeaderValue found = this.parameters.Find(l => string.Equals(l.Name, "charset", StringComparison.OrdinalIgnoreCase));
				if (found == null) {
					return null;
				}

				return found.Value;
			}

			set {
				if (this.parameters == null) {
					this.parameters = new List<NameValueHeaderValue>();
				}

				this.parameters.SetValue("charset", value);
			}
		}

		public string MediaType {
			get {
				return this.media_type;
			}
			set {
				if (value == null) {
					throw new ArgumentNullException("MediaType");
				}

				string temp;
				if (TryParseMediaType(new Lexer(value), out temp) != Token.Type.End) {
					throw new FormatException();
				}

				this.media_type = temp;
			}
		}

		public ICollection<NameValueHeaderValue> Parameters {
			get {
				return this.parameters ?? (this.parameters = new List<NameValueHeaderValue>());
			}
		}

		object ICloneable.Clone() {
			return new MediaTypeHeaderValue(this);
		}

		public override bool Equals(object obj) {
			var source = obj as MediaTypeHeaderValue;
			if (source == null) {
				return false;
			}

			return string.Equals(source.media_type, this.media_type, StringComparison.OrdinalIgnoreCase) &&
			       source.parameters.SequenceEqual(this.parameters);
		}

		public override int GetHashCode() {
			return this.media_type.ToLowerInvariant().GetHashCode() ^ HashCodeCalculator.Calculate(this.parameters);
		}

		public static MediaTypeHeaderValue Parse(string input) {
			MediaTypeHeaderValue value;
			if (TryParse(input, out value)) {
				return value;
			}

			throw new FormatException(input);
		}

		public override string ToString() {
			if (this.parameters == null) {
				return this.media_type;
			}

			return this.media_type + CollectionExtensions.ToString(this.parameters);
		}

		public static bool TryParse(string input, out MediaTypeHeaderValue parsedValue) {
			parsedValue = null;

			var lexer = new Lexer(input);

			string media;
			List<NameValueHeaderValue> parameters = null;
			Token? token = TryParseMediaType(lexer, out media);
			if (token == null) {
				return false;
			}

			switch (token.Value.Kind) {
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

			parsedValue = new MediaTypeHeaderValue {
				                                       media_type = media,
				                                       parameters = parameters
			                                       };

			return true;
		}

		internal static bool TryParse<T>(string input, out T parsedValue, Func<T> factory) where T : MediaTypeHeaderValue {
			parsedValue = null;

			var lexer = new Lexer(input);

			string media;
			List<NameValueHeaderValue> parameters = null;
			Token? token = TryParseMediaType(lexer, out media);
			if (token == null) {
				return false;
			}

			switch (token.Value.Kind) {
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

			parsedValue = factory();
			parsedValue.media_type = media;
			parsedValue.parameters = parameters;

			return true;
		}

		private static Token? TryParseMediaType(Lexer lexer, out string media) {
			media = null;

			Token token = lexer.Scan();
			if (token != Token.Type.Token) {
				return null;
			}

			if (lexer.Scan() != Token.Type.SeparatorSlash) {
				return null;
			}

			Token token2 = lexer.Scan();
			if (token2 != Token.Type.Token) {
				return null;
			}

			media = lexer.GetStringValue(token) + "/" + lexer.GetStringValue(token2);

			return lexer.Scan();
		}
	}
}