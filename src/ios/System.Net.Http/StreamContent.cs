using System;
using System.IO;
using System.Threading.Tasks;

namespace System.Net.Http {

	public class StreamContent : HttpContent {
		
		private readonly Stream _stream;
		private bool _contentConsumed;

		public StreamContent(Stream stream) {
			this._stream = stream;
		}

		protected override Task SerializeToStreamAsync(Stream stream) {
			this.PrepareContent();
			return Task.Factory.StartNew(() => this._stream.CopyTo(stream));
		}

		private void PrepareContent() {
			if (this._contentConsumed) {
				if (!this._stream.CanSeek) {
					throw new InvalidOperationException("Content stream already readed.");
				}
				this._stream.Position = 0;
			}
			this._contentConsumed = true;
		}

		protected override void Dispose(bool disposing) {
			if (!this.Disposed && disposing) {
				this._stream.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}