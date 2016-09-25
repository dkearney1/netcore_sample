using System;
using NLog;
using NLog.Config;

namespace LoggingImplementations
{
    public sealed class NLogLogger : Logger, LoggingInterfaces.ILogger
    {
        private const string _loggerName = nameof(NLogLogger);

        public static LoggingInterfaces.ILogger GetLogger()
        {
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition(
                "utc_date",
                typeof(UtcDateRenderer));

            var logger = LogManager.GetLogger(
                nameof(NLogLogger),
                typeof(NLogLogger)) as LoggingInterfaces.ILogger;

            return logger;
        }

        public void Trace(Exception exception) => Trace(exception, string.Empty);
        public new void Trace(Exception exception, string format, params object[] args)
        {
            if (!base.IsTraceEnabled) return;
            base.Log(LogLevel.Trace, exception, format, args);
        }

        public void Debug(Exception exception) => Debug(exception, string.Empty);
        public new void Debug(Exception exception, string format, params object[] args)
        {
            if (!base.IsDebugEnabled) return;
            base.Log(LogLevel.Debug, exception, format, args);
        }

        public void Info(Exception exception) => Info(exception, string.Empty);
        public new void Info(Exception exception, string format, params object[] args)
        {
            if (!base.IsInfoEnabled) return;
            base.Log(LogLevel.Info, exception, format, args);
        }

        public void Warn(Exception exception) => Warn(exception, string.Empty);
        public new void Warn(Exception exception, string format, params object[] args)
        {
            if (!base.IsWarnEnabled) return;
            base.Log(LogLevel.Warn, exception, format, args);
        }

        public void Error(Exception exception) => Error(exception, string.Empty);
        public new void Error(Exception exception, string format, params object[] args)
        {
            if (!base.IsErrorEnabled) return;
            base.Log(LogLevel.Error, exception, format, args);
        }

        public void Fatal(Exception exception) => Fatal(exception, string.Empty);
        public new void Fatal(Exception exception, string format, params object[] args)
        {
            if (!base.IsFatalEnabled) return;
            base.Log(LogLevel.Fatal, exception, format, args);
        }
    }
}