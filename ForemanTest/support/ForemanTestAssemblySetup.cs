using Avalonia;
using Avalonia.Headless;
using Foreman;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;

namespace ForemanTest.support {
    [TestClass]
    public static class ForemanTestAssemblySetup {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext _) {
            UserMessages.TestHandler = UserMessages.FailTestOnAnyMessage;
            if (Avalonia.Application.Current is null) {
                AppBuilder.Configure<ForemanAvaloniaApp>()
                    .UseHeadless(new AvaloniaHeadlessPlatformOptions())
                    .SetupWithoutStarting();
            }
            AvaloniaBootstrap.EnsureInitialized();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup() {
            UserMessages.TestHandler = null;
        }
    }
}
