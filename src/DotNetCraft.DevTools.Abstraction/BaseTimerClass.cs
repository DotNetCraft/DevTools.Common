using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace DotNetCraft.DevTools.Abstraction
{
    public abstract class BaseTimerClass: IStartStop, IDisposable
    {
        private readonly ILogger<BaseTimerClass> _logger;

        private readonly string _className;
        private readonly Timer _timer;
        private bool _isBusy;
        private readonly CancellationTokenSource _cts;

        protected BaseTimerClass(ITimerConfig config, ILogger<BaseTimerClass> logger)
        {
            if (config == null) 
                throw new ArgumentNullException(nameof(config));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _className = GetType().Name;
            _cts = new CancellationTokenSource();

            _timer = new Timer(config.IntervalMs);
            _timer.Elapsed +=TimerElapsedAsync;
        }

        private async void TimerElapsedAsync(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _logger.LogTrace($"{_className}: Executing timer...");

            if (_cts.Token.IsCancellationRequested)
            {
                _logger.LogTrace($"{_className}: Timer was cancelled...");
                return;
            }

            lock (_timer)
            {
                if (_isBusy)
                {
                    _logger.LogWarning($"{_className}: Another timer is running => skipping");
                    return;
                }
            }

            try
            {
                await OnTimerElapsedAsync(sender, elapsedEventArgs, _cts.Token);

                _logger.LogTrace($"{_className}: Timer was executed.");
            }
            catch (OperationCanceledException e)
            {
                _logger.LogInformation(e,$"{_className}: Timer was cancelled: {e.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{_className}: Failed to execute timer: {ex.Message}");
            }
            finally
            {
                _isBusy = false;
            }
        }

        protected abstract Task OnTimerElapsedAsync(object sender, ElapsedEventArgs elapsedEventArgs, CancellationToken ctsToken);

        protected virtual Task OnTimerStartAsync()
        {
            _timer.Start();
            return Task.CompletedTask;
        }

        protected virtual Task OnTimerStopAsync()
        {
            _cts.Cancel();
            _timer.Stop();
            return Task.CompletedTask;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"{_className}: Starting timer...");

                await OnTimerStartAsync();

                _logger.LogInformation($"{_className}: Timer was started");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"{_className}: Failed to start timer: {ex.Message}");
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"{_className}: Stopping timer...");

                await OnTimerStopAsync();
                
                _logger.LogInformation($"{_className}: Timer was stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{_className}: Failed to stop timer: {ex.Message}");
                throw;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //try
                //{
                //    Task.WaitAll(StopAsync());
                //}
                //catch (Exception ex)
                //{
                //    _logger.LogError(ex, $"{_className}: Failed to stop timer from Dispose method: {ex.Message}");
                //}

                _timer.Dispose();
                _cts.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
