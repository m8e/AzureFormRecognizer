using System;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace funcformrecognizer
{
    public static class formrecogniblobtrigger
    {
        [FunctionName("formrecogniblobtrigger")]
        public static async Task Run([BlobTrigger("forms/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob, 
            [CosmosDB(
              databaseName: "dbformrecognizer",
              collectionName: "dbformcontainer",
              ConnectionStringSetting = "CosmosDBConnection", CreateIfNotExists = true)] IAsyncCollector<object> analysisresults
          , ILogger log)
        {


                try
                {
                    string endpoint = "https://formdeploymentdemo022.cognitiveservices.azure.com/";
                    string apiKey = "2c15beb8e33f45a08704b6f159a21b3b";
                    var credential = new AzureKeyCredential(apiKey);
                    var client = new DocumentAnalysisClient(new Uri(endpoint), credential);
                    //Uri fileUri = new Uri("<fileUri>");

                    AnalyzeDocumentOperation operation = await client.StartAnalyzeDocumentAsync("prebuilt-document", myBlob);

                    await operation.WaitForCompletionAsync();

                    AnalyzeResult result = operation.Value;
                    await analysisresults.AddAsync(result);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    } 