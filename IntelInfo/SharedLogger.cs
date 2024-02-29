using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayMagicianShared
{
    public class SharedLogger
    {

        public static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Provides a way of passing the NLog Logger instance to the DisplayMagician.Shared library so we log to a single log file.
        /// </summary>
        /// <param name="parentLogger"></param>
        public SharedLogger(NLog.Logger parentLogger)
        {
            SharedLogger.logger = parentLogger;
        }
    }
}