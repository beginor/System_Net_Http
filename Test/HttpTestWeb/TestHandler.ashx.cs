using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HttpTestWeb {
	/// <summary>
	/// Summary description for TestHandler
	/// </summary>
	public class TestHandler : IHttpHandler {

		public void ProcessRequest(HttpContext context) {
			context.Response.ContentType = "text/plain";
			context.Response.Write("Request Method: " + context.Request.HttpMethod);
		}

		public bool IsReusable {
			get {
				return false;
			}
		}
	}
}