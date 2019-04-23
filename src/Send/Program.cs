using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace Send
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("start");
            IConnectionFactory connectionFactory = new ConnectionFactory
            {
                HostName = "127.0.0.1",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            using (IConnection connection = connectionFactory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    //交换机名称
                    string exchangeName = "exchange3";
                    //路由名称 
                    string routeKey = "routeKey.A";  //详细解释 https://www.sojson.com/blog/48.html
                    //声明交换机   路由交换机类型direct
                    channel.ExchangeDeclare(exchange: exchangeName, type: "topic");
                    while (true)
                    {
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        properties.MessageId = Guid.NewGuid().ToString("N");
                        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                        //properties.Headers = new Dictionary<string, object>();

                        Console.WriteLine("消息内容：");
                        string message = Console.ReadLine();
                        //消息内容
                        byte[] body = Encoding.UTF8.GetBytes(message);
                        //发送消息  发送到路由匹配的消息队列中
                        channel.BasicPublish(exchange: exchangeName, routingKey: routeKey, basicProperties: null, body: body);
                        Console.WriteLine("成功发送消息:" + message);
                    }
                }
            }
        }
    }
}