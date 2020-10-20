using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;

//[assembly: log4net.Config.XmlConfigurator(ConfigFile = "App.config", Watch = true)]
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace EpowerWSManagement
{
    public sealed class Bitacorizador
    {
        private static readonly ILog logger =
           LogManager.GetLogger(typeof(Bitacorizador));
        /*private static readonly log4net.ILog log = log4net.LogManager.GetLogger
                (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);*/
        private static Bitacorizador instance = null;
        private static readonly object padlock = new object(); 
        public Bitacorizador()
        {
            
        }
        public static Bitacorizador Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        //log4net.Config.XmlConfigurator.Configure();
                        BasicConfigurator.Configure();
                        instance = new Bitacorizador();
                    }
                    return instance;
                }
            }
        }

        public void LogError(string error)
        {
            logger.Error(error);
        }

        public void LogDebug(string message)
        {
            logger.Debug(message);
        }

        public void LogInfo(string message)
        {
            logger.Info(message);
        }

    }
}
