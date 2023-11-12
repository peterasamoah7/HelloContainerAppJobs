// See https://aka.ms/new-console-template for more information
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

Console.WriteLine("Executing Job");

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

await Task.Delay(TimeSpan.FromSeconds(60));

Console.WriteLine("Completed Job");
