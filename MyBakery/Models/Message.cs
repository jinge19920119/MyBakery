using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyBakery.Models
{
    public class Message
    {
        public String text { get; set; }
        public Message() { }
        public Message(String text)
        {
            this.text = text;
        }
    }
}