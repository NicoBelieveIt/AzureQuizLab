using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureQuizLab.Functions;

public class ProcessQuiz
{
    private readonly ILogger<ProcessQuiz> _logger;

    public ProcessQuiz(ILogger<ProcessQuiz> logger)
    {
        _logger = logger;
    }

    [Function(nameof(ProcessQuiz))]
    public void Run([QueueTrigger("quiz-queue", Connection = "AzureWebJobsStorage")] QueueMessage message)
    {
        _logger.LogInformation("C# Queue trigger function processed: {messageText}", message.MessageText);
    }
}