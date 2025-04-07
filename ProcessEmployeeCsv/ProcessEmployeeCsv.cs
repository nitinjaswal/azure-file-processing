using System.Globalization;
using CsvHelper;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ProcessEmployeeCsv
{
    public class ProcessEmployeeCsv
    {
        private readonly ILogger<ProcessEmployeeCsv> _logger;
        private readonly IConfiguration _config;

        public ProcessEmployeeCsv(ILogger<ProcessEmployeeCsv> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [Function(nameof(ProcessEmployeeCsv))]
        public async Task Run(
        [BlobTrigger("uploads/{name}", Connection = "AzureWebJobsStorage")] Stream blobStream,
        string name,
        ILogger log)
        {
            _logger.LogInformation($"Triggered by uploaded file: {name}");
            var employees = new List<Employee>();

            try
            {
                using (var reader = new StreamReader(blobStream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    employees = new List<Employee>(csv.GetRecords<Employee>());
                }

                _logger.LogInformation($"CSV parsed");

                var connectionString = "Server=tcp:nj-sqlserver.database.windows.net,1433;Initial Catalog=FIleProcessingDb;Persist Security Info=False;User ID=nj-sqlserver;Password=Temp@1234!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";//_config["SqlConnection"];
                _logger.LogInformation("SQL Connection String: " + connectionString);
                using var conn = new SqlConnection(connectionString);
                await conn.OpenAsync();

                foreach (var emp in employees)
                {
                    var cmd = new SqlCommand(@"INSERT INTO Employees 
                    (EmployeeId, FirstName, LastName, Email, Department, JoiningDate) 
                    VALUES (@Id, @First, @Last, @Email, @Dept, @Join)", conn);

                    cmd.Parameters.AddWithValue("@Id", emp.EmployeeId);
                    cmd.Parameters.AddWithValue("@First", emp.FirstName);
                    cmd.Parameters.AddWithValue("@Last", emp.LastName);
                    cmd.Parameters.AddWithValue("@Email", emp.Email);
                    cmd.Parameters.AddWithValue("@Dept", emp.Department);
                    cmd.Parameters.AddWithValue("@Join", emp.JoiningDate);

                    await cmd.ExecuteNonQueryAsync();
                }

                await SendSummaryEmail(name, employees.Count);
                _logger.LogInformation($"✅ Processed {employees.Count} employees from {name}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error processing file {name}: {ex.Message}");
            }
        }
        private async Task SendSummaryEmail(string fileName, int count)
        {
            var apiKey = "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";//_config["SendGridApiKey"];
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress("nitinjas.chd@gmail.com", "File processed");
            var to = new EmailAddress("nitinjas.chd@gmail.com");
            var subject = "Employee Upload Summary";
            var plainText = $"Processed {count} records from file: {fileName}";
            var html = $"<strong>Processed {count}</strong> records from <em>{fileName}</em>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainText, html);
            await client.SendEmailAsync(msg);
        }
    }

    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public DateTime JoiningDate { get; set; }
    }
}
