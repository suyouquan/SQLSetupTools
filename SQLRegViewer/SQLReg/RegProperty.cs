using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 

namespace SQLReg
{
    public class RegProperty
    {
        public String Name { get; set; }
        public String Type { get; set; }
        public String Data { get; set; }
        public RegProperty(string nm,string tp,string data)
        {
            Name = nm;
            Type = tp;
            Data = data;
        }
    }
}
