using Moq;
using ProcessMonitor;
using System.IO;
using System.Text;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace ProcessMonitorTests
{
    public class ProcessMonitorAppTests
    {
        private ITestOutputHelper _output;

        public ProcessMonitorAppTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async void ReturnSuccessCodeWhenProcessAlreadyExceedTimelifeWithForceStop()
        {
            using var cancelTokenSource = new CancellationTokenSource();

            var consoleMessage = "The process lifetime has already been exceeded. The process is killed.";

            var (console, sb) = GetConsole();

            var mockProcessService = GetMockProcessService(processLifetime: 6);

            var mockSleepService = GetSleepService();

            var app = new ProcessMonitorApp(console, mockProcessService.Object, mockSleepService.Object)
            {
                ProcessName = "test",
                Lifetime = "5",
                WaitClosing = false
            };
            var res = await app.OnExecute(cancelTokenSource.Token);
            Assert.Equal(0, res);
            Assert.Contains(consoleMessage, sb.ToString());
            mockProcessService.Verify(p => p.ForceStopProcess(), Times.Once());
        }

        [Fact]
        public async void ReturnErrorCodeWhenProcessNotRunning()
        {
            using var cancelTokenSource = new CancellationTokenSource();

            var processName = "testprocess";
            var consoleMessage = $"The process '{processName}' does not running";

            var (console, sb) = GetConsole();

            var mockProcessService = GetMockProcessService(isProcessRunning: false);

            var mockSleepService = GetSleepService();

            var app = new ProcessMonitorApp(console, mockProcessService.Object, mockSleepService.Object)
            {
                ProcessName = processName,
            };
            var res = await app.OnExecute(cancelTokenSource.Token);
            Assert.Equal(1, res);
            Assert.Contains(consoleMessage, sb.ToString());
        }

        [Fact]
        public async void ReturnSuccessCodeWhenProcessSuccessKilled()
        {
            using var cancelTokenSource = new CancellationTokenSource();

            var processName = "testprocess";
            var consoleMessage = $"The process {processName} was successfully stopped.";

            var (console, sb) = GetConsole();

            var mockProcessService = GetMockProcessService(processLifetime:1);

            var mockSleepService = GetSleepService();

            var app = new ProcessMonitorApp(console, mockProcessService.Object, mockSleepService.Object)
            {
                ProcessName = processName,
                Lifetime = "5",
                Rate = "1",
                WaitClosing = false
            };
            var res = await app.OnExecute(cancelTokenSource.Token);
            Assert.Equal(0, res);
            Assert.Contains(consoleMessage, sb.ToString());
            mockSleepService.Verify(s => s.Sleep(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(4));
            mockProcessService.Verify(p => p.ForceStopProcess(), Times.Once());
        }

        private Mock<IProcessService> GetMockProcessService(bool isProcessRunning = true, int processLifetime = 5)
        {
            var mock = new Mock<IProcessService>();
            mock.SetupSet(p => p.ProcessName = It.IsAny<string>());
            mock.SetupGet(p => p.IsProcessRunning).Returns(isProcessRunning);
            mock.SetupGet(p => p.ProcessLifetime).Returns(()=>processLifetime++);
            mock.Setup(p => p.ForceStopProcess()).Verifiable();
            return mock;
        }

        private (TestConsole, StringBuilder) GetConsole()
        {
            var sb = new StringBuilder();
            var console = new TestConsole(_output)
            {
                Out = new StringWriter(sb)
            };
            return (console, sb);
        }

        private Mock<ISleepService> GetSleepService()
        {
            var mock = new Mock<ISleepService>();
            mock.Setup(m => m.Sleep(It.IsAny<int>(), It.IsAny<CancellationToken>()));
            return mock;
        }
    }
}
