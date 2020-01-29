using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCTradeTracker.ComputerVision
{
    public class CVClient
    {

        private const string EXTRACT_TEXT_URL_IMAGE = "https://i.redd.it/ss2s8k9fjb101.jpg";

        // Add your Computer Vision subscription key and endpoint to your environment variables.
        private static string subscriptionKey = Environment.GetEnvironmentVariable("COMPUTER_VISION_SUBSCRIPTION_KEY");

        private static string endpoint = Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT");
        
        private ComputerVisionClient Client { get; }
        public CVClient()
        {
            Client = Authenticate(endpoint, subscriptionKey);
        }

        private ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
                new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
                { Endpoint = endpoint };
            return client;
        }

        public async Task<IEnumerable<TextRecognitionResult>> ReadBitmapAsync(string fileName, CancellationToken token)
        {

            // Read text from URL
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                BatchReadFileInStreamHeaders textHeaders = await Client.BatchReadFileInStreamAsync(fs, token);
                // After the request, get the operation location (operation ID)
                string operationLocation = textHeaders.OperationLocation;
                // Retrieve the URI where the recognized text will be stored from the Operation-Location header.
                // We only need the ID and not the full URL
                const int numberOfCharsInOperationId = 36;
                string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);
                // Extract the text
                // Delay is between iterations and tries a maximum of 10 times.
                int i = 0;
                int maxRetries = 10;
                ReadOperationResult results;
                Console.WriteLine();
                do
                {
                    results = await Client.GetReadOperationResultAsync(operationId);
                    await Task.Delay(1000);
                    if (i == 9)
                    {
                        Console.WriteLine("Server timed out.");
                    }
                }
                while ((results.Status == TextOperationStatusCodes.Running ||
                    results.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries);
                return results.RecognitionResults;
            }
        }

    }

}
