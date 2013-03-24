//
// HttpMethod.cs
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

using System.Net.Http.Headers;

namespace System.Net.Http {
	public class HttpMethod : IEquatable<HttpMethod> {
		private static readonly HttpMethod delete_method = new HttpMethod("DELETE");
		private static readonly HttpMethod get_method = new HttpMethod("GET");
		private static readonly HttpMethod head_method = new HttpMethod("HEAD");
		private static readonly HttpMethod options_method = new HttpMethod("OPTIONS");
		private static readonly HttpMethod post_method = new HttpMethod("POST");
		private static readonly HttpMethod put_method = new HttpMethod("PUT");
		private static readonly HttpMethod trace_method = new HttpMethod("TRACE");

		private readonly string method;

		public HttpMethod(string method) {
			if (string.IsNullOrEmpty(method)) {
				throw new ArgumentException("method");
			}

			Parser.Token.Check(method);

			this.method = method;
		}

		public static HttpMethod Delete {
			get {
				return delete_method;
			}
		}

		public static HttpMethod Get {
			get {
				return get_method;
			}
		}

		public static HttpMethod Head {
			get {
				return head_method;
			}
		}

		public string Method {
			get {
				return this.method;
			}
		}

		public static HttpMethod Options {
			get {
				return options_method;
			}
		}

		public static HttpMethod Post {
			get {
				return post_method;
			}
		}

		public static HttpMethod Put {
			get {
				return put_method;
			}
		}

		public static HttpMethod Trace {
			get {
				return trace_method;
			}
		}

		public bool Equals(HttpMethod other) {
			return string.Equals(this.method, other.method, StringComparison.OrdinalIgnoreCase);
		}

		public static bool operator ==(HttpMethod left, HttpMethod right) {
			if ((object)left == null || (object)right == null) {
				return ReferenceEquals(left, right);
			}

			return left.Equals(right);
		}

		public static bool operator !=(HttpMethod left, HttpMethod right) {
			return !(left == right);
		}

		public override bool Equals(object obj) {
			var other = obj as HttpMethod;
			return !ReferenceEquals(other, null) && Equals(other);
		}

		public override int GetHashCode() {
			return this.method.GetHashCode();
		}

		public override string ToString() {
			return this.method;
		}
	}
}
