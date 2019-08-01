using System;
using System.Collections.Generic;
using System.Text;

namespace telegram_sender
{
    public class MessageModel
    {
        public int PhotoId { get; set; }
        public string Link { get; set; }
        public string Price { get; set; }
        public string Location { get; set; }
        public string Map { get; set; }
        public (string lat, string lng)? Coordinates { get; set; }
        public DateTime InsertDate { get; set; }
        public List<string> Photos { get; set; }
        public int Distance { get; set; }
        public string WorkDistance { get; set; }
    }
}
