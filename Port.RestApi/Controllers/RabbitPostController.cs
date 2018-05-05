
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;


namespace Port.RestApi.Controllers
{
    using Port.Entities.Entities;
    using Port.Bussines.Base;
    using Port.RestApi.Models;
    using Port.RestApi.RabbitMQ;
    using Port.Entities.Tools;

    [RoutePrefix("api/PortalUserService")]
    public class RabbitPostController : ApiController
    {
        IEnumerable<CreateUserModel> createUser = new List<CreateUserModel>();
        public GenericRepository<CreateUserModel> GRepo { get; }

        GenericPostRabbitMQ<Port_User> rabbit;

        public RabbitPostController()
        {
            var conn = DbConnection.SqlConnectionString;
            GRepo = new GenericRepository<CreateUserModel>(conn);

        }

        [HttpPost]
        [Authorize]
        [ActionName("CreateUser")]
        public async Task<IHttpActionResult> AddOrUpdateCustomerOtherInformation(Port_User model)
        {

            //Respon için Guid farklı bir yerde üretilebilir fakat şimdilik buralar yazılı geçilecek.
            var portUserResponseGuidId = Guid.NewGuid();
            model.responseGuidId = portUserResponseGuidId;

            bool _isOk = false;
           

            rabbit = new GenericPostRabbitMQ<Port_User>(model);
            var isOk = await rabbit.Post();

            if (_isOk == false) { return Content(HttpStatusCode.BadRequest, HttpStatusCode.BadRequest.ToString()); }
            return Ok();

        }
    }
}