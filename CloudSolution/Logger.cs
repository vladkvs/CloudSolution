using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Cloud.Core
{
    public abstract class Logger
    {
        public ILog Log { get; set; }

        protected Logger()
        {
            log4net.Config.XmlConfigurator.Configure();
            Log = LogManager.GetLogger(GetType());
        }
    }
}
