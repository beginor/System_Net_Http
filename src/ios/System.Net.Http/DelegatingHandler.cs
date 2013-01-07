using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http {

	public class DelegatingHandler : HttpMessageHandler {

		private HttpMessageHandler _innerHandler;
		private bool _operationStarted;

		public HttpMessageHandler InnerHandler {
			get {
				this.CheckDisposed();
				return this._innerHandler;
			}
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				this.CheckDisposedOrStarted();
				this._innerHandler = value;
			}
		}

		protected DelegatingHandler() {
		}

		protected DelegatingHandler(HttpMessageHandler innerHandler) {
			this.InnerHandler = innerHandler;
		}

		private void CheckDisposedOrStarted() {
			this.CheckDisposed();
			if (this._operationStarted) {
				throw new InvalidOperationException("SR.net_http_client_operation_started");
			}
		}

		protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			if (request == null) {
				throw new ArgumentNullException("request", "SR.net_http_handler_norequest");
			}
			this.SetOperationStarted();
			return this._innerHandler.SendAsync(request, cancellationToken);
		}

		private void SetOperationStarted() {
			this.CheckDisposed();
			if (this._innerHandler == null) {
				throw new InvalidOperationException("SR.net_http_handler_not_assigned");
			}
			if (!this._operationStarted) {
				this._operationStarted = true;
			}
		}

		protected override void Dispose(bool disposing) {
			if (!this.Disposed && disposing) {
				if (this.InnerHandler != null) {
					this.InnerHandler.Dispose();
				}
			}
			base.Dispose(disposing);
		}
	}
}