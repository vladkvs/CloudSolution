using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud.Core.Factory
{
    public abstract class Creator
    {
        public abstract ICloud CreateCloud();
    }
}
