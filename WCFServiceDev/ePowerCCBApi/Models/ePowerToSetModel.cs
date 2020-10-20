using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePowerCCBApi.Models
{
    public class ePowerToSetModel
    {
        public int applicationId { get; set; }
        public int doctypeId { get; set; }
        public int queryId { get; set; }
        public string[] parametro { get; set; }
        public string[] parametroSet { get; set; }
        public string imagen { get; set; }
        public string imgname { get; set; }
        public string extension { get; set; }
        public string carpeta { get; set; }
        public int posicion
        {
            get; set;
        }
    }
}
