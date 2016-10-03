using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud.Core.Models
{
    public abstract class CloudItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
