using Port.Bussines.Base;
using Port.Entities.Entities;
using Port.Entities.Tools;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Port.RestApi.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public string UserRole { get; set; } = string.Empty;
        public IEnumerable<dynamic> Dy { get; private set; } = new List<dynamic>();

        public IEnumerable<Port_User> User { get; private set; } = new List<Port_User>();
        public GenericRepository<Port_User> GRepo { get; set; } 

        public SimpleAuthorizationServerProvider()
        {
            var conn = DbConnection.SqlConnectionString;
            GRepo = new GenericRepository<Port_User>(conn);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {

            context.OwinContext.Set("as:clientAllowedOrigin", "");
            context.OwinContext.Set("as:clientRefreshTokenLifeTime","");

            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin") ?? "*";
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var param = new Dictionary<string, object>
            {
                { "@email", context.UserName },
                { "@pass", context.Password }
            };

            Dy = await GRepo.QuerySp("GET_USER", param);

            User = Dy.Select(x => new Port_User
            {
                pu_Id = x.pu_Id,
                pu_FirstName = x.pu_FirstName,
                pu_LastName = x.pu_LastName,
                pu_Email = x.pu_Email,
                pu_Pasaword = x.pu_Pasaword,
                pu_BirthDate = x.pu_BirthDate
            }).ToList();

            
                if (User == null)
                {
                    context.SetError("invalid_grant", "Kullanıcı adı veya parola doğru değil.");
                try
                {
                    var client = new MongoClient("mongodb://localhost:27017");
                    var db = client.GetDatabase("LogDb");
                    var command = new BsonDocumentCommand<BsonDocument>(new BsonDocument{{"collstats", "LogTable" } });
                    var stats = await client.GetDatabase("LogDb").RunCommandAsync(command);
                    var statsControl= stats["capped"].AsBoolean;
                    if (statsControl)
                    {
                        await db.CreateCollectionAsync("LogTable");

                    }
                    var collection = db.GetCollection<PortLog>("LogTable");
                    var documnt = new PortLog { Id=3,Name="log" };
                    await collection.InsertOneAsync(documnt);

                    //var filter = new BsonDocument("Id", "1");
                    //var list = await collection.Find(filter).ToListAsync();
                }
                catch (Exception ex)
                {

                    ex.ToBson();
                    return;
                }
                return;
            }

            // ReSharper disable once PossibleNullReferenceException
            var userId = User.FirstOrDefault().pu_Id.ToString();

            //tüm kullanıcılar için claim' lar oluşturuluyor
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, UserRole));
            identity.AddClaim(new Claim(ClaimTypes.Sid, userId));

            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    { "userName", context.UserName },
                    {"userId",User.FirstOrDefault()?.pu_Id.ToString() },
                    {"userRole",UserRole}
                });
            
            // token üretimi
            var ticket = new AuthenticationTicket(identity, props);
            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
            context.Validated(ticket);
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "yenilenen clientId orjinal  clientId den farklı");
                return Task.FromResult<object>(null);
            }

            // yeni token
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);

            var newClaim = newIdentity.Claims.FirstOrDefault(c => c.Type == "newClaim");
            if (newClaim != null)
            {
                newIdentity.RemoveClaim(newClaim);
            }
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
    }
}