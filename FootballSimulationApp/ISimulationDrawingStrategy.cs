using System.Drawing;
using FootballSimulation;

namespace FootballSimulationApp
{
    internal interface ISimulationDrawingStrategy
    {
        void Draw(Graphics g, ISimulation s);
    }
}