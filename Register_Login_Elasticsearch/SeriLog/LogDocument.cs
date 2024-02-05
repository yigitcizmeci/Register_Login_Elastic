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
        public string Message { get; set; }
        public string Exception { get; set; }
        public string MessageTemplate { get; set; }
        public string UserId { get; set; }
        public LogEventDetails logEventDetails { get; set; }

    }
    public class LogEventDetails
    {
        public DateTimeOffset TimeStamp { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}
