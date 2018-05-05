using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Port.Entities.Entities.Enums;
using Port.Entities.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;



namespace Port.RabbitMqListener
{
    class Program
    {
        static Port_User createUser;

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var userModelChannel = connection.CreateModel())
            {
                createUser = new Port_User();
                userModelChannel.QueueDeclare(queue: "Port_User",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var userConsumer = new EventingBasicConsumer(userModelChannel);
                userConsumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var data = Encoding.UTF8.GetString(body);
                    createUser = JsonConvert.DeserializeObject<Port_User>(data);
                    Console.WriteLine(" [x] Alınan {0}", createUser.pu_FirstName + " : " + createUser.ToString());
          
                    // ilk log queue ye alındı
                    createUser.status = ResponseResultEnums.QUE;
                    var insertStatus = ConnetcAndProccessStart(createUser, "Insert");
                    if (insertStatus.Result)
                    {
                        var updateStatus = ConnetcAndProccessStart(createUser, "Update");

                        if (updateStatus.Result)
                        {
                            // gelen user için işlem tamamlandı.
                            createUser.status = ResponseResultEnums.CMP;
                        }
                        else {
                            //hata oluştu
                            createUser.status = ResponseResultEnums.ERR;
                        }                       
                    }
                    else
                    {
                        //hata oluştu
                        createUser.status = ResponseResultEnums.ERR;
                    }
                };
                userModelChannel.BasicConsume(queue: "Port_User",
                                     autoAck: true,
                                     consumer: userConsumer);

                
                Console.WriteLine(" Çıkmak [enter] için basın.");
                Console.ReadLine();
            }
        }

        static async Task<bool> ConnetcAndProccessStart(Port_User user, string proccess)
        {
            createUser = new Port_User();
            bool isOk = false;

            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("LogDb");
            var command = new BsonDocumentCommand<BsonDocument>(new BsonDocument { { "collstats", "UserLogTable" } });
            var stats = await client.GetDatabase("LogDb").RunCommandAsync(command);
            var statsControl = stats["capped"].AsBoolean;
            if (statsControl)
            {
                await db.CreateCollectionAsync("UserLogTable");

            }
            var collection = db.GetCollection<Port_User>("UserLogTable");
            try
            {
                if (proccess == "Insert")
                {
                    await collection.InsertOneAsync(user);
                }

                if (proccess == "Update")
                {
                    var builder = Builders<Port_User>.Filter;
                    var filter = builder.Or(
                    builder.Where(r => r.pu_Id == createUser.pu_Id),
                    builder.OfType<Port_User>(s => s.pu_Id == createUser.pu_Id));
                    var cursor = collection.FindSync(filter);
                    var updates = new List<UpdateDefinition<Port_User>>();
                    await collection.FindOneAndUpdateAsync(filter, Builders<Port_User>.Update.Combine(updates), new FindOneAndUpdateOptions<Port_User>
                    {
                        ReturnDocument = ReturnDocument.After,
                    });

                }


                isOk = true;
            }
            catch (Exception)
            {

                isOk = false;
            }


            return isOk;
        }
    }
}
