using System;
using System.Drawing;
using FootballSimulation;

namespace FootballSimulationApp
{
    /// <summary>
    ///     Encapsulates a drawing strategy for a football simulation.
    /// </summary>
    internal interface ISimulationDrawingStrategy : IDisposable
    {
        /// <summary>
        ///     Draws the simulation with the specified graphics context.
        /// </summary>
        /// <param name="g">The graphics context.</param>
        /// <param name="s">The simulation to draw.</param>
        void Draw(Graphics g, ISimulation s);
    }
}