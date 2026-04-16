using AzureQuizLab.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AzureQuizLab
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            var connectionStringBuiler = new SqlConnectionStringBuilder(builder.Configuration.GetConnectionString("DefaultConnection"));
            if(connectionStringBuiler.Password.Length == 0)
            {
                connectionStringBuiler.Password = builder.Configuration["DbPassword"];
            }
            builder.Services.AddDbContext<QuizDbContext>(options =>
                options.UseSqlServer(
                    connectionStringBuiler.ConnectionString,
                    sqlOptions => sqlOptions.EnableRetryOnFailure()
                ));

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddAzureWebAppDiagnostics();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
