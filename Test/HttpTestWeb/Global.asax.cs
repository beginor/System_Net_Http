using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace HttpTestWeb {

	public class Global : System.Web.HttpApplication {

		protected void Application_Start(object sender, EventArgs e) {
			RegisterWebApiRoute(RouteTable.Routes);
		}

		private static void RegisterWebApiRoute(RouteCollection routeCollection) {
			routeCollection.MapHttpRoute(
				name:"DefaultApi",
				routeTemplate:"api/{controller}/{id}",
				defaults:new {id = RouteParameter.Optional}
				);
		}
	}
}