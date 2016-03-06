using System;

namespace FootballSimulationApp
{
    /// <summary>
    ///     A simulation loop.
    /// </summary>
    internal interface ISimulationLoop
    {
        /// <summary>Gets a value indicating whether the game loop is running.</summary>
        bool IsRunning { get; }

        /// <summary>
        ///     Sets the update function that is called when the simulation needs to update.
        /// </summary>
        /// <param name="step">The update function.</param>
        void SetUpdate(Action<TimeSpan> step);

        /// <summary>
        ///     Sets the draw function that is called when the simulation needs to draw.
        /// </summary>
        /// <param name="step">The draw function.</param>
        void SetDraw(Action<TimeSpan> step);

        /// <summary>
        ///     Starts the game loop.
        /// </summary>
        void Start();

        /// <summary>
        ///     Calls update and draw when appropriate. This method should be called
        ///     whenever the application does not have messages to process.
        /// </summary>
        void Tick();
    }
}