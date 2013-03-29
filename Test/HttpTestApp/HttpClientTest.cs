using System;
using System.Collections.Generic;
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
				this.EnqueueCallback(() => {
					if (t.IsFaulted) {
						Assert.Fail(t.Exception.GetBaseException().Message);
					}
					else {
						var json = t.Result;
						Assert.IsFalse(string.IsNullOrEmpty(json));
					}
				});
				this.EnqueueTestComplete();
			});

		}

		[TestMethod]
		[Asynchronous]
		public void TestPost() {
			var httpClient = new HttpClient {
				BaseAddress = App.Current.Host.Source
			};
			var param = new Dictionary<string, string> {
				{"fName", "zhang"},
				{"lName", "zhimin"},
				{"value", "zhang zhimin"}
			};
			//var str = "hello,world";
			httpClient.PostAsync("/testhandler.ashx", new FormUrlEncodedContent(param)).ContinueWith(t => {
				this.EnqueueCallback(() => {
					if (t.IsFaulted) {
						Assert.Fail(t.Exception.GetBaseException().Message);
					}
					else {
						var response = t.Result;
						var content = response.Content.ReadAsStringAsync();
						content.Wait();
						var result = content.Result;
						Assert.IsFalse(string.IsNullOrEmpty(result));
					}
				});
				this.EnqueueTestComplete();
			});
		}
	}
}