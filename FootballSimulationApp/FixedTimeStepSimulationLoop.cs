using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;

namespace FootballSimulationApp
{
    internal sealed class FixedTimeStepSimulationLoop : ISimulationLoop
    {
        private static readonly Action<TimeSpan> DefaultSimulationStep = t => { };

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private TimeSpan _accumulatedTime;
        private Action<TimeSpan> _draw = DefaultSimulationStep;
        private TimeSpan _lastTime;
        private Action<TimeSpan> _update = DefaultSimulationStep;

        /// <summary>
        ///     After <see cref="Start" /> has been called, <see cref="FixedTimeStepSimulationLoop" /> will try to call
        ///     the update function on the fixed interval specified by <see cref="TargetElapsedTime" />. After this, if it is not
        ///     time to call update again, the draw function is called. The game loop idles until the next time to call update.
        /// </summary>
        /// <param name="targetElapsedTime">The target time between calls to <c>update</c>.</param>
        /// <param name="maxElapsedTime">The maximum amount of time between calls to <c>draw</c>.</param>
        public FixedTimeStepSimulationLoop(TimeSpan targetElapsedTime, TimeSpan maxElapsedTime)
        {
            Contract.Requires<ArgumentException>(targetElapsedTime.Ticks > 0);
            Contract.Requires<ArgumentException>(targetElapsedTime <= maxElapsedTime);

            TargetElapsedTime = targetElapsedTime;
            MaxElapsedTime = maxElapsedTime;
        }

        /// <summary>The target time between update calls.</summary>
        public TimeSpan TargetElapsedTime { get; }

        /// <summary>The maximum time between draw calls.</summary>
        public TimeSpan MaxElapsedTime { get; }

        /// <summary>Gets a value indicating whether the game loop is running.</summary>
        public bool IsRunning => _stopwatch.IsRunning;

        /// <summary>
        ///     Starts the game loop.
        /// </summary>
        public void Start() => _stopwatch.Start();

        /// <summary>
        ///     Sets the update function that is called when the simulation needs to update.
        /// </summary>
        /// <param name="step">The update function.</param>
        public void SetUpdate(Action<TimeSpan> step) => _update = step ?? DefaultSimulationStep;

        /// <summary>
        ///     Sets the draw function that is called when the simulation needs to draw.
        /// </summary>
        /// <param name="step">The draw function.</param>
        public void SetDraw(Action<TimeSpan> step) => _draw = step ?? DefaultSimulationStep;

        /// <summary>
        ///     Calls update and draw when appropriate. This method should be called
        ///     whenever the application does not have messages to process.
        /// </summary>
        public void Tick()
        {
            if ((_accumulatedTime += GetElapsedTime()) < TargetElapsedTime)
            {
                Thread.Sleep(TargetElapsedTime - _accumulatedTime);
                return;
            }

            var totalElapsedTime = _accumulatedTime;
            _update(TargetElapsedTime);
            while ((_accumulatedTime -= TargetElapsedTime) >= TargetElapsedTime)
                _update(TargetElapsedTime);

            _draw(totalElapsedTime);
        }

        private TimeSpan GetElapsedTime()
        {
            var currentTime = _stopwatch.Elapsed;
            var elapsedTime = currentTime - _lastTime;

            _lastTime = currentTime;
            return elapsedTime > MaxElapsedTime ? MaxElapsedTime : elapsedTime;
        }
    }
}