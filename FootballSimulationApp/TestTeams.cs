using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using FootballSimulation;

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
        private readonly Dictionary<IPointMass, string> messages = new Dictionary<IPointMass, string>();
        private static Kick k;

        public KeepawayTeam(ReadOnlyCollection<PointMass> players, RectangleF goalBounds) :
            base(players, goalBounds)
        {
        }

        public override Kick Execute(ISimulation simulation)
        {
            var ballChaser = FootballStrategies.ClosestPlayerToPoint(Players, simulation.Ball, 0);
            var kick = Kick.None;

            foreach (var player in Players)
            {
                if (player == ballChaser)
                {
                    messages[player] = "Chaser";

                    var playersExceptSelf = Players.ToList();
                    playersExceptSelf.Remove(player);

                    player.Force = SteeringStrategies.Pursue(player, simulation.Ball, 1);

                    if ((player.Position - simulation.Ball.Position).Length() < 20)
                        k = kick = FootballStrategies.PassToPlayer(player, FootballStrategies
                            .ClosestPlayerToPoint(playersExceptSelf, player, 100), simulation.Ball);
                    else
                       k = kick = Kick.None;
                }
                else
                {
                    messages[player] = "";

                    var allPlayers = simulation.Teams[0].Players.Concat(simulation.Teams[1].Players);
                    FootballStrategies.SpreadOut(player, allPlayers, simulation.PitchBounds, 150, 100);
                }
            }

            return kick;
        }

        public override void DrawDebugInfo(ISimulation simulation, Graphics g)
        {
            g.DrawLine(Pens.GhostWhite, simulation.Ball.Position, simulation.Ball.Position + simulation.Ball.Velocity);
            foreach (var p in Players)
            {
                //g.DrawLine(Pens.Orange, p.Position, p.Position + 3*p.Velocity);
                //g.DrawLine(Pens.Purple, p.Position, p.Position + 3*p.Acceleration);
                if (k.Force != Vector2.Zero && messages[p] == "Chaser")
                {
                    //g.DrawLine(Pens.Pink, p.Position, p.Position + 3 * k.Force);
                    var playersExceptSelf = Players.ToList();
                    playersExceptSelf.Remove(p);

                    var target = FootballStrategies.ClosestPlayerToPoint(playersExceptSelf, p, 100);
                    var desired = target.Position - p.Position;
                    g.DrawLine(Pens.Gold, p.Position, p.Position + 10 * desired);
                    g.DrawLine(Pens.Fuchsia, simulation.Ball.Position, simulation.Ball.Position + simulation.Ball.Velocity.Projection(desired));
                    g.DrawLine(Pens.Brown, simulation.Ball.Position, simulation.Ball.Position + 10 * simulation.Ball.Velocity.Rejection(desired));

                }

                string message;
                if (messages.TryGetValue(p, out message))
                    g.DrawString(message, SystemFonts.DefaultFont, Brushes.Black, p.Position.X + 10, p.Position.Y + 10);
            }
        }
    }
}