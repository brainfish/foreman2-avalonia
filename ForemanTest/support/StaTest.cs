using System;
using Avalonia.Threading;

namespace ForemanTest.support {
    /// <summary>Runs UI code on the Avalonia dispatcher thread started for tests.</summary>
    internal static class StaTest {
        public static void Run(Action body) {
            if (Dispatcher.UIThread.CheckAccess()) {
                body();
                return;
            }
            Dispatcher.UIThread.Invoke(body);
        }
    }
}
