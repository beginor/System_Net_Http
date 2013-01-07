using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace System.Net.Http {

	public abstract class HttpContent : Disposable {

		private Encoding _encoding;
		private HttpContentHeaders _headers;

		protected static Encoding DefaultEncoding = Encoding.UTF8;

		public Encoding Encoding {
			get {
				return this._encoding ?? (this._encoding = DefaultEncoding);
			}
			set {
				this._encoding = value;
				if (value != null) {
					this.Headers.ContentEncoding = value.WebName;
				}
			}
		}

		public HttpContentHeaders Headers {
			get {
				return this._headers ?? (this._headers = new HttpContentHeaders());
			}
		}

		protected abstract Task SerializeToStreamAsync(Stream stream);

		public Task<string> ReadAsStringAsync() {
			Stream stream = new MemoryStream();
			return this.SerializeToStreamAsync(stream).ContinueWith(t => {
				string result;
				stream.Position = 0;
				using (var reader = new StreamReader(stream)) {
					result = reader.ReadToEnd();
				}
				return result;
			});
		}

		public Task<byte[]> ReadAsBufferAsync() {
			var tcs = new TaskCompletionSource<byte[]>();
			try {
				Stream stream = new MemoryStream();
				this.SerializeToStreamAsync(stream).ContinueWith(copyTask => {
					try {
						if (copyTask.IsFaulted) {
							stream.Dispose();
							tcs.SetException(copyTask.Exception.GetBaseException());
						}
						else {
							if (copyTask.IsCanceled) {
								stream.Dispose();
								tcs.SetCanceled();
							}
							else {
								stream.Seek(0, SeekOrigin.Begin);
								var buff = new byte[stream.Length];
								Task<int>.Factory.FromAsync(stream.BeginRead, stream.EndRead, buff, 0, buff.Length, null).ContinueWith(readTask => tcs.SetResult(buff));
							}
						}
					}
					catch (Exception ex) {
						stream.Dispose();
						tcs.SetException(ex);
					}
				});
			}
			catch (Exception ex) {
				tcs.SetException(ex);
			}
			return tcs.Task;
		}

		public Task<Stream> ReadAsStreamAsync() {
			var tcs = new TaskCompletionSource<Stream>();
			try {
				Stream stream = new MemoryStream();
				this.SerializeToStreamAsync(stream).ContinueWith(t => {
					try {
						if (t.IsFaulted) {
							stream.Dispose();
							tcs.SetException(t.Exception.GetBaseException());
						}
						else {
							if (t.IsCanceled) {
								stream.Dispose();
								tcs.SetCanceled();
							}
							else {
								stream.Seek(0, SeekOrigin.Begin);
								tcs.SetResult(stream);
							}
						}
					}
					catch (Exception ex) {
						stream.Dispose();
						tcs.SetException(ex);
					}
				});
			}
			catch (Exception ex) {
				tcs.SetException(ex);
			}
			return tcs.Task;
		}

		public Task CopyToAsync(Stream stream) {
			if (stream == null) {
				throw new ArgumentNullException("stream");
			}
			var tcs = new TaskCompletionSource<object>();
			try {
				var task = this.SerializeToStreamAsync(stream);
				if (task == null) {
					throw new InvalidOperationException();
				}
				task.ContinueWith(t => {
					if (t.IsFaulted) {
						tcs.TrySetException(t.Exception.GetBaseException());
						return;
					}
					if (t.IsCanceled) {
						tcs.TrySetCanceled();
						return;
					}
					tcs.TrySetResult(null);
				});
			}
			catch (Exception ex) {
				tcs.TrySetException(ex);
			}
			return tcs.Task;
		}
	}
}