using System;
using System.Diagnostics;
using System.IO;

namespace LeanplumSDK.WebSocketSharp
{
	internal class Logger
	{
		private volatile string _file;

		private volatile LogLevel _level;

		private Action<LogData, string> _output;

		private object _sync;

		public string File
		{
			get
			{
				return _file;
			}
			set
			{
				lock (_sync)
				{
					_file = value;
					Warn(string.Format("The current path to the log file has been changed to {0}.", _file ?? string.Empty));
				}
			}
		}

		public LogLevel Level
		{
			get
			{
				return _level;
			}
			set
			{
				_level = value;
				Warn(string.Format("The current logging level has been changed to {0}.", _level));
			}
		}

		public Logger()
			: this(LogLevel.ERROR, null, null)
		{
		}

		public Logger(LogLevel level)
			: this(level, null, null)
		{
		}

		public Logger(LogLevel level, string file, Action<LogData, string> output)
		{
			_level = level;
			_file = file;
			_output = ((output == null) ? new Action<LogData, string>(defaultOutput) : output);
			_sync = new object();
		}

		private static void defaultOutput(LogData data, string path)
		{
			string value = data.ToString();
			Console.WriteLine(value);
			if (path != null && path.Length > 0)
			{
				writeLine(value, path);
			}
		}

		private void output(string message, LogLevel level)
		{
			if (level < _level || message == null || message.Length == 0)
			{
				return;
			}
			lock (_sync)
			{
				LogData logData = null;
				try
				{
					logData = new LogData(level, new StackFrame(2, true), message);
					_output(logData, _file);
				}
				catch (Exception ex)
				{
					logData = new LogData(LogLevel.FATAL, new StackFrame(0, true), ex.Message);
					Console.WriteLine(logData.ToString());
				}
			}
		}

		private static void writeLine(string value, string path)
		{
			using (StreamWriter writer = new StreamWriter(path, true))
			{
				using (TextWriter textWriter = TextWriter.Synchronized(writer))
				{
					textWriter.WriteLine(value);
				}
			}
		}

		public void Debug(string message)
		{
			output(message, LogLevel.DEBUG);
		}

		public void Error(string message)
		{
			output(message, LogLevel.ERROR);
		}

		public void Fatal(string message)
		{
			output(message, LogLevel.FATAL);
		}

		public void Info(string message)
		{
			output(message, LogLevel.INFO);
		}

		public void SetOutput(Action<LogData, string> output)
		{
			lock (_sync)
			{
				_output = ((output == null) ? new Action<LogData, string>(defaultOutput) : output);
				Warn("The current output action has been replaced.");
			}
		}

		public void Trace(string message)
		{
			output(message, LogLevel.TRACE);
		}

		public void Warn(string message)
		{
			output(message, LogLevel.WARN);
		}
	}
}
