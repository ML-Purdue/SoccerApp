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
        private const int EdgeOverlapRadius = 100;
        private const int PlayerOverlapRadius = 150;
        private const float StoppingMultiplier = -1f;

        public KeepawayTeam(ReadOnlyCollection<PointMass> players, RectangleF goalBounds) :
            base(players, goalBounds)
        {
        }

        public override Kick Execute(ISimulation simulation)
        {
            var allPlayers = simulation.Teams[0].Players.Concat(simulation.Teams[1].Players);
            var ballChaser = ClosestToPoint(Players, simulation.Ball.Position);
            Kick kick = Kick.None;

            foreach (var player in Players)
            {
                if (player == ballChaser)
                    kick = ChaseBall(player, simulation.Ball);
                else
                    SpreadOut(player, allPlayers, simulation.PitchBounds);
            }

            return kick;
        }

        private Kick ChaseBall(PointMass player, IPointMass ball)
        {
            player.Force = SteeringStrategies.Pursue(player, (PointMass)ball, 100);
            if ((player.Position - ball.Position).Length() < 20)
                return new Kick(player, new Vector2(100, 0));
            return Kick.None;
        }

        private IPointMass ClosestToPoint(IEnumerable<IPointMass> players, Vector2 point)
        {
            var closest = players.First();
            var len = (closest.Position - point).Length();

            players.ForEach(p =>
            {
                var l = (p.Position - point).Length();
                if (l < len)
                {
                    closest = p;
                    len = l;
                }
            });

            return closest;
        }

        private void SpreadOut(PointMass player, IEnumerable<IPointMass> allPlayers, RectangleF pitchBounds)
        {
            Vector2 v = Vector2.Zero;

            foreach (var otherPlayer in allPlayers)
            {
                if (player == otherPlayer)
                    continue;

                var between = player.Position - otherPlayer.Position;
                if (between.Length() < PlayerOverlapRadius)
                    v += Vector2.Normalize(between);
            }

            var force = Vector2.Normalize(v) * 10;
            var stoppingForce = StoppingMultiplier * player.Velocity;

            force = v.Length() > 0 ? force : stoppingForce;

            if (Math.Abs(player.Position.X - pitchBounds.X) < EdgeOverlapRadius && player.Velocity.X < 0)
                force = new Vector2(StoppingMultiplier * player.Velocity.X, force.Y);
            if (Math.Abs(player.Position.X - pitchBounds.Right) < EdgeOverlapRadius && player.Velocity.X > 0)
                force = new Vector2(StoppingMultiplier * player.Velocity.X, force.Y);
            if (Math.Abs(player.Position.Y - pitchBounds.Y) < EdgeOverlapRadius && player.Velocity.Y < 0)
                force = new Vector2(force.X, StoppingMultiplier * player.Velocity.Y);
            if (Math.Abs(player.Position.Y - pitchBounds.Bottom) < EdgeOverlapRadius && player.Velocity.Y > 0)
                force = new Vector2(force.X, StoppingMultiplier * player.Velocity.Y);

            player.Force = force;
        }
    }
}
