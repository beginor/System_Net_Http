//
// HttpClient.cs
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
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http {
	public class HttpClient : HttpMessageInvoker {
		private static readonly TimeSpan TimeoutDefault = TimeSpan.FromSeconds(100);

		private Uri base_address;
		private long buffer_size;
		private CancellationTokenSource cancellation_token;
		private bool disposed;
		private HttpRequestHeaders headers;
		private TimeSpan timeout;

		public HttpClient()
			: this(new HttpClientHandler(), true) {
		}

		public HttpClient(HttpMessageHandler handler)
			: this(handler, true) {
		}

		public HttpClient(HttpMessageHandler handler, bool disposeHandler)
			: base(handler, disposeHandler) {
			this.buffer_size = int.MaxValue;
			this.timeout = TimeoutDefault;
		}

		public Uri BaseAddress {
			get {
				return this.base_address;
			}
			set {
				this.base_address = value;
			}
		}

		public HttpRequestHeaders DefaultRequestHeaders {
			get {
				return this.headers ?? (this.headers = new HttpRequestHeaders());
			}
		}

		public long MaxResponseContentBufferSize {
			get {
				return this.buffer_size;
			}
			set {
				if (value <= 0) {
					throw new ArgumentOutOfRangeException();
				}

				this.buffer_size = value;
			}
		}

		public TimeSpan Timeout {
			get {
				return this.timeout;
			}
			set {
				if ( /*value != System.Threading.Timeout.InfiniteTimeSpan && */value < TimeSpan.Zero) {
					throw new ArgumentOutOfRangeException();
				}

				this.timeout = value;
			}
		}

		public void CancelPendingRequests() {
			if (this.cancellation_token != null) {
				this.cancellation_token.Cancel();
			}

			this.cancellation_token = new CancellationTokenSource();
		}

		protected override void Dispose(bool disposing) {
			if (disposing && !this.disposed) {
				this.disposed = true;

				if (this.cancellation_token != null) {
					this.cancellation_token.Dispose();
				}
			}

			base.Dispose(disposing);
		}

		public Task<HttpResponseMessage> DeleteAsync(string requestUri) {
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri));
		}

		public Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken) {
			return SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri), cancellationToken);
		}

		public Task<HttpResponseMessage> DeleteAsync(Uri requestUri) {
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri));
		}

		public Task<HttpResponseMessage> DeleteAsync(Uri requestUri, CancellationToken cancellationToken) {
			return SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri), cancellationToken);
		}

		public Task<HttpResponseMessage> GetAsync(string requestUri) {
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri));
		}

		public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken) {
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri));
		}

		public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption) {
			return SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption);
		}

		public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken) {
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption, cancellationToken);
		}

		public Task<HttpResponseMessage> GetAsync(Uri requestUri) {
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri));
		}

		public Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken) {
			return SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);
		}

		public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption) {
			return SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption);
		}

		public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken) {
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption, cancellationToken);
		}

		public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content) {
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri) {Content = content});
		}

		public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken) {
			return SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri) {Content = content}, cancellationToken);
		}

		public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content) {
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri) {Content = content});
		}

		public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken) {
			return SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri) {Content = content}, cancellationToken);
		}

		public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content) {
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri) {Content = content});
		}

		public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken) {
			return SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri) {Content = content}, cancellationToken);
		}

		public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content) {
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri) {Content = content});
		}

		public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken) {
			return SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri) {Content = content}, cancellationToken);
		}

		public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) {
			return this.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
		}

		public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption) {
			return this.SendAsync(request, completionOption, CancellationToken.None);
		}

		public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			return this.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
		}

		public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken) {
			if (request == null) {
				throw new ArgumentNullException("request");
			}

			if (request.SetIsUsed()) {
				throw new InvalidOperationException("Cannot send the same request message multiple times");
			}

			if (request.RequestUri == null) {
				if (this.base_address == null) {
					throw new InvalidOperationException("The request URI must either be an absolute URI or BaseAddress must be set");
				}

				request.RequestUri = this.base_address;
			}
			else if (!request.RequestUri.IsAbsoluteUri) {
				if (this.base_address == null) {
					throw new InvalidOperationException("The request URI must either be an absolute URI or BaseAddress must be set");
				}

				request.RequestUri = new Uri(this.base_address, request.RequestUri);
			}

			if (this.headers != null) {
				request.Headers.AddHeaders(this.headers);
			}

			return this.SendAsyncWorker(request, completionOption, cancellationToken);
		}

		private async Task<HttpResponseMessage> SendAsyncWorker(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken) {
			try {
				if (this.cancellation_token == null) {
					this.cancellation_token = new CancellationTokenSource();
				}

				using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(this.cancellation_token.Token, cancellationToken)) {
					cts.CancelAfter(this.timeout);

					Task<HttpResponseMessage> task = base.SendAsync(request, cts.Token);
					if (task == null) {
						throw new InvalidOperationException("Handler failed to return a value");
					}

					HttpResponseMessage response = await task/*.ConfigureAwait(false)*/;
					if (response == null) {
						throw new InvalidOperationException("Handler failed to return a response");
					}

					//
					// Read the content when default HttpCompletionOption.ResponseContentRead is set
					//
					if (response.Content != null && (completionOption & HttpCompletionOption.ResponseHeadersRead) == 0) {
						await response.Content.LoadIntoBufferAsync(this.MaxResponseContentBufferSize).ConfigureAwait(false);
					}

					return response;
				}
			}
			finally {
				this.cancellation_token.Dispose();
				this.cancellation_token = null;
			}
		}

		public async Task<byte[]> GetByteArrayAsync(string requestUri) {
			using (HttpResponseMessage resp = await GetAsync(requestUri, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false)) {
				resp.EnsureSuccessStatusCode();
				return await resp.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
			}
		}

		public async Task<byte[]> GetByteArrayAsync(Uri requestUri) {
			using (HttpResponseMessage resp = await GetAsync(requestUri, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false)) {
				resp.EnsureSuccessStatusCode();
				return await resp.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
			}
		}

		public async Task<Stream> GetStreamAsync(string requestUri) {
			HttpResponseMessage resp = await GetAsync(requestUri, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
			resp.EnsureSuccessStatusCode();
			return await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
		}

		public async Task<Stream> GetStreamAsync(Uri requestUri) {
			HttpResponseMessage resp = await GetAsync(requestUri, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
			resp.EnsureSuccessStatusCode();
			return await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
		}

		public async Task<string> GetStringAsync(string requestUri) {
			using (HttpResponseMessage resp = await GetAsync(requestUri, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false)) {
				resp.EnsureSuccessStatusCode();
				return await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
			}
		}

		public async Task<string> GetStringAsync(Uri requestUri) {
			using (HttpResponseMessage resp = await GetAsync(requestUri, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false)) {
				resp.EnsureSuccessStatusCode();
				return await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
			}
		}
	}
}
