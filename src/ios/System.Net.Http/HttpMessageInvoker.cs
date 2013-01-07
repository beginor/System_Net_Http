using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http {

	public class HttpMessageInvoker : Disposable {
		
		private readonly HttpMessageHandler _handler;
		private readonly bool _disposeHandler;

		public HttpMessageInvoker(HttpMessageHandler handler) : this(handler, true) {
		}

		public HttpMessageInvoker(HttpMessageHandler handler, bool disposeHandler) {
			this._handler = handler;
			this._disposeHandler = disposeHandler;
		}

		public virtual Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) {
			return this.SendAsync(request, CancellationToken.None);
		}

		public virtual Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			if (request == null) {
				throw new ArgumentNullException("request");
			}
			return this._handler.SendAsync(request, cancellationToken);
		}

		protected override void Dispose(bool disposing) {
			if (disposing && !this.Disposed) {
				if (this._disposeHandler) {
					this._handler.Dispose();
				}
			}
			base.Dispose(disposing);
		}
	}

}