//
// CacheControlHeaderValue.cs
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
using System.Globalization;
using System.Text;

namespace System.Net.Http.Headers {
	public class CacheControlHeaderValue : ICloneable {
		private List<NameValueHeaderValue> extensions;
		private List<string> no_cache_headers, private_headers;

		public ICollection<NameValueHeaderValue> Extensions {
			get {
				return this.extensions ?? (this.extensions = new List<NameValueHeaderValue>());
			}
		}

		public TimeSpan? MaxAge { get; set; }

		public bool MaxStale { get; set; }

		public TimeSpan? MaxStaleLimit { get; set; }

		public TimeSpan? MinFresh { get; set; }

		public bool MustRevalidate { get; set; }

		public bool NoCache { get; set; }

		public ICollection<string> NoCacheHeaders {
			get {
				return this.no_cache_headers ?? (this.no_cache_headers = new List<string>());
			}
		}

		public bool NoStore { get; set; }

		public bool NoTransform { get; set; }

		public bool OnlyIfCached { get; set; }

		public bool Private { get; set; }

		public ICollection<string> PrivateHeaders {
			get {
				return this.private_headers ?? (this.private_headers = new List<string>());
			}
		}

		public bool ProxyRevalidate { get; set; }

		public bool Public { get; set; }

		public TimeSpan? SharedMaxAge { get; set; }

		object ICloneable.Clone() {
			var copy = (CacheControlHeaderValue)this.MemberwiseClone();
			if (this.extensions != null) {
				copy.extensions = new List<NameValueHeaderValue>();
				foreach (NameValueHeaderValue entry in this.extensions) {
					copy.extensions.Add(entry);
				}
			}

			if (this.no_cache_headers != null) {
				copy.no_cache_headers = new List<string>();
				foreach (string entry in this.no_cache_headers) {
					copy.no_cache_headers.Add(entry);
				}
			}

			if (this.private_headers != null) {
				copy.private_headers = new List<string>();
				foreach (string entry in this.private_headers) {
					copy.private_headers.Add(entry);
				}
			}

			return copy;
		}

		public override bool Equals(object obj) {
			var source = obj as CacheControlHeaderValue;
			if (source == null) {
				return false;
			}

			if (this.MaxAge != source.MaxAge || this.MaxStale != source.MaxStale || this.MaxStaleLimit != source.MaxStaleLimit ||
			    this.MinFresh != source.MinFresh || this.MustRevalidate != source.MustRevalidate || this.NoCache != source.NoCache ||
			    this.NoStore != source.NoStore || this.NoTransform != source.NoTransform || this.OnlyIfCached != source.OnlyIfCached ||
			    this.Private != source.Private || this.ProxyRevalidate != source.ProxyRevalidate || this.Public != source.Public ||
			    this.SharedMaxAge != source.SharedMaxAge) {
				return false;
			}

			return this.extensions.SequenceEqual(source.extensions) &&
			       this.no_cache_headers.SequenceEqual(source.no_cache_headers) &&
			       this.private_headers.SequenceEqual(source.private_headers);
		}

		public override int GetHashCode() {
			int hc = 29;
			unchecked {
				hc = hc * 29 + HashCodeCalculator.Calculate(this.extensions);
				hc = hc * 29 + this.MaxAge.GetHashCode();
				hc = hc * 29 + this.MaxStale.GetHashCode();
				hc = hc * 29 + this.MaxStaleLimit.GetHashCode();
				hc = hc * 29 + this.MinFresh.GetHashCode();
				hc = hc * 29 + this.MustRevalidate.GetHashCode();
				hc = hc * 29 + HashCodeCalculator.Calculate(this.no_cache_headers);
				hc = hc * 29 + this.NoCache.GetHashCode();
				hc = hc * 29 + this.NoStore.GetHashCode();
				hc = hc * 29 + this.NoTransform.GetHashCode();
				hc = hc * 29 + this.OnlyIfCached.GetHashCode();
				hc = hc * 29 + this.Private.GetHashCode();
				hc = hc * 29 + HashCodeCalculator.Calculate(this.private_headers);
				hc = hc * 29 + this.ProxyRevalidate.GetHashCode();
				hc = hc * 29 + this.Public.GetHashCode();
				hc = hc * 29 + this.SharedMaxAge.GetHashCode();
			}

			return hc;
		}

		public static CacheControlHeaderValue Parse(string input) {
			CacheControlHeaderValue value;
			if (TryParse(input, out value)) {
				return value;
			}

			throw new FormatException(input);
		}

		public static bool TryParse(string input, out CacheControlHeaderValue parsedValue) {
			parsedValue = null;
			if (input == null) {
				return true;
			}

			var value = new CacheControlHeaderValue();

			var lexer = new Lexer(input);
			Token t;
			do {
				t = lexer.Scan();
				if (t != Token.Type.Token) {
					return false;
				}

				string s = lexer.GetStringValue(t);
				bool token_read = false;
				TimeSpan? ts;
				switch (s) {
					case "no-store":
						value.NoStore = true;
						break;
					case "no-transform":
						value.NoTransform = true;
						break;
					case "only-if-cached":
						value.OnlyIfCached = true;
						break;
					case "public":
						value.Public = true;
						break;
					case "must-revalidate":
						value.MustRevalidate = true;
						break;
					case "proxy-revalidate":
						value.ProxyRevalidate = true;
						break;
					case "max-stale":
						value.MaxStale = true;
						t = lexer.Scan();
						if (t != Token.Type.SeparatorEqual) {
							token_read = true;
							break;
						}

						t = lexer.Scan();
						if (t != Token.Type.Token) {
							return false;
						}

						ts = lexer.TryGetTimeSpanValue(t);
						if (ts == null) {
							return false;
						}

						value.MaxStaleLimit = ts;
						break;
					case "max-age":
					case "s-maxage":
					case "min-fresh":
						t = lexer.Scan();
						if (t != Token.Type.SeparatorEqual) {
							return false;
						}

						t = lexer.Scan();
						if (t != Token.Type.Token) {
							return false;
						}

						ts = lexer.TryGetTimeSpanValue(t);
						if (ts == null) {
							return false;
						}

						switch (s.Length) {
							case 7:
								value.MaxAge = ts;
								break;
							case 8:
								value.SharedMaxAge = ts;
								break;
							default:
								value.MinFresh = ts;
								break;
						}

						break;
					case "private":
					case "no-cache":
						if (s.Length == 7) {
							value.Private = true;
						}
						else {
							value.NoCache = true;
						}

						t = lexer.Scan();
						if (t != Token.Type.SeparatorEqual) {
							token_read = true;
							break;
						}

						t = lexer.Scan();
						if (t != Token.Type.QuotedString) {
							return false;
						}

						foreach (string entry in lexer.GetQuotedStringValue(t).Split(',')) {
							string qs = entry.Trim('\t', ' ');

							if (s.Length == 7) {
								value.PrivateHeaders.Add(qs);
							}
							else {
								value.NoCache = true;
								value.NoCacheHeaders.Add(qs);
							}
						}
						break;
					default:
						string name = lexer.GetStringValue(t);
						string svalue = null;

						t = lexer.Scan();
						if (t == Token.Type.SeparatorEqual) {
							t = lexer.Scan();
							switch (t.Kind) {
								case Token.Type.Token:
								case Token.Type.QuotedString:
									svalue = lexer.GetStringValue(t);
									break;
								default:
									return false;
							}
						}
						else {
							token_read = true;
						}

						value.Extensions.Add(NameValueHeaderValue.Create(name, svalue));
						break;
				}

				if (!token_read) {
					t = lexer.Scan();
				}
			}
			while (t == Token.Type.SeparatorComma);

			if (t != Token.Type.End) {
				return false;
			}

			parsedValue = value;
			return true;
		}

		public override string ToString() {
			const string separator = ", ";

			var sb = new StringBuilder();
			if (this.NoStore) {
				sb.Append("no-store");
				sb.Append(separator);
			}

			if (this.NoTransform) {
				sb.Append("no-transform");
				sb.Append(separator);
			}

			if (this.OnlyIfCached) {
				sb.Append("only-if-cached");
				sb.Append(separator);
			}

			if (this.Public) {
				sb.Append("public");
				sb.Append(separator);
			}

			if (this.MustRevalidate) {
				sb.Append("must-revalidate");
				sb.Append(separator);
			}

			if (this.ProxyRevalidate) {
				sb.Append("proxy-revalidate");
				sb.Append(separator);
			}

			if (this.NoCache) {
				sb.Append("no-cache");
				if (this.no_cache_headers != null) {
					sb.Append("=\"");
					this.no_cache_headers.ToStringBuilder(sb);
					sb.Append("\"");
				}

				sb.Append(separator);
			}

			if (this.MaxAge != null) {
				sb.Append("max-age=");
				sb.Append(this.MaxAge.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture));
				sb.Append(separator);
			}

			if (this.SharedMaxAge != null) {
				sb.Append("s-maxage=");
				sb.Append(this.SharedMaxAge.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture));
				sb.Append(separator);
			}

			if (this.MaxStale) {
				sb.Append("max-stale");
				if (this.MaxStaleLimit != null) {
					sb.Append("=");
					sb.Append(this.MaxStaleLimit.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture));
				}

				sb.Append(separator);
			}

			if (this.MinFresh != null) {
				sb.Append("min-fresh=");
				sb.Append(this.MinFresh.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture));
				sb.Append(separator);
			}

			if (this.Private) {
				sb.Append("private");
				if (this.private_headers != null) {
					sb.Append("=\"");
					this.private_headers.ToStringBuilder(sb);
					sb.Append("\"");
				}

				sb.Append(separator);
			}

			this.extensions.ToStringBuilder(sb);

			if (sb.Length > 2 && sb[sb.Length - 2] == ',' && sb[sb.Length - 1] == ' ') {
				sb.Remove(sb.Length - 2, 2);
			}

			return sb.ToString();
		}
	}
}
