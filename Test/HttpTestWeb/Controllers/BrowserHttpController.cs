using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Linq;
using HttpTestLib;

namespace HttpTestWeb.Controllers {

	public class BrowserHttpController : ApiController {

		private static IList<TestEntity> _data = TestEntity.CreateSamples();

		public IEnumerable<TestEntity> Get() {
			return _data;
		}

		public int Post(TestEntity newEntity) {
			var id = _data.Max(e => e.Id) + 1;
			newEntity.Id = id;
			_data.Add(newEntity);
			return id;
		}
	}
}