using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace AzureQuizLab.Functions;

public class CleanupQuizData
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;

    public CleanupQuizData(ILoggerFactory loggerFactory, IConfiguration configuration)
    {
        _logger = loggerFactory.CreateLogger<CleanupQuizData>();
        _configuration = configuration;
    }

    [Function("CleanupQuizData")]
    public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);

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

            cmd.Parameters.AddWithValue("@msg", "C# Timer trigger function executed");
            cmd.ExecuteNonQuery();
        }

        _logger.LogInformation("Ecriture en base effectuée");
    }
}