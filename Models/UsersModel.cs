using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PScnFin
{
    public class UsersModel
    {
        public string pc_name { get; set; }
        public string ip { get; set; }
        public string full
        {
            get
            {
                return $"{pc_name} {ip}";
            }
        }
    }
}
