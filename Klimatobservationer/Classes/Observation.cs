using System;
using System.Collections.Generic;
using System.Text;

namespace Klimatobservationer.Classes
{
    class Observation
    {
        public DateTime Date { get; set; }

        public int Id { get; set; }

        public int Observer_id { get; set; }

        public int Geolocation_id { get; set; }
        

        public override string ToString()
        {
            return $"{Id}. {Date.Year}-{Date.Month}-{Date.Day}";
        }
    }
}
