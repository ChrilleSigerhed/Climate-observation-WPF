using System;
using System.Collections.Generic;
using System.Text;

namespace Klimatobservationer.Classes
{
    class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int BaseId { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
        
        
    }
}
