using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePowerCCBApi.Models
{
    public class ePowerGetInfoModels
    {
        public int applicationId { get; set; }
        public int doctypeId { get; set; }
        public int queryId { get; set; }
        public string[] parametros { get; set; }
    }
}
