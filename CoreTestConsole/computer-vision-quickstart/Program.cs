using System;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace computer_vision_quickstart
{
    class Program
    {

        // URL image for OCR (optical character recognition). (Image of motivational meme).
        private const string EXTRACT_TEXT_URL_IMAGE = "https://i.redd.it/ss2s8k9fjb101.jpg";

       static void Main(string[] args)
        {
            var client = Authenticate(Connection.Endpoint, Connection.Key);
            // Read the batch text from an image (handwriting and/or printed).
            BatchReadFileUrl(client, EXTRACT_TEXT_URL_IMAGE).Wait();
            //BatchReadFileLocal(client, EXTRACT_TEXT_LOCAL_IMAGE).Wait();
        }

        /*
        * BATCH READ FILE - URL IMAGE
        * Recognizes handwritten text. 
        * This API call offers an improvement of results over the Recognize Text calls.
        */
        public static async Task BatchReadFileUrl(ComputerVisionClient client, string urlImage)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("BATCH READ FILE - URL IMAGE");
            Console.WriteLine();

            // Read text from URL
            BatchReadFileHeaders textHeaders = await client.BatchReadFileAsync(urlImage);
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
            Console.WriteLine($"Extracting text from URL image {Path.GetFileName(urlImage)}...");
            Console.WriteLine();
            do
            {
                results = await client.GetReadOperationResultAsync(operationId);
                Console.WriteLine("Server status: {0}, waiting {1} seconds...", results.Status, i);
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
            var textRecognitionLocalFileResults = results.RecognitionResults;
            foreach (TextRecognitionResult recResult in textRecognitionLocalFileResults)
            {
                foreach (Line line in recResult.Lines)
                {
                    Console.WriteLine(line.Text);
                }
            }
            Console.WriteLine();
        }

        /*
        * AUTHENTICATE
        * Creates a Computer Vision client used by each example.
        */
        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
            new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
            { Endpoint = endpoint };
            return client;
        }

    }
}
