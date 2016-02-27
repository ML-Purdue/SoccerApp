using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace FootballSimulationApp
{
    internal sealed class FixedTimeStepGameLoop
    {
        /// <summary>
        ///     A method called on a regular interval.
        /// </summary>
        /// <param name="step">The time passed since the last call.</param>
        public delegate void Step(TimeSpan step);

        private readonly Step _draw;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly Step _update;

        private TimeSpan _accumulatedTime;
        private TimeSpan _lastTime;

        /// <summary>
        ///     After <see cref="Start" /> has been called, <see cref="FixedTimeStepGameLoop" /> will try to call <c>update</c> on
        ///     the fixed interval specified by <c>targetElapsedTime</c>. After this, if it is not time to call <c>update</c>
        ///     again, <c>draw</c> is call. The game loop idles until the next time to call <c>update</c>.
        /// </summary>
        /// <param name="targetElapsedTime">The target time between calls to <c>update</c>.</param>
        /// <param name="maxElapsedTime">The maximum amount of time between calls to <c>draw</c>.</param>
        /// <param name="update">Processes game logic.</param>
        /// <param name="draw">Draws a frame.</param>
        public FixedTimeStepGameLoop(TimeSpan targetElapsedTime, TimeSpan maxElapsedTime, Step update, Step draw)
        {
            Contract.Requires<ArgumentException>(targetElapsedTime.Ticks > 0);
            Contract.Requires<ArgumentException>(targetElapsedTime <= maxElapsedTime);
            Contract.Requires<ArgumentNullException>(update != null);
            Contract.Requires<ArgumentNullException>(draw != null);

            TargetElapsedTime = targetElapsedTime;
            MaxElapsedTime = maxElapsedTime;
            _update = update;
            _draw = draw;
        }

        /// <summary>The target time between update calls.</summary>
        public TimeSpan TargetElapsedTime { get; }

        /// <summary>The maximum time between draw calls.</summary>
        public TimeSpan MaxElapsedTime { get; }

        /// <summary>
        ///     Starts the game loop.
        /// </summary>
        public void Start() => _stopwatch.Start();

        /// <summary>
        ///     Calls update and draw when appropriate. This method should be called
        ///     whenever the application does not have messages to process.
        /// </summary>
        public void Tick()
        {
            if ((_accumulatedTime += GetElapsedTime()) < TargetElapsedTime)
                return;

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