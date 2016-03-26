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

            foreach (var player in Players)
            {
                Vector2 v = new Vector2();

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

                force = v.Length() > 0 ? force :stoppingForce;

                if (Math.Abs(player.Position.X - simulation.PitchBounds.X) < EdgeOverlapRadius && player.Velocity.X < 0)
                    force = new Vector2(StoppingMultiplier * player.Velocity.X, force.Y);
                if (Math.Abs(player.Position.X - simulation.PitchBounds.Right) < EdgeOverlapRadius && player.Velocity.X > 0)
                    force = new Vector2(StoppingMultiplier * player.Velocity.X, force.Y);
                if (Math.Abs(player.Position.Y - simulation.PitchBounds.Y) < EdgeOverlapRadius && player.Velocity.Y < 0)
                    force = new Vector2(force.X, StoppingMultiplier * player.Velocity.Y);
                if (Math.Abs(player.Position.Y - simulation.PitchBounds.Bottom) < EdgeOverlapRadius && player.Velocity.Y > 0)
                    force = new Vector2(force.X, StoppingMultiplier * player.Velocity.Y);

                player.Force = force;
            }

            return Kick.None;
        }
    }
}
