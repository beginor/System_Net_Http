using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HttpTestLib;

namespace HttpTestWeb.Controllers {

	public class ClientHttpController : BrowserHttpController {

		public HttpResponseMessage Put(int id, TestEntity entity) {
			var data = Data.FirstOrDefault(d => d.Id == id);
			if (data == null) {
				return new HttpResponseMessage(HttpStatusCode.NoContent);
			}
			data.Name = entity.Name;
			data.Age = entity.Age;
			data.Birthday = entity.Birthday;
			return new HttpResponseMessage(HttpStatusCode.OK);
		}

		public HttpResponseMessage Delete(int id) {
			var data = Data.FirstOrDefault(d => d.Id == id);
			if (data != null) {
				Data.Remove(data);
			}
			return new HttpResponseMessage(HttpStatusCode.NoContent);
		}
	}
}