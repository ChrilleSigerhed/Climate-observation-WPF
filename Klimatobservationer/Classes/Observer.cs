using System;
using System.Collections.Generic;
using System.Text;

namespace Klimatobservationer.Classes
{
    class Observer
    {
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public int Id { get; set; }

        public override string ToString()
        {
            return $"{Firstname} {Lastname}";
        }
    }
}
