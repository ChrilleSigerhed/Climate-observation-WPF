using System;
using System.Collections.Generic;
using System.Text;

namespace Klimatobservationer.Classes
{
    class Measurement
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public int Observation_id { get; set; }
        public int Category_id { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return $"{Id}. {Name} Value: {Value}";
        }

    }
}
