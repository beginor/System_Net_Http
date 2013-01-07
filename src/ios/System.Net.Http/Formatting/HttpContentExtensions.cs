using System;
using System.Threading.Tasks;

namespace System.Net.Http.Formatting {

	public static class HttpContentExtensions {

		public static Task<T> ReadAsJsonAsync<T>(this HttpContent content) {
			return content.ReadAsAsync<T>(new JsonMediaTypeFormatter());
		}

		public static Task<T> ReadXmlAsAsync<T>(this HttpContent content) {
			return content.ReadAsAsync<T>(new XmlMediaTypeFormatter());
		}

		public static Task<T> ReadAsAsync<T>(this HttpContent content) {
			return content.ReadAsAsync<T>(GetFormatter(content.Headers.ContentType));
		}

		public static Task<T> ReadAsAsync<T>(this HttpContent content, MediaTypeFormatter formatter) {
			var tcs = new TaskCompletionSource<T>();
			try {
				var type = typeof(T);
				if (!formatter.CanReadType(type)) {
					tcs.TrySetException(new InvalidOperationException(string.Format("formatter {0} can not read {1}", formatter, type)));
				}
				else {
					var contentReadTask = content.ReadAsStreamAsync();
					contentReadTask.ContinueWith(t => {
						if (t.IsFaulted) {
							tcs.TrySetException(t.Exception.GetBaseException());
						}
						else {
							if (t.IsCanceled) {
								tcs.TrySetCanceled();
							}
							else {
								formatter.ReadFromStreamAsync(type, content.Headers, t.Result).ContinueWith(t2 => {
									if (t2.IsFaulted) {
										tcs.TrySetException(t2.Exception.GetBaseException());
									}
									else if (t2.IsCanceled) {
										tcs.TrySetCanceled();
									}
									else {
										tcs.TrySetResult((T)t2.Result);
									}
								});
							}
						}
					});
				}
			}
			catch (Exception ex) {
				tcs.TrySetException(ex);
			}
			return tcs.Task;
		}

		private static MediaTypeFormatter GetFormatter(string contentType) {
			if (contentType.IsNullOrEmpty()) {
				throw new ArgumentNullException("contentType");
			}
			var isXml = contentType.Contains(HttpMediaTypeNames.TextXml) || contentType.Contains(HttpMediaTypeNames.ApplicationXml);
			if (isXml) {
				return new XmlMediaTypeFormatter();
			}
			var isJson = contentType.Contains(HttpMediaTypeNames.TextJson) || contentType.Contains(HttpMediaTypeNames.ApplicationJson);
			if (isJson) {
				return new JsonMediaTypeFormatter();
			}
			throw new NotSupportedException(string.Format("Content type {0} is not supported!", contentType));
		}
	}
}