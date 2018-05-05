using System;
using System.Data.Entity.Migrations;
using System.Web.Http;
using Port.RestApi.Providers;
using Port.Entities.Entities;
using Port.Entities.Migrations;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(Port.RestApi.Startup))]

namespace Port.RestApi
{
    public class Startup

    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

        public static HttpConfiguration Config;
        public void Configuration(IAppBuilder app)
        {
            Config = new HttpConfiguration();

            ConfigureOAuth(app);
            WebApiConfig.Register(Config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            // nuget : Microsoft.AspNet.WebApi.OwinSelfHost
            app.UseWebApi(Config);
            
            Configuration configuration = new Configuration
            {
                ContextType = typeof(PortDbContext)
            };
            var migrator = new DbMigrator(configuration);
            migrator.Update();
        }
        public void ConfigureOAuth(IAppBuilder app)
        {
            app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

            OAuthAuthorizationServerOptions oAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new SimpleAuthorizationServerProvider()
            };
            app.UseOAuthAuthorizationServer(oAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);
            

        }
    }
}
