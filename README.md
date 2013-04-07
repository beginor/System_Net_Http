## System.Net.Http for Silverlight

Silverlight 版本的 System.Net.Http ， 源自 Mono 的源代码。 使用方法参考 [http://msdn.microsoft.com/library/system.net.http.aspx](http://msdn.microsoft.com/library/system.net.http.aspx)

[由于 Silverlight 平台对 HTTP 的限制](http://msdn.microsoft.com/library/cc838250.aspx)， 移除了部分功能， 例如 Proxy 、 AllowAutoRedirect 、 PreAuthenticate 以及 KeepAlive 设置等， 这些都是 Silverlight 不支持的。

对于 Silverlight 的 BrowserHttp ， 仅仅支持 GET 和 POST 方法， 示例代码如下：

```c#
HttpClient client = new HttpClient {
   BaseAddress = new Uri("http://localhost:8080/HttpTestWeb/api/")
};

// Get string from server
client.GetStringAsync("browserhttp/").ContinueWith(t => {
   if (t.IsFaulted) {
      // report error here
      //Application.Current.ReportError(t.Exception.GetBaseException());
   } else {
      string txt = t.Result;
      //Assert.IsFalse(string.IsNullOrEmpty(txt));
   }
});

// Post form data to server
var param = new Dictionary<string, string> {
   {"Name", "Client Post"},
   {"Age", "1"},
   {"Birthday", DateTime.Today.ToString("s")}
};
client.PostAsync("browserhttp/", new FormUrlEncodedContent(param)).ContinueWith(t => {
   if (t.IsFaulted) {
      // report error here
      // Application.Current.ReportError(t.Exception.GetBaseException());
   } else {
      HttpResponseMessage response = t.Result;
      //Assert.IsTrue(response.EnsureSuccessStatusCode);
   }
});
```

对于 ClientHttp ， 除了 GET 和 POST 之外， 还支持 PUT 和 DELETE （其它的 HTTP 方法也可能支持， 未测试）， 示例代码如下：

```c#
// PUT to update
var param = new Dictionary<string, string> {
   {"Id", "1" },
   {"Name", "Client Post"},
   {"Age", "1"},
   {"Birthday", DateTime.Today.ToString("s")}
};
client.PutAsync("clienthttp/1", new FormUrlEncodedContent(param)).ContinueWith(t => {
   if (t.IsFaulted) {
      // report error here
      // Application.Current.ReportError(t.Exception.GetBaseException());
   } else {
      HttpResponseMessage response = t.Result;
      //Assert.IsTrue(response.EnsureSuccessStatusCode);
   }
});

// DELETE
client.DeleteAsync("clienthttp/1").ContinueWith(t => {
   if (t.IsFaulted) {
      // report error here
      // Application.Current.ReportError(t.Exception.GetBaseException());
   } else {
      HttpResponseMessage response = t.Result;
      //Assert.IsTrue(response.EnsureSuccessStatusCode);
   }
});
```

支持职责链模式的 MessageProcessingHandler ， 如下面的代码所示：

```c#
public class CustomProcessingHandler : MessageProcessingHandler {

   protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken) {
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
```

使用起来也是非常简单的：

```c#
var customHandler = new CustomProcessingHandler {
	InnerHandler = new HttpClientHandler()
};
var client = new HttpClient(customHandler, true) {
	BaseAddress = new Uri("http://localhost:8080/HttpTestWeb/api/")
};
```