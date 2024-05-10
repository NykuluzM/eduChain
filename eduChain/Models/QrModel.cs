using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eduChain.Models
{
    public class QrModel
    {
        public bool IsSelected { get; set; }
        public int Id { get; set; }
        public DateTime Expiration { get; set; }
    }
}
