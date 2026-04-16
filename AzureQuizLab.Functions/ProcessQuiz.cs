using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureQuizLab.Functions;

public class ProcessQuiz
{
    private readonly ILogger<ProcessQuiz> _logger;
    private readonly IConfiguration _configuration;

    public ProcessQuiz(ILogger<ProcessQuiz> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [Function(nameof(ProcessQuiz))]
    public void Run([QueueTrigger("quiz-queue", Connection = "AzureWebJobsStorage")] QueueMessage message)
    {
        _logger.LogInformation("C# Queue trigger function processed: {messageText}", message.MessageText);

        var connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");

        var connectionStringBuiler = new SqlConnectionStringBuilder(connectionString);
        if (connectionStringBuiler.Password.Length == 0)
        {
            connectionStringBuiler.Password = _configuration["DbPassword"];
        }

        using (SqlConnection conn = new SqlConnection(connectionStringBuiler.ConnectionString))
        {
            conn.Open();
            var cmd = new SqlCommand("INSERT INTO Logs (Message, LogDate) VALUES (@msg, GETDATE())", conn);
            cmd.Parameters.AddWithValue("@msg", message.MessageText);
            cmd.ExecuteNonQuery();
        }
        _logger.LogInformation("Ecriture en base effectuée");
    }
}