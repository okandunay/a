
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Port.RestApi.Controllers
{
    using Port.RestApi;
    using Port.RestApi.Models;
    using Port.Bussines.Base;
    using Port.Entities.Tools;   

    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        public IEnumerable<dynamic> dyList { get; private set; } = new List<dynamic>();
        IEnumerable<LoginUserModel> loginUser = new List<LoginUserModel>();
        public GenericRepository<LoginUserModel> GRepo { get; }

        protected AccountController()
        {
            var conn = DbConnection.SqlConnectionString;
            GRepo = new GenericRepository<LoginUserModel>(conn);
        }

        [HttpPost]
        [Authorize]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ActionName("Register")]
        public async Task<IHttpActionResult> Register(string email,string pass)
        {
            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var accessTokenResponse = GenerateLocalAccessTokenResponse(email);


            //https://stackoverflow.com/questions/29296778/how-to-generate-an-owin-bearer-token-during-registration-of-a-user
            //AuthenticationTicket ticket = new AuthenticationTicket(claimsIdentity, new AuthenticationProperties());
            //string token = Startup.OAuthServerOptions.AccessTokenFormat.Protect(ticket);


 return Ok(accessTokenResponse);
            
        }

        /// <summary>
        /// Laportal Login
        /// </summary>
        /// <param name="model"> kullanıcı mail ve şifre</param>
        /// <returns>Geriye Dönen Bilgi ve işaretciler </returns>
        /// access_token = Size özel token üretimi yapar, Login olduğunuz andan itibaren size belirtilen süre içerisinde izniniz olan servisleri kullanabilirsiniz,süre bittiğinde tekrar login olmalısınız
        /// token_type = Bearer, “The OAuth 2.0 Authorization Framework,”.Bu şartname, OAuth 2.0 korumalı kaynaklara erişmek için HTTP isteklerinde taşıyıcı simgelerinin nasıl kullanılacağını açıklar. RFC 6749=http://self-issued.info/docs/draft-ietf-oauth-v2-bearer.html#RFC6749
        /// expires_in = üretilen token için saniye cinsinden kalan zamanı temsil eder
        /// userName = Kullanıcı email adresiniz giriş işlemlerinde  mail adresi zorunludur
        /// userId=Kayıt işlemleri sırasında size özel üretilen Id numaranız. bazı servislerde parametre olarak kullanmanız için gereklidir.
        /// userRole= "", .
        /// userClient="userClient",
        /// .issued=Token başlangıç zamanı
        /// .expires=token bitiş zamanı
        [HttpPost]
        [AllowAnonymous]
        [ResponseType(typeof(LoginUserModel))]
        [ActionName("Login")]
        public async Task<HttpResponseMessage> LoginUser(LoginUserModel model)
        {
            //Parametreyi dapper e gönderme olayı sonraki konu olduğu için kısaca es geçiyorum.
            Dictionary<string, object> _params =
         new Dictionary<string, object>();

            _params.Add("@email", model.userName);
            _params.Add("@pass", model.password);


            dyList = await GRepo.QuerySp("GET_USER", _params);

            loginUser = dyList.Select(x => new LoginUserModel
            {
               userName=x.userName,password=x.password
               
            }).ToList();


            var request = HttpContext.Current.Request;
            var tokenServiceUrl = request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath + "/token";
            using (var client = new HttpClient())
            {
                var requestParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", model.userName),
                new KeyValuePair<string, string>("password", model.password)
            };
                var requestParamsFormUrlEncoded = new FormUrlEncodedContent(requestParams);
                var tokenServiceResponse = await client.PostAsync(tokenServiceUrl, requestParamsFormUrlEncoded);
                var responseString = await tokenServiceResponse.Content.ReadAsStringAsync();
                var responseCode = tokenServiceResponse.StatusCode;
                tokenServiceResponse.Headers.Add("userClient", "LaPortal");
                var responseMsg = new HttpResponseMessage(responseCode)
                {
                    Content = new StringContent(responseString, Encoding.UTF8, "application/json")
                };
                return responseMsg;
            }
        }

        private JObject GenerateLocalAccessTokenResponse(string userName)
        {

            var tokenExpiration = TimeSpan.FromDays(1);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim("role", "user"));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);

            JObject tokenResponse = new JObject(
                                        new JProperty("userName", userName),
                                        new JProperty("access_token", accessToken),
                                        new JProperty("token_type", "bearer"),
                                        new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
                                        new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                                        new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
        );

            return tokenResponse;
        }

    }
}