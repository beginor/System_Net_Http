namespace System.Net.Http {

	public partial class Disposable : IDisposable {

		protected bool Disposed {
			get;
			private set;
		}

		public void Dispose() {
			if (this.Disposed) {
				throw new ObjectDisposedException("Object has bean disposabled!");
			}
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			this.Disposed = true;
		}

		protected void CheckDisposed() {
			if (this.Disposed) {
				throw new ObjectDisposedException(this.GetType().FullName);
			}
		}
	}
}