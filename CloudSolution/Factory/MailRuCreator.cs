using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloud.Core.MailRu;

namespace Cloud.Core.Factory
{
    public class MailRuCreator : Creator
    {
        public override ICloud CreateCloud()
        {
            return new MailRuCloud();
        }
    }
}
