using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PScnFin
{
    public class DataModel
    {
        public int data_id { get; set; }
        public int positive_scan { get; set; }
        public int negative_scan { get; set; }
        public string pc_name { get; set; }
        public string process_name { get; set; }
        public int scan_id { get; set; }

    }
}
