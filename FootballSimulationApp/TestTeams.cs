using System.Numerics;
using FootballSimulation;
using System;
using System.Drawing;
using System.Collections.ObjectModel;

namespace FootballSimulationApp
{
    public class NullTeam : Team
    {
        public NullTeam(ReadOnlyCollection<PointMass> players, RectangleF goalBounds) :
            base(players, goalBounds)
        {
        }

        public override Kick Execute(ISimulation simulation) { return Kick.None; }
    }

    public class TeamA : Team
    {
        public TeamA(ReadOnlyCollection<PointMass> players, RectangleF goalBounds) :
            base(players, goalBounds)
        {
        }

        public override Kick Execute(ISimulation simulation)
        {
            return Kick.None;
        }
    }
}
