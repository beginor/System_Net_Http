//
// HttpHeaderValueCollection.cs
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

using System.Collections;
using System.Collections.Generic;

namespace System.Net.Http.Headers {
	public sealed class HttpHeaderValueCollection<T> : ICollection<T> where T : class {
		private readonly HeaderInfo headerInfo;
		private readonly HttpHeaders headers;
		private readonly List<T> list;

		internal HttpHeaderValueCollection(HttpHeaders headers, HeaderInfo headerInfo) {
			this.list = new List<T>();
			this.headers = headers;
			this.headerInfo = headerInfo;
		}

		public int Count {
			get {
				return this.list.Count;
			}
		}

		public bool IsReadOnly {
			get {
				return false;
			}
		}

		public void Add(T item) {
			this.list.Add(item);
		}

		public void Clear() {
			this.list.Clear();
		}

		public bool Contains(T item) {
			return this.list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex) {
			this.list.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item) {
			return this.list.Remove(item);
		}

		public IEnumerator<T> GetEnumerator() {
			return this.list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		public void ParseAdd(string input) {
			this.headers.AddValue(input, this.headerInfo, false);
		}

		public override string ToString() {
			// This implementation prints different values than
			// what .NET does when one of the values is invalid
			// But it better represents what is actually hold by
			// the collection
			return string.Join(", ", this.list);
		}

		public bool TryParseAdd(string input) {
			return this.headers.AddValue(input, this.headerInfo, true);
		}

		internal T Find(Predicate<T> predicate) {
			return this.list.Find(predicate);
		}

		internal void Remove(Predicate<T> predicate) {
			T item = this.Find(predicate);
			if (item != null) {
				Remove(item);
			}
		}
	}
}
