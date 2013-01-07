using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http.Formatting {

	public static class HttpClientExtensions {

		public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string url, T obj) {
			return client.PostAsJsonAsync(url, obj, CancellationToken.None);
		}

		public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string url, T obj, CancellationToken cancellation) {
			return client.PostAsync(url, obj, new JsonMediaTypeFormatter(), cancellation);
		}

		public static Task<HttpResponseMessage> PostAsXmlAsync<T>(this HttpClient client, string url, T obj) {
			return client.PostAsXmlAsync(url, obj, CancellationToken.None);
		}

		public static Task<HttpResponseMessage> PostAsXmlAsync<T>(this HttpClient client, string url, T obj, CancellationToken cancellation) {
			return client.PostAsync(url, obj, new XmlMediaTypeFormatter(), cancellation);
		}

		public static Task<HttpResponseMessage> PostAsync<T>(this HttpClient httpClient, string url, T obj, MediaTypeFormatter formatter) {
			return httpClient.PostAsync(url, obj, formatter, CancellationToken.None);
		}

		public static Task<HttpResponseMessage> PostAsync<T>(this HttpClient httpClient, string url, T obj, MediaTypeFormatter formatter, CancellationToken cancellation) {
			return httpClient.PostAsync(url, obj, formatter, formatter.SupportedMediaTypes.First(), cancellation);
		}

		public static Task<HttpResponseMessage> PostAsync<T>(this HttpClient httpClient, string url, T obj, MediaTypeFormatter formatter, string mediaType) {
			return httpClient.PostAsync(url, obj, formatter, mediaType, CancellationToken.None);
		}
		
		public static Task<HttpResponseMessage> PostAsync<T>(this HttpClient httpClient, string url, T obj, MediaTypeFormatter formatter, string mediaType, CancellationToken cancellation) {
			return httpClient.PostAsync(url, new ObjectContent<T>(obj, formatter, mediaType), cancellation);
		}

		public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient client, string url, T obj) {
			return client.PutAsJsonAsync(url, obj, CancellationToken.None);
		}

		public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient client, string url, T obj, CancellationToken cancellation) {
			return client.PutAsync(url, obj, new JsonMediaTypeFormatter(), cancellation);
		}

		public static Task<HttpResponseMessage> PutAsXmlAsync<T>(this HttpClient client, string url, T obj) {
			return client.PutAsXmlAsync(url, obj, CancellationToken.None);
		}

		public static Task<HttpResponseMessage> PutAsXmlAsync<T>(this HttpClient client, string url, T obj, CancellationToken cancellation) {
			return client.PutAsync(url, obj, new XmlMediaTypeFormatter(), cancellation);
		}

		public static Task<HttpResponseMessage> PutAsync<T>(this HttpClient httpClient, string url, T obj, MediaTypeFormatter formatter) {
			return httpClient.PutAsync(url, obj, formatter, CancellationToken.None);
		}

		public static Task<HttpResponseMessage> PutAsync<T>(this HttpClient httpClient, string url, T obj, MediaTypeFormatter formatter, CancellationToken cancellation) {
			return httpClient.PutAsync(url, obj, formatter, formatter.SupportedMediaTypes.First(), cancellation);
		}

		public static Task<HttpResponseMessage> PutAsync<T>(this HttpClient httpClient, string url, T obj, MediaTypeFormatter formatter, string mediaType) {
			return httpClient.PutAsync(url, obj, formatter, mediaType, CancellationToken.None);
		}
		
		public static Task<HttpResponseMessage> PutAsync<T>(this HttpClient httpClient, string url, T obj, MediaTypeFormatter formatter, string mediaType, CancellationToken cancellation) {
			return httpClient.PutAsync(url, new ObjectContent<T>(obj, formatter, mediaType), cancellation);
		}
	}
}