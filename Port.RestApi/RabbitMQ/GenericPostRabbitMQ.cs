using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Port.RestApi.RabbitMQ
{
    public class GenericPostRabbitMQ<T> where T :class
    {
        public T data;

        public GenericPostRabbitMQ(T _data)
        {
            this.data = _data;
        }
        public async Task< string> Post()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
             await  Task.Run(()=> channel.QueueDeclare(queue: data.GetType().Name,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null));

                var postData = JsonConvert.SerializeObject(data);
                var body = Encoding.UTF8.GetBytes(postData);

                await Task.Run(() => channel.BasicPublish(exchange: "",
                                     routingKey: data.GetType().Name,
                                     basicProperties: null,
                                     body: body));
                return $"[x] RabbitMQ Kuyruğuna  Gönderildi : {data.GetType().Name}";


            }
        }
    }
}