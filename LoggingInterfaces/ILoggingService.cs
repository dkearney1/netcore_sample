using System;

namespace LoggingInterfaces
{
	public interface ILogger
	{
		bool IsTraceEnabled{ get; }
		bool IsDebugEnabled{ get; }
		bool IsInfoEnabled{ get; }
		bool IsWarnEnabled{ get; }
		bool IsErrorEnabled{ get; }
		bool IsFatalEnabled{ get; }

		void Trace(Exception exception);
		void Trace(string format, params object[] args);
		void Trace(Exception exception, string format, params object[] args);

		void Debug(Exception exception);
		void Debug(string format, params object[] args);
		void Debug(Exception exception, string format, params object[] args);

		void Info(Exception exception);
		void Info(string format, params object[] args);
		void Info(Exception exception, string format, params object[] args);

		void Warn(Exception exception);
		void Warn(string format, params object[] args);
		void Warn(Exception exception, string format, params object[] args);

		void Error(Exception exception);
		void Error(string format, params object[] args);
		void Error(Exception exception, string format, params object[] args);

		void Fatal(Exception exception);
		void Fatal(string format, params object[] args);
		void Fatal(Exception exception, string format, params object[] args);
	}
}