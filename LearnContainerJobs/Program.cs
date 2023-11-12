// See https://aka.ms/new-console-template for more information
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

Stopwatch stopwatch = Stopwatch.StartNew();

var factory = LoggerFactory.Create(builder => {
    builder.AddConsole();
});

var logger = factory.CreateLogger<Program>();

logger.LogInformation("Executing Job");

string? connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
string? queueName = Environment.GetEnvironmentVariable("AZURE_STORAGE_QUEUE_NAME");

//string? connectionString = "UseDevelopmentStorage=true";
//string? queueName = "test";

var queueClient = new QueueClient(connectionString, queueName);

QueueMessage[] messages = await queueClient.ReceiveMessagesAsync(maxMessages: 1, visibilityTimeout: TimeSpan.FromSeconds(60));

if (messages.Length == 0)
{
    Console.WriteLine("No message recieved. Exiting....");
    return;
}

foreach(var message in messages)
{
    Console.WriteLine($"Message: {message.MessageText}");

    await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
}

logger.LogInformation($"Completed Queuw Job - {stopwatch.Elapsed}");
logger.LogInformation($"Waiting for 1mins before exiting");
await Task.Delay(TimeSpan.FromSeconds(60));

logger.LogInformation("Completed Job");
