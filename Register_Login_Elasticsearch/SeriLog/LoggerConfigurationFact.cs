using System.Collections.ObjectModel;
using System.Data;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.MSSqlServer;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;
using SqlColumn = Serilog.Sinks.MSSqlServer.SqlColumn;

namespace Register_Login_Elasticsearch.SeriLog
{
    public static class LoggerConfigurationFact
    {
        public static Logger CreateLogger(IConfiguration configuration)
        {
            try
            {
                var customColumnOptions = CreateCustomColumnOptions();
                var columnOptions = new ColumnOptions
                {
                    Level = { ColumnName = "Level", DataType = SqlDbType.NVarChar, AllowNull = false, DataLength = 128 },
                    TimeStamp = { ColumnName = "TimeStamp", DataType = SqlDbType.DateTimeOffset, AllowNull = false },
                    LogEvent = { ColumnName = "LogEvent", DataType = SqlDbType.NVarChar, AllowNull = true, DataLength = 4000 },
                    Message = { ColumnName = "Message", DataType = SqlDbType.NVarChar, AllowNull = true, DataLength = 4000 },
                    Exception = { ColumnName = "Exception", DataType = SqlDbType.NVarChar, AllowNull = true, DataLength = 4000 },
                    MessageTemplate = { ColumnName = "MessageTemplate", DataType = SqlDbType.NVarChar, AllowNull = true, DataLength = 4000 },
                    AdditionalColumns = customColumnOptions.AdditionalColumns
                };
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                var conf = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env}.json", optional: false)
                    .Build();

                columnOptions.Store.Remove(StandardColumn.Properties);
                columnOptions.Store.Add(StandardColumn.LogEvent);
                return new LoggerConfiguration()
                     .WriteTo.File("Logs/log.txt")
                        .WriteTo.MSSqlServer(configuration.GetConnectionString("sqlConnection"), sinkOptions: new MSSqlServerSinkOptions
                        {
                            AutoCreateSqlTable = true,
                            TableName = "Logs"
                        },
                columnOptions: columnOptions)
                .Enrich.FromLogContext()
                .Enrich.With(new CustomUserIdColumn())
                .Enrich.WithExceptionDetails()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(ConfigureElasticSink(conf, env))
                .Enrich.WithProperty("Environment", env)
                .ReadFrom.Configuration(conf)
                .CreateLogger();

                ElasticsearchSinkOptions ConfigureElasticSink(IConfiguration configuration, string env)
                {
                    return new ElasticsearchSinkOptions(new Uri(configuration["Elastic:Uri"]!))
                    {
                        AutoRegisterTemplate = true,
                        IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{env.ToLower()}-{DateTime.UtcNow:MM-yyyy}",
                        NumberOfReplicas = 1,
                        NumberOfShards = 2,
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating logger.", ex);
            }
        }
        private static ColumnOptions CreateCustomColumnOptions()
        {
            var customColumnOptions = new ColumnOptions
            {
                AdditionalColumns = new Collection<SqlColumn>
        {
         new SqlColumn
         {
             ColumnName = "UserId",
             DataType = SqlDbType.NVarChar,
             AllowNull = false,
         }
     }
            };
            return customColumnOptions;
        }
    }
}
