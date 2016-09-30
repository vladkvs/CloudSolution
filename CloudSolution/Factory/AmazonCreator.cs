using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloud.Core.Amazon;

namespace Cloud.Core.Factory
{
    public class AmazonCreator : Creator
    {
        public override ICloud CreateCloud()
        {
            return new AmazonCloud();
        }
    }
}
