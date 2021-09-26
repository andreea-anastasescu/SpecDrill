using Microsoft.Extensions.Logging;
using SpecDrill.Infrastructure;
using SpecDrill.Secondary.Ports.AutomationFramework;
using System;
using System.Collections.Generic;

namespace SpecDrill.AutomationScopes
{
    public sealed class ImplicitWaitScope : IDisposable
    {
        private readonly ILogger Logger = DI.GetLogger<ImplicitWaitScope>();

        private readonly string message;
        private readonly IBrowserDriver browser;
        private readonly Stack<TimeSpan> timeoutHistory;
        public ImplicitWaitScope(IBrowserDriver browser, Stack<TimeSpan> timeoutHistory, TimeSpan timeout, string? message)
        {
            this.message = message ?? string.Empty;
            this.browser = browser;
            this.timeoutHistory = timeoutHistory;

            lock (timeoutHistory)
            {
                timeoutHistory.Push(timeout);
                browser.ChangeBrowserDriverTimeout(timeout);
                Logger.LogInformation(string.Format("ImplicitWaitScope: Set Timeout to {0}. {1}", timeout, message));
            }
        }

        public void Dispose()
        {
            TimeSpan previousTimeout;
            lock (timeoutHistory)
            {
                previousTimeout = timeoutHistory.Pop();
                browser.ChangeBrowserDriverTimeout(previousTimeout);
            }

            Logger.LogInformation(string.Format("ImplicitWaitScope: Restored Timeout to {0}. {1}", previousTimeout, message ?? string.Empty));
        }

        public static ImplicitWaitScope Create(IBrowserDriver driver, Stack<TimeSpan> timeoutHistory, TimeSpan timeout, string? message = null)
        {
            return new ImplicitWaitScope(driver, timeoutHistory, timeout, message);
        }
    }
}
