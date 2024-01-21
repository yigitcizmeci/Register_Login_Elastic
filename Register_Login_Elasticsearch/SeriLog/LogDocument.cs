using Nest;
using Serilog.Events;
using Serilog;
using Elasticsearch.Net;

namespace Register_Login_Elasticsearch.SeriLog
{
    public class LogDocument
    {
        public string Level { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public string LogEvent { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string MessageTemplate { get; set; }
        public int UserId { get; set; }

//        //public void LogUserActivity(string? elasticId, string userId, string message, LogEventLevel logEventLevel = LogEventLevel.Information)
//        //{
//        //    var logDocument = new LogDocument
//        //    {
//        //        ElasticId = elasticId ?? throw new ArgumentNullException(nameof(elasticId)),
//        //        Message = message ?? throw new ArgumentNullException(nameof(message)),
//        //        UserId = userId,
//        //        LogEventLevel = logEventLevel,
//        //        Timestamp = DateTime.UtcNow
//        //    };

//        //    Log.Logger.Write(logEventLevel, "{@LogDocument}", logDocument);
//        //}

//        //public LogDocument(string elasticId, string message, LogEventLevel logEventLevel, DateTime timestamp)
//        //{
//        //    ElasticId = elasticId;
//        //    Message = message;
//        //    LogEventLevel = logEventLevel;
//        //    Timestamp = timestamp;
//        //}

    }
}
