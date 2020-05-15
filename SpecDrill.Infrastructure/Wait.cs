using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SpecDrill.Infrastructure;
using SpecDrill.Infrastructure.Logging;
using SpecDrill.Infrastructure.Logging.Interfaces;

namespace SpecDrill
{
    public class RetryWaitContext
    {
        protected ILogger Log = Infrastructure.Logging.Log.Get<RetryWaitContext>();

        public int RetryCount { get; set; }
        public TimeSpan? RetryInterval { get; set; }

        private Action action = () => { return; };

        public void Until(Func<bool> waitCondition)
        {
            Stopwatch sw = new Stopwatch();

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
                Log.Error(e, $"TryingAction: retryCount={retryCount}");
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
                Log.Error(e, "Wait with retry: retryCount={0}, retryInterval={1} / maxWait={2}", retryCount, this.RetryInterval ?? TimeSpan.FromSeconds(0), retryInterval);
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
        protected static readonly ILogger Log = Infrastructure.Logging.Log.Get<MaxWaitContext>();
        public TimeSpan MaximumWait { get; set; }
        private Func<Func<bool>, bool, Tuple<bool,Exception?>> safeWait = (waitCondition, throwException) =>
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
                Log.Error(e, "Error on wait");
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
                Log.Info($"c = {conclusive}, cm={conditionMet}, maxWait = {MaximumWait}");
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
                MaximumWait = TimeSpan.FromMilliseconds(Globals.Configuration?.WebDriver?.MaxWait ?? 60000)
            }.Until(waitCondition);
        }
    }
}
