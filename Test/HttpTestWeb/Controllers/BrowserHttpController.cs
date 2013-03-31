using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq;
using HttpTestLib;

namespace HttpTestWeb.Controllers {

	public class BrowserHttpController : ApiController {

		protected static readonly IList<TestEntity> Data = TestEntity.CreateSamples().ToList();

		public IEnumerable<TestEntity> Get() {
			return Data;
		}

		public int Post(TestEntity newEntity) {
			var id = Data.Max(e => e.Id) + 1;
			newEntity.Id = id;
			Data.Add(newEntity);
			return id;
		}

	}
}