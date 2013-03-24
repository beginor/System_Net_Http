using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttpTestApp {

	[TestClass]
	public class HttpClientTest : SilverlightTest {

		[TestMethod, Asynchronous]
		public void TestGet() {
			var httpClient = new HttpClient {
				BaseAddress = new Uri("http://agserver.gdepb.gov.cn/")
			};
			httpClient.GetStringAsync("ArcGIS/rest/services/?f=json&pretty=true").ContinueWith(t => {
				if (t.IsFaulted) {
					Assert.Fail(t.Exception.GetBaseException().Message);
					this.TestComplete();
				}
				else {
					var json = t.Result;
					Assert.IsFalse(string.IsNullOrEmpty(json));
					this.TestComplete();
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());
			
		}
	}
}