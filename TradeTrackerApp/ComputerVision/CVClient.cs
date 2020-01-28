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

        public async Task<IEnumerable<TextRecognitionResult>> ReadBitmapAsync(Bitmap bitmap, CancellationToken token)
        {

            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                // Read text from URL
                RecognizeTextInStreamHeaders textHeaders = await Client.RecognizeTextInStreamAsync(stream, TextRecognitionMode.Printed, token);
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
                // Display the found text.
                Console.WriteLine();
                return results.RecognitionResults;
            }

        }
    }

}
