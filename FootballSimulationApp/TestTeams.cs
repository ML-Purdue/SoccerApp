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
        private int v = 0;
        private readonly Dictionary<IPointMass, string> messages = new Dictionary<IPointMass, string>();
        private static Kick k;

        public KeepawayTeam(ReadOnlyCollection<PointMass> players, RectangleF goalBounds) :
            base(players, goalBounds)
        {
        }

        private static float DistanceBetween(Vector2 a, Vector2 b)
        {
            return Vector2.Distance(a, b);
        }

        private static IPointMass ClosestPlayerToPoint(IEnumerable<IPointMass> players, IPointMass target, float max, Vector2 goalMiddle)
        {
            return players.OrderBy(p => (p.Position - SteeringStrategies.FuturePosition(p, target, max)).LengthSquared() + DistanceBetween(p.Position, goalMiddle)).First();
        }

        private static bool isOutsideOfField(PointMass player, RectangleF field)
        {
            if (player.Position.Y > field.Top && player.Position.Y < field.Bottom && player.Position.X > field.Left && player.Position.X < field.Right)
                return false;
            else
                return true;
        }

        public override Kick Execute(ISimulation simulation)
        {
            var ballChaser = FootballStrategies.ClosestPlayerToPoint(this.Players, simulation.Ball, 0);
            var kick = Kick.None;

            foreach (var player in Players)
            {
                if (player == ballChaser)
                {
                    messages[player] = "Chaser";

                    var playersExceptSelf = Players.ToList();
                    playersExceptSelf.Remove(player);

                    player.Force = SteeringStrategies.Pursue(player, simulation.Ball, 1);

                    if ((player.Position - simulation.Ball.Position).Length() < 20) {
                        var isLeftTeam = this.GoalBounds.Left > 0 ? false : true;
                        PointMass[] arr = new PointMass[7];
                        playersExceptSelf.CopyTo(arr, 0);
                        arr[4] = new PointMass(1, 1, 1, 1, new Vector2(isLeftTeam ? this.GoalBounds.Left : this.GoalBounds.Right, this.GoalBounds.Top - (0.2f) * this.GoalBounds.Height), Vector2.Zero);
                        arr[5] = new PointMass(1, 1, 1, 1, new Vector2(isLeftTeam ? this.GoalBounds.Left : this.GoalBounds.Right, this.GoalBounds.Top - (0.5f) * this.GoalBounds.Height), Vector2.Zero);
                        arr[6] = new PointMass(1, 1, 1, 1, new Vector2(isLeftTeam ? this.GoalBounds.Left : this.GoalBounds.Right, this.GoalBounds.Top - (0.8f) * this.GoalBounds.Height), Vector2.Zero);
                        ReadOnlyCollection<PointMass> roc = new ReadOnlyCollection<PointMass>(arr);
                        Vector2 middleOfGoal = new Vector2(isLeftTeam ? this.GoalBounds.Left : this.GoalBounds.Right, this.GoalBounds.Top - (0.5f) * this.GoalBounds.Height);
                        k = kick = FootballStrategies.PassToPlayer(player, ClosestPlayerToPoint(roc, player, 100, middleOfGoal), simulation.Ball);
                    }  else
                        k = kick = Kick.None;
                }
                else
                {
                    messages[player] = isOutsideOfField(player, simulation.PitchBounds) ? "Outside" : "inside";

                    var allPlayers = simulation.Teams[0].Players.Concat(simulation.Teams[1].Players);
                    if (isOutsideOfField(player, simulation.PitchBounds))
                        SteeringStrategies.Arrive(player, Vector2.Zero, 10000000, 1);
                    else
                        FootballStrategies.SpreadOut(player, allPlayers, simulation.PitchBounds, 150, 100);
                }
            }

            return kick;
        }

        public override void DrawDebugInfo(ISimulation simulation, Graphics g)
        {
            //g.DrawLine(Pens.GhostWhite, simulation.Ball.Position, simulation.Ball.Position + simulation.Ball.Velocity);
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
                    //g.DrawLine(Pens.Gold, p.Position, p.Position + 10 * desired);
                    //g.DrawLine(Pens.Fuchsia, simulation.Ball.Position, simulation.Ball.Position + simulation.Ball.Velocity.Projection(desired));
                    //g.DrawLine(Pens.Brown, simulation.Ball.Position, simulation.Ball.Position + 10 * simulation.Ball.Velocity.Rejection(desired));

                }

                string message;
                if (messages.TryGetValue(p, out message))
                    g.DrawString(message, SystemFonts.DefaultFont, Brushes.Black, p.Position.X + 10, p.Position.Y + 10);
            }
        }
    }
}