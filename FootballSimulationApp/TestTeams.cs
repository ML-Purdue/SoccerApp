using System.Collections.ObjectModel;
using System.Drawing;
using FootballSimulation;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System;

namespace FootballSimulationApp
{
    internal class NullTeam : Team
    {
        public NullTeam(ReadOnlyCollection<PointMass> players, RectangleF goalBounds) :
            base(players, goalBounds)
        {
        }

        public override Kick Execute(ISimulation simulation)
        {
            return Kick.None;
        }
    }

    internal class KeepawayTeam : Team
    {
        private Dictionary<IPointMass, string> messages = new Dictionary<IPointMass, string>();

        public KeepawayTeam(ReadOnlyCollection<PointMass> players, RectangleF goalBounds) :
            base(players, goalBounds)
        {
        }

        public override Kick Execute(ISimulation simulation)
        {
            var allPlayers = simulation.Teams[0].Players.Concat(simulation.Teams[1].Players);
            var ballChaser = FootballStrategies.ClosestToPoint(Players, (PointMass)simulation.Ball, 100);
            Kick kick = Kick.None;

            foreach (var player in Players)
            {
                if (player == ballChaser)
                    kick = FootballStrategies.ChaseBall(player, this, simulation.Ball, simulation);
                else
                    FootballStrategies.SpreadOut(player, allPlayers, simulation.PitchBounds, -150, 100);
            }

            return kick;
        }

        public override void DrawDebugInfo(Graphics g)
        {
            foreach (var p in Players)
            {
                g.DrawLine(Pens.Orange, p.Position, p.Position + 3 * p.Velocity);
                g.DrawLine(Pens.Purple, p.Position, p.Position + 3 * p.Acceleration);
                string message;
                if (messages.TryGetValue(p, out message))
                    g.DrawString(message, SystemFonts.DefaultFont, Brushes.Black, p.Position.X + 10, p.Position.Y + 10);
            }
        }
    }
}
