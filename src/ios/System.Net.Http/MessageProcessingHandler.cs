using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http {

	public abstract class MessageProcessingHandler : DelegatingHandler {

		protected MessageProcessingHandler() {
		}

		protected MessageProcessingHandler(HttpMessageHandler innerHandler) : base(innerHandler) {
		}

		protected abstract HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken);

		protected abstract HttpResponseMessage ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken);

		protected internal override sealed Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			if (request == null) {
				throw new ArgumentNullException("request");
			}
			var tcs = new TaskCompletionSource<HttpResponseMessage>();
			try {
				var request2 = this.ProcessRequest(request, cancellationToken);
				var task2 = base.SendAsync(request2, cancellationToken);
				task2.ContinueWith(task => {
					if (task.IsFaulted) {
						tcs.SetException(task.Exception.GetBaseException());
					}
					else {
						if (task.IsCanceled) {
							tcs.TrySetCanceled();
						}
						else {
							if (task.Result == null) {
								tcs.TrySetException(new InvalidOperationException("SR.net_http_handler_noresponse"));
							}
							else {
								try {
									var response = this.ProcessResponse(task.Result, cancellationToken);
									tcs.TrySetResult(response);
								}
								catch (OperationCanceledException ex) {
									HandleCanceledOperations(cancellationToken, tcs, ex);
								}
								catch (Exception ex) {
									tcs.TrySetException(ex);
								}
							}
						}
					}
				}, TaskContinuationOptions.ExecuteSynchronously);
			}
			catch (OperationCanceledException ex) {
				HandleCanceledOperations(cancellationToken, tcs, ex);
			}
			catch (Exception ex) {
				tcs.TrySetException(ex);
			}
			return tcs.Task;
		}

		private static void HandleCanceledOperations(CancellationToken cancellationToken, TaskCompletionSource<HttpResponseMessage> tcs, OperationCanceledException e) {
			if (cancellationToken.IsCancellationRequested && e.CancellationToken == cancellationToken) {
				tcs.TrySetCanceled();
			}
			else {
				tcs.TrySetException(e);
			}
		}

	}
}