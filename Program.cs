using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue; 

namespace QueueApp
{
    class Program
    {
        private const string connectionString = "DefaultEndpointsProtocol=https;EndpointSuffix=core.windows.net;AccountName=cloudshell2107777753;AccountKey=DE/AOHkeikb1tpMH4g7ufHxYY7vF7F+nMB89e/tE0j7jqMRj2HZC7FBlBsNIBRLJvml/iHL1qEzas5eTzPWyjg==";
        
        static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                string value = String.Join(" ", args);
                await SendArticleAsync(value);
                Console.WriteLine($"Sent async message: {value}");
            }
            else
            {
                string value = await ReceiveArticleAsync();
                Console.WriteLine($"Received {value}");
            }
        }

        static async Task<CloudQueue> GetQueue()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            CloudQueue queue = queueClient.GetQueueReference("newsqueue");
            bool createdQueue = await queue.CreateIfNotExistsAsync();
            if (createdQueue)
            {
                Console.WriteLine("The queue of news articles was created.");
            }
            return queue;

        }

        static async Task SendArticleAsync(string newsMessage)
        {
            CloudQueue queue = await GetQueue();   

            CloudQueueMessage articleMessage = new CloudQueueMessage(newsMessage);
            await queue.AddMessageAsync(articleMessage);
        }

        static async Task<string> ReceiveArticleAsync()
        {
            CloudQueue queue = await GetQueue();
            bool exists = await queue.ExistsAsync();
            if (!exists)
            {
                return "<queue empty or not created>";
            }
            var cloudMsg = await queue.GetMessageAsync();            

            if(!(cloudMsg==null))
            {
                var msg = cloudMsg.AsString;
                Console.WriteLine($"Retrieved Message {msg}");
                return msg;
                
                await queue.DeleteMessageAsync(cloudMsg);
                Console.WriteLine($"Deleted message");

            }
            return null;
            
        }


    }
}
