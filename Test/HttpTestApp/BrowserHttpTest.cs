﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttpTestApp {

	[TestClass]
	public class BrowserHttpTest : SilverlightTest {

		private readonly HttpClient _client = new HttpClient {
			BaseAddress = new Uri("http://localhost:8080/HttpTestWeb/api/")
		};

		[TestMethod]
		[Asynchronous]
		public void TestGet() {
			this._client.GetStringAsync("browserhttp/").ContinueWith(t => {
				this.EnqueueCallback(() => {
					if (t.IsFaulted) {
						Assert.Fail(t.Exception.GetBaseException().Message);
					}
					else {
						var json = t.Result;
						Assert.IsFalse(string.IsNullOrEmpty(json));
						Console.WriteLine(json);
					}
				});
				this.EnqueueTestComplete();
			});

		}

		[TestMethod]
		[Asynchronous]
		public void TestPost() {
			var param = new Dictionary<string, string> {
				{"Name", "Client Post"},
				{"Age", "1"},
				{"Birthday", DateTime.Today.ToString("s")}
			};
			this._client.PostAsync("browserhttp/", new FormUrlEncodedContent(param)).ContinueWith(t => {
				this.EnqueueCallback(() => {
					if (t.IsFaulted) {
						Assert.Fail(t.Exception.GetBaseException().Message);
					}
					else {
						var response = t.Result;
						var content = response.Content;
						var readContentTask = content.ReadAsStringAsync();
						readContentTask.Wait();
						var result = readContentTask.Result;
						Assert.IsFalse(string.IsNullOrEmpty(result));
						Console.WriteLine(result);
					}
				});
				this.EnqueueTestComplete();
			});
		}

		[TestMethod]
		[Asynchronous]
		public void TestSendAcceptHeader() {
			var request = new HttpRequestMessage(HttpMethod.Get, "browserhttp/");
			request.Headers.Accept.ParseAdd("application/xml");

			this._client.SendAsync(request).ContinueWith(task => {
				this.EnqueueCallback(() => {
					if (task.IsFaulted) {
						Assert.Fail(task.Exception.GetBaseException().ToString());
					}
					else {
						var response = task.Result;
						Assert.IsTrue(response.IsSuccessStatusCode);
						Assert.AreEqual("application/xml", response.Content.Headers.ContentType.MediaType);
					}
				});
				this.EnqueueTestComplete();
			});

		}

		[TestMethod]
		[Asynchronous]
		public void TestSendCustomHeader() {
			var request = new HttpRequestMessage(HttpMethod.Get, "browserhttp/");
			request.Headers.Accept.ParseAdd("application/xml");
			request.Headers.TryAddWithoutValidation("ClientType", "System.Net.Http.HttpClient");

			this._client.SendAsync(request).ContinueWith(task => {
				this.EnqueueCallback(() => {
					if (task.IsFaulted) {
						Assert.Fail(task.Exception.GetBaseException().ToString());
					}
					else {
						var response = task.Result;
						Assert.IsTrue(response.IsSuccessStatusCode);
						Assert.AreEqual("application/xml", response.Content.Headers.ContentType.MediaType);
					}
				});
				this.EnqueueTestComplete();
			});
		}

		[TestMethod]
		[Asynchronous]
		public void TestPut() {
			var param = new Dictionary<string, string> {
				{"Id", "1" },
				{"Name", "Client Post"},
				{"Age", "1"},
				{"Birthday", DateTime.Today.ToString("s")}
			};
			this._client.PutAsync("browserhttp/1", new FormUrlEncodedContent(param)).ContinueWith(t => {
				this.EnqueueCallback(() => {
					if (t.IsFaulted) {
						Assert.Fail(t.Exception.GetBaseException().Message);
					}
					else {
						var response = t.Result;
						Assert.IsTrue(response.IsSuccessStatusCode);
					}
				});
				this.EnqueueTestComplete();
			});
		}

		[TestMethod]
		[Asynchronous]
		public void TestDelete() {
			this._client.DeleteAsync("browserhttp/1").ContinueWith(t => {
				this.EnqueueCallback(() => {
					if (t.IsFaulted) {
						Assert.Fail(t.Exception.GetBaseException().Message);
					}
					else {
						var response = t.Result;
						Assert.IsTrue(response.IsSuccessStatusCode);
					}
				});
				this.EnqueueTestComplete();
			});
		}
	}
}