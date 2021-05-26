using System;
using System.Collections.Generic;
using System.Text;

namespace Communications.Core.Models
{
    public class SNSMessage
    {
        public string Tag { get; set; }
        public string Id { get; set; }
        public DateTime PublishedDate {get;set;}
        public object Data { get; set; }
    }
}
