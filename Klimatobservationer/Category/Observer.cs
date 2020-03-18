using System;
using System.Collections.Generic;
using System.Text;

namespace Klimatobservationer.Category
{
    class Observer
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public int id { get; set; }
        public override string ToString()
        {
            return $"{firstname} {lastname}";
        }
    }
}
