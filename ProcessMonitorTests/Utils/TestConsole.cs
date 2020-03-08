using McMaster.Extensions.CommandLineUtils;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit.Abstractions;

namespace ProcessMonitorTests
{
    public class TestConsole: IConsole
    {
        public TestConsole(ITestOutputHelper output)
        {
            Out = new XunitTextWriter(output);
            Error = new XunitTextWriter(output);
        }

        public TextWriter Out { get; set; }

        public TextWriter Error { get; set; }

        public TextReader In => throw new NotImplementedException();

        public bool IsInputRedirected => throw new NotImplementedException();

        public bool IsOutputRedirected => true;

        public bool IsErrorRedirected => true;

        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }

        public event ConsoleCancelEventHandler? CancelKeyPress;

        public void ResetColor()
        {
        }

        public void RaiseCancelKeyPress()
        {
            // See https://github.com/dotnet/corefx/blob/f2292af3a1794378339d6f5c8adcc0f2019a2cf9/src/System.Console/src/System/ConsoleCancelEventArgs.cs#L14
            var eventArgs = typeof(ConsoleCancelEventArgs)
                .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .First()
                .Invoke(new object[] { ConsoleSpecialKey.ControlC });
            CancelKeyPress?.Invoke(this, (ConsoleCancelEventArgs)eventArgs);
        }
    }

    public class XunitTextWriter : TextWriter
    {
        private readonly ITestOutputHelper _output;
        private readonly StringBuilder _sb = new StringBuilder();

        public XunitTextWriter(ITestOutputHelper output)
        {
            _output = output;
        }

        public override Encoding Encoding => Encoding.Unicode;

        public override void Write(char ch)
        {
            if (ch == '\n')
            {
                _output.WriteLine(_sb.ToString());
                _sb.Clear();
            }
            else
            {
                _sb.Append(ch);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_sb.Length > 0)
                {
                    _output.WriteLine(_sb.ToString());
                    _sb.Clear();
                }
            }

            base.Dispose(disposing);
        }
    }
}
