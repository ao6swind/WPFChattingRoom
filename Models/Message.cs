using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    class Message
    {
        public User From { get; set; }
        public User To { get; set; }
        public string Content { get; set; }
    }
}
