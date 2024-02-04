using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estranged.Lfs.Data
{
    public  class LockStatus
    {
        public bool IsLocked { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
