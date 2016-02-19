using System;
using System.Diagnostics;

namespace FootballSimulationApp
{
    internal sealed class GameLoop
    {
        public delegate void Step(TimeSpan step);

        private readonly Step _draw;
        private readonly TimeSpan _maxElapsedTime;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly TimeSpan _targetElapsedTime;
        private readonly Step _update;
        private TimeSpan _accumulatedTime;
        private TimeSpan _lastTime;

        public GameLoop(TimeSpan targetElapsedTime, TimeSpan maxElapsedTime, Step update, Step draw)
        {
            _targetElapsedTime = targetElapsedTime;
            _maxElapsedTime = maxElapsedTime;
            _update = update;
            _draw = draw;
        }

        public void Start() => _stopwatch.Start();

        public void OnTick()
        {
            if ((_accumulatedTime += GetElapsedTime()) < _targetElapsedTime)
                return;

            var totalElapsedTime = _accumulatedTime;
            _update(_targetElapsedTime);
            while ((_accumulatedTime -= _targetElapsedTime) >= _targetElapsedTime)
                _update(_targetElapsedTime);

            _draw(totalElapsedTime);
        }

        private TimeSpan GetElapsedTime()
        {
            var currentTime = _stopwatch.Elapsed;
            var elapsedTime = currentTime - _lastTime;

            _lastTime = currentTime;
            return elapsedTime > _maxElapsedTime ? _maxElapsedTime : elapsedTime;
        }
    }
}