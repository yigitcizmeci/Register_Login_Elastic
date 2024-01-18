//using Nest;
//using Serilog.Events;
//using Serilog;
//using Elasticsearch.Net;

//namespace Register_Login_Elasticsearch.SeriLog
//{
//    public class LogEvent
//    {
//        public string ElasticId { get; init; } = null!;
//        public int UserId { get; set; }
//        public string Message { get; init; } = null!;
//        public LogEventLevel LogEventLevel { get; init; }
//        public DateTime Timestamp { get; init; }

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

//    }
//}
