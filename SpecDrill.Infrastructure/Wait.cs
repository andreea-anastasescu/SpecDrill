using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpecDrill.Configuration;
using SpecDrill.Infrastructure;
using SpecDrill.Infrastructure.Configuration;
using System;
using System.Diagnostics;
using System.Threading;

namespace SpecDrill
{
    public class RetryWaitContext
    {
        protected ILogger Logger = DI.GetLogger<RetryWaitContext>();
        
        public int RetryCount { get; set; }
        public TimeSpan? RetryInterval { get; set; }

        private Action action = () => { return; };

        public void Until(Func<bool> waitCondition)
        {
            Stopwatch sw = new();

            // default retry interval is 20s
            var retryInterval = this.RetryInterval ?? TimeSpan.FromSeconds(20);
            int retryCount = this.RetryCount;
            while (retryCount >= 0)
            {
                bool actionSucceeded = SafelyPerform(action, retryCount);

                retryCount--;
                if (actionSucceeded)
                {
                    sw.Start();
                    while (sw.Elapsed < retryInterval)
                    {
                        var conditionMet = SafelyEvaluate(waitCondition, retryInterval, retryCount);
                        if (conditionMet)
                            return;
                        Thread.Sleep(10);
                    }
                    sw.Reset();
                }
                Thread.Sleep(10);
            }
            sw.Stop();

            throw new TimeoutException(string.Format("Explicit Wait with Retry of (1+{0})*{1} Timed Out!", this.RetryCount, retryInterval));
        }
        private bool SafelyPerform(Action action, int retryCount)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"TryingAction: retryCount={retryCount}");
                //HACK: workaround for https://stackoverflow.com/questions/48450594/selenium-timed-out-receiving-message-from-renderer
                if (e.Message.Contains("timeout") && e.Message.Contains("renderer"))
                    return true;
            }
            return false;
        }
        private bool SafelyEvaluate(Func<bool> waitCondition, TimeSpan retryInterval, int retryCount)
        {
            try
            {
                if (waitCondition())
                    return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Wait with retry: retryCount={0}, retryInterval={1} / maxWait={2}", retryCount, this.RetryInterval ?? TimeSpan.FromSeconds(0), retryInterval);
                
            }
            return false;
        }

        public RetryWaitContext Doing(Action action)
        {
            this.action = action ?? this.action;
            return this;
        }
    }

    public class MaxWaitContext
    {
        protected static readonly ILogger Logger = DI.GetLogger<MaxWaitContext>();
        public TimeSpan MaximumWait { get; set; }
        private Func<Func<bool>, bool, Tuple<bool, Exception?>> safeWait = (waitCondition, throwException) =>
         {
            // exception is null =>
            // true, null -> true
            // false, null -> false
            // exception is not null
            // => exception (inconclusive)
            bool result = false;
             try
             {
                 result = waitCondition();
             }
             catch (Exception e)
             {
                 Logger.LogError(e, "Error on wait");
                 if (throwException)
                     throw;
                 return Tuple.Create<bool, Exception?>(result, e);
             }
             return Tuple.Create<bool, Exception?>(result, null);
         };

        public void Until(Func<bool> waitCondition, bool throwExceptionOnTimeout = true)
        {
            Func<Tuple<bool, Exception?>> safeWaitCondition = () => safeWait(waitCondition, false);
            bool conclusive = false;
            bool conditionMet = false;
            Exception? lastError = null;

            Stopwatch sw = new Stopwatch();

            sw.Start();
            while (sw.Elapsed < MaximumWait)
            {
                var waitResult = safeWaitCondition();

                lastError = waitResult.Item2;
                conclusive = lastError == null;
                conditionMet = waitResult.Item1;
                Logger.LogInformation($"c = {conclusive}, cm={conditionMet}, maxWait = {MaximumWait}");

                if (conclusive && conditionMet)
                {
                    return;
                }

                Thread.Sleep(33);
            }
            sw.Stop();

            if (!conditionMet && throwExceptionOnTimeout)
            {
                throw new TimeoutException($"Explicit Wait of {this.MaximumWait} Timed Out ! Reason: {lastError?.ToString() ?? "(see logs)"}");
            }
        }
    }

    public static class Wait
    {
        public static MaxWaitContext NoMoreThan(TimeSpan maximumWait)
        {
            return new MaxWaitContext { MaximumWait = maximumWait };
        }

        /// <summary>
        /// Default parameters: retryCount = 3, retryInterval = 20s
        /// </summary>
        /// <param name="retryCount"></param>
        /// <param name="retryInterval"></param>
        /// <returns></returns>
        public static RetryWaitContext WithRetry(int retryCount = 3, TimeSpan? retryInterval = null)
        {
            return new RetryWaitContext { RetryCount = retryCount, RetryInterval = retryInterval };
        }

        public static void Until(Func<bool> waitCondition)
        {
            new MaxWaitContext
            {
                MaximumWait = TimeSpan.FromMilliseconds(ConfigurationManager.Settings.WebDriver?.MaxWait ?? 60000)
            }.Until(waitCondition);
        }
    }
}
