//
// HttpContent.cs
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

using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http {

	public abstract class HttpContent : IDisposable {

		private FixedMemoryStream buffer;
		private bool disposed;
		private HttpContentHeaders headers;
		private Stream stream;

		public HttpContentHeaders Headers {
			get {
				return this.headers ?? (this.headers = new HttpContentHeaders(this));
			}
		}

		public void Dispose() {
			this.Dispose(true);
		}

		public Task CopyToAsync (Stream stream)
		{
			return CopyToAsync (stream, null);
		}

		public Task CopyToAsync(Stream stream , TransportContext context) {
			if (stream == null) {
				throw new ArgumentNullException("stream");
			}

			if (this.buffer != null) {
				return this.buffer.CopyToAsync(stream);
			}

			return this.SerializeToStreamAsync(stream , context);
		}

		protected virtual async Task<Stream> CreateContentReadStreamAsync() {
			await this.LoadIntoBufferAsync().ConfigureAwait(false);
			return this.buffer;
		}

		private static FixedMemoryStream CreateFixedMemoryStream(long maxBufferSize) {
			return new FixedMemoryStream(maxBufferSize);
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing && !this.disposed) {
				this.disposed = true;

				if (this.buffer != null) {
					this.buffer.Dispose();
				}
			}
		}

		public Task LoadIntoBufferAsync() {
			return this.LoadIntoBufferAsync(65536);
		}

		public async Task LoadIntoBufferAsync(long maxBufferSize) {
			if (this.disposed) {
				throw new ObjectDisposedException(this.GetType().ToString());
			}

			if (this.buffer != null) {
				return;
			}

			this.buffer = CreateFixedMemoryStream(maxBufferSize);
			await this.SerializeToStreamAsync(this.buffer , null).ConfigureAwait(false);
			this.buffer.Seek(0, SeekOrigin.Begin);
		}

		public async Task<Stream> ReadAsStreamAsync() {
			if (this.disposed) {
				throw new ObjectDisposedException(this.GetType().ToString());
			}

			if (this.buffer != null) {
				return new MemoryStream(this.buffer.GetBuffer(), 0, (int)this.buffer.Length, false);
			}

			if (this.stream == null) {
				this.stream = await this.CreateContentReadStreamAsync().ConfigureAwait(false);
			}

			return this.stream;
		}

		public async Task<byte[]> ReadAsByteArrayAsync() {
			await this.LoadIntoBufferAsync().ConfigureAwait(false);
			return this.buffer.ToArray();
		}

		public async Task<string> ReadAsStringAsync() {
			await this.LoadIntoBufferAsync().ConfigureAwait(false);
			if (this.buffer.Length == 0) {
				return string.Empty;
			}

			Encoding encoding;
			if (this.headers != null && this.headers.ContentType != null && this.headers.ContentType.CharSet != null) {
				encoding = Encoding.GetEncoding(this.headers.ContentType.CharSet);
			}
			else {
				encoding = Encoding.UTF8;
			}

			return encoding.GetString(this.buffer.GetBuffer(), 0, (int)this.buffer.Length);
		}

		protected internal abstract Task SerializeToStreamAsync(Stream stream , TransportContext context);

		protected internal abstract bool TryComputeLength(out long length);

		private sealed class FixedMemoryStream : MemoryStream {
			private readonly long maxSize;

			public FixedMemoryStream(long maxSize) {
				this.maxSize = maxSize;
			}

			private void CheckOverflow(int count) {
				if (this.Length + count > this.maxSize) {
					throw new HttpRequestException(string.Format("Cannot write more bytes to the buffer than the configured maximum buffer size: {0}", this.maxSize));
				}
			}

			public override void WriteByte(byte value) {
				this.CheckOverflow(1);
				base.WriteByte(value);
			}

			public override void Write(byte[] buffer, int offset, int count) {
				this.CheckOverflow(count);
				base.Write(buffer, offset, count);
			}
		}
	}
}
