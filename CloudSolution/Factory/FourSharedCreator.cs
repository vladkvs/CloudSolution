using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloud.Core.FourShared;

namespace Cloud.Core.Factory
{
    public class FourSharedCreator : Creator
    {
        public override ICloud CreateCloud()
        {
            return new FourSharedCloud();
        }
    }
}
