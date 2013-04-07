using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttpTestApp {

	[TestClass]
	public class MessageProcessingHandlerTest : SilverlightTest {

		[TestMethod, Asynchronous]
		public void Test1() {
			var customHandler = new CustomProcessingHandler {
				InnerHandler = new HttpClientHandler()
			};
			var client = new HttpClient(customHandler, true) {
				BaseAddress = new Uri("http://localhost:8080/HttpTestWeb/api/")
			};
		}
	}

	public class CustomProcessingHandler : MessageProcessingHandler {

		protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken) {
			request.Headers.Accept.ParseAdd("application/xml");
			request.Headers.TryAddWithoutValidation("ClientType", "System.Net.Http.HttpClient");
			if (request.Method != HttpMethod.Get && request.Method != HttpMethod.Post) {
				request.Headers.TryAddWithoutValidation("RequestMethod", request.Method.Method);
				request.Method = HttpMethod.Post;
			}
			return request;
		}

		protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken) {
			var request = response.RequestMessage;
			if (request.Headers.Contains("RequestMethod")) {
				IEnumerable<string> values;
				if (request.Headers.TryGetValues("RequestMethod", out values)) {
					request.Method = new HttpMethod(values.First());
				}
			}
			return response;
		}
	}
}