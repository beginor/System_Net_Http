using System.Collections.Generic;
using System.Web.Http;

namespace HttpTestWeb.Controllers {

	public class ClientHttpController : ApiController {

		// GET api/default
		public IEnumerable<string> Get() {
			return new string[] { "value1", "value2" };
		}

		// GET api/default/5
		public string Get(int id) {
			return "value";
		}

		// POST api/default
		public void Post([FromBody]Dictionary<string, string> value) {
			var str = value;
		}

		// PUT api/default/5
		public void Put(int id, [FromBody]string value) {
		}

		// DELETE api/default/5
		public void Delete(int id) {
		}
	}
}