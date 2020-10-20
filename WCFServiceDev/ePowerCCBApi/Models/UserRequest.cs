using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePowerCCBApi.Models
{
    public class UserRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public int mode { get; set; }
    }
}
