using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioSample
{
    internal sealed class Logging
    {
        public static Logger Logger => LogManager.GetCurrentClassLogger();

        public static void Setup()
        {
            var config = new LoggingConfiguration();
            var datetime = DateTime.Now.ToString(@"yyMMdd_HH\:mm\:ss");
            var layout = string.Join(" | ", new[]
            {
                @"${longdate}",
                @"${processid}",
                @"${threadid}",
                //@"${callsite:includeNamespace=false} (Line ${callsite-linenumber})",
                @"${level:format=FirstCharacter}",
                @"${message}",
            });
            layout += @"${when:when=length('${exception}')>0:Inner=|}${exception:format=tostring}";
            var logfile = new FileTarget
            {
                Name = "logfile",
                FileName = @"${basedir}/logs/" + $"{datetime}.log",
                Layout = layout,
            };
            var logconsole = new ConsoleTarget
            {
                Name = "logconsole",
                Layout = layout,
            };

            config.AddRule(new LoggingRule("*", LogLevel.Debug, logfile));
            config.AddRule(new LoggingRule("*", LogLevel.Debug, logconsole));
            config.AddTarget("logfile", logfile);
            config.AddTarget("logconsole", logconsole);

            LogManager.Configuration = config;
            LogManager.ReconfigExistingLoggers();
        }

        #region Private
        private Logging() { }
        #endregion
    }
}
