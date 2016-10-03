using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud.Core.Models
{
    public class CloudFile : CloudItem
    {
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
    }
}
