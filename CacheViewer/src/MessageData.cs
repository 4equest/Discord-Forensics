using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CacheViewer
{
    public class MessageData
    {
        public string? ID { get; set; }
        public string? Content { get; set; }
        public string? ChannelID { get; set; }
        public string? AuthorID { get; set; }
        public string? AuthorUsername { get; set; }
        public bool Pinned { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
