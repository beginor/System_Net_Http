using System.IO;
using System.Threading.Tasks;

namespace System.Net.Http {

	public class ByteArrayContent : HttpContent {

		private readonly byte[] _content;
		private readonly int _offset;
		private readonly int _count;

		public ByteArrayContent(byte[] content) {
			this._content = content;
			this._offset = 0;
			this._count = content.Length;
		}

		public ByteArrayContent(byte[] content, int offset, int count) {
			this._content = content;
			this._offset = offset;
			this._count = count;
		}

		protected override Task SerializeToStreamAsync(Stream stream) {
			return Task.Factory.FromAsync(stream.BeginWrite, stream.EndWrite, this._content, this._offset, this._count, null);
		}
	}
}