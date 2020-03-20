using System;
using System.Collections.Generic;
using System.Text;

namespace Klimatobservationer.Classes
{
    class Measurement
    {
        public int ID { get; set; }
        public double Value { get; set; }
        public int Observation_id { get; set; }
        public int Category_id { get; set; }

    }
}
