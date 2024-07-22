using Azure.Messaging.ServiceBus;
using Models;
using Newtonsoft.Json;
using Services;
using Services.Utils;

namespace MessageConsumer
{
    internal class Program
    {
        static readonly string connectionString = "";
        static readonly string queueName = "peopleQueue";

        static readonly PersonService service = new PersonService(new MongoDataBaseSettings()
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "PeopleDb",
            PersonCollectionName = "People"
        });

        static async Task Main(string[] args)
        {
            var client = new ServiceBusClient(connectionString);
            var processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());


            processor.ProcessMessageAsync += PersonMessageHander;
            processor.ProcessErrorAsync += ErrorHandler;

            Console.CancelKeyPress += async (s, e) =>
            {
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            };

            await processor.StartProcessingAsync();

            Console.WriteLine("Wait for a signal to stop receiving messages");
        }

        static async Task PersonMessageHander(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine("Message received: " + body);


            var person = JsonConvert.DeserializeObject<Person>(body);

            await service.CreatePersonAsync(person);

            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
