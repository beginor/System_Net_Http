using System;
using System.Threading.Tasks;
using System.Threading;

namespace System.Net.Http {

	public class HttpClient : HttpMessageInvoker {

		Uri _baseAddress;

		public Uri BaseAddress {
			get {
				return this._baseAddress;
			}
			set {
				if (!value.IsAbsoluteUri) {
					throw new InvalidOperationException("BaseAddress must be a absolute uri!");
				}
				this._baseAddress = value;
			}
		}

		public HttpClient() : this(new HttpClientHandler()) {
		}

		public HttpClient(HttpMessageHandler handler) : this(handler, true) {
		}

		public HttpClient(HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler) {
		}

		public Task<HttpResponseMessage> GetAsync(string url) {
			return this.GetAsync(url, CancellationToken.None);
		}

		public Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellation) {
			var request = new HttpRequestMessage(HttpMethods.Get, this.PrepareUri(url));
			return this.SendAsync(request, cancellation);
		}

		public Task<HttpResponseMessage> PostAsync(string url, HttpContent content) {
			return this.PostAsync(url, content, CancellationToken.None);
		}

		public Task<HttpResponseMessage> PostAsync(string url, HttpContent content, CancellationToken cancellation) {
			var request = new HttpRequestMessage(HttpMethods.Post, this.PrepareUri(url)) {
				Content = content
			};
			return this.SendAsync(request, cancellation);
		}

		public Task<HttpResponseMessage> PutAsync(string url, HttpContent content) {
			return this.PutAsync(url, content, CancellationToken.None);
		}

		public Task<HttpResponseMessage> PutAsync(string url, HttpContent content, CancellationToken cancellation) {
			var request = new HttpRequestMessage(HttpMethods.Put, this.PrepareUri(url)) {
				Content = content
			};
			return this.SendAsync(request, cancellation);
		}

		public Task<HttpResponseMessage> DeleteAsync(string url) {
			return this.DeleteAsync(url, CancellationToken.None);
		}

		public Task<HttpResponseMessage> DeleteAsync(string url, CancellationToken cancellation) {
			return this.SendAsync(new HttpRequestMessage(HttpMethods.Delete, this.PrepareUri(url)), cancellation);
		}

		private Uri PrepareUri(string url) {
			if (this.BaseAddress == null) {
				return new Uri(url, UriKind.Absolute);
			}
			return new Uri(this.BaseAddress, url);
		}
	}
}