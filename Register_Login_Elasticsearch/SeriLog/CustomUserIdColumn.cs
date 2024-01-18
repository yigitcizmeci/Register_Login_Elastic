using Register_Login_Elasticsearch.Models;
using Register_Login_Elasticsearch.Repositories;
using Serilog.Core;
using Serilog.Events;

namespace Register_Login_Elasticsearch.SeriLog
{
    public class CustomUserIdColumn : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            int userId = GetUserIdFromLogEvent(logEvent);
            var userIdProperty = new LogEventProperty("UserId", new ScalarValue(userId));
            logEvent.AddOrUpdateProperty(userIdProperty);
        }

        private int GetUserIdFromLogEvent(LogEvent logEvent)
        {
            if (logEvent.Properties is not null)
            {
                var (userIdKey, value) = logEvent.Properties.FirstOrDefault(x => x.Key == "UserId");
                if (value is not null && value is ScalarValue scalarValue && scalarValue.Value is int userId)
                {
                    return userId;
                }
            }

            return -1;
        }

    }
}
