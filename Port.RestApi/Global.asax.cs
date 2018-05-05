using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Port.RestApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {

                this.Context.Response.AddHeader("Access-Control-Allow-Headers", "accept,origin,authorization,content-type");
            
        }
    }
}
