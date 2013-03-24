//
// TransferCodingHeaderValue.cs
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
	public class TransferCodingHeaderValue : ICloneable {
		internal List<NameValueHeaderValue> parameters;
		private string value;

		public TransferCodingHeaderValue(string value) {
			Parser.Token.Check(value);
			this.value = value;
		}

		protected TransferCodingHeaderValue(TransferCodingHeaderValue source) {
			this.value = source.value;
			if (source.parameters != null) {
				foreach (NameValueHeaderValue p in source.parameters) {
					this.Parameters.Add(new NameValueHeaderValue(p));
				}
			}
		}

		internal TransferCodingHeaderValue() {
		}

		public ICollection<NameValueHeaderValue> Parameters {
			get {
				return this.parameters ?? (this.parameters = new List<NameValueHeaderValue>());
			}
		}

		public string Value {
			get {
				return this.value;
			}
		}

		object ICloneable.Clone() {
			return new TransferCodingHeaderValue(this);
		}

		public override bool Equals(object obj) {
			var fchv = obj as TransferCodingHeaderValue;
			return fchv != null &&
			       string.Equals(this.value, fchv.value, StringComparison.OrdinalIgnoreCase) &&
			       this.parameters.SequenceEqual(fchv.parameters);
		}

		public override int GetHashCode() {
			int hc = this.value.ToLowerInvariant().GetHashCode();
			if (this.parameters != null) {
				hc ^= HashCodeCalculator.Calculate(this.parameters);
			}

			return hc;
		}

		public static TransferCodingHeaderValue Parse(string input) {
			TransferCodingHeaderValue value;

			if (TryParse(input, out value)) {
				return value;
			}

			throw new FormatException(input);
		}

		public override string ToString() {
			return this.value + CollectionExtensions.ToString(this.parameters);
		}

		public static bool TryParse(string input, out TransferCodingHeaderValue parsedValue) {
			return TryParse(input, out parsedValue, () => new TransferCodingHeaderValue());
		}

		internal static bool TryParse<T>(string input, out T parsedValue, Func<T> factory) where T : TransferCodingHeaderValue {
			parsedValue = null;

			var lexer = new Lexer(input);
			Token t = lexer.Scan();
			if (t != Token.Type.Token) {
				return false;
			}

			T result = factory();
			result.value = lexer.GetStringValue(t);

			t = lexer.Scan();

			// Parameters parsing
			if (t == Token.Type.SeparatorSemicolon) {
				if (!NameValueHeaderValue.ParseParameters(lexer, out result.parameters)) {
					return false;
				}
			}
			else if (t != Token.Type.End) {
				return false;
			}

			parsedValue = result;
			return true;
		}
	}
}
