using System;
using System.Threading;
using Avalonia;
using Avalonia.Headless;
using Avalonia.Threading;
using Foreman;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;

namespace ForemanTest.support {
    [TestClass]
    public static class ForemanTestAssemblySetup {
        private static readonly ManualResetEventSlim Ready = new(false);
        private static Exception? InitError;
        private static CancellationTokenSource? LoopCts;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext _) {
            UserMessages.TestHandler = UserMessages.FailTestOnAnyMessage;
            LoopCts = new CancellationTokenSource();
            var ui = new Thread(() => {
                try {
                    AppBuilder.Configure<ForemanAvaloniaApp>()
                        .UseHeadless(new AvaloniaHeadlessPlatformOptions())
                        .SetupWithoutStarting();
                    Ready.Set();
                    Dispatcher.UIThread.MainLoop(LoopCts.Token);
                } catch (OperationCanceledException) {
                    Ready.Set();
                } catch (Exception ex) {
                    InitError = ex;
                    Ready.Set();
                }
            }) {
                IsBackground = true,
                Name = "Avalonia-UI"
            };
            ui.Start();
            if (!Ready.Wait(TimeSpan.FromSeconds(20)))
                throw new TimeoutException("Avalonia headless UI thread did not start.");
            if (InitError is not null)
                throw InitError;
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup() {
            UserMessages.TestHandler = null;
            LoopCts?.Cancel();
        }
    }
}
