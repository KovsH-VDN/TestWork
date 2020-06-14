using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandemic
{
    class Contact
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Member1_ID { get; set; }
        public int Member2_ID { get; set; }


        public Contact(DateTime from, DateTime to, int member1_ID, int member2_ID)
        {
            From = from;
            To = to;
            Member1_ID = member1_ID;
            Member2_ID = member2_ID;
        }
    }
}
