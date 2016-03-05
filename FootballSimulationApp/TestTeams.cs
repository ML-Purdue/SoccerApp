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
        public string Name => "Team Strategy A";
        bool hasKicked = false;
        int kickCounter = 0;

        public TeamA(ReadOnlyCollection<PointMass> players, RectangleF goalBounds) :
            base(players, goalBounds)
        {
        }

        public override Kick Execute(ISimulation simulation)
        {
            //foreach (var p in team.Players)
            Console.WriteLine(Players[0].Velocity.X);
            Console.WriteLine(Players[0].Velocity.Y);
            Console.WriteLine();
            if (kickCounter < 34)
            {
                var p = Players[0];
                var force = SteeringStrategies.Seek(p, Vector2.Zero, 50);
                // var force = SteeringStrategies.Arrive(p, Vector2.Zero,p.MaxSpeed, 250);
                p.Force = force;
                if ((p.Position - Vector2.Zero).Length() < Math.Sqrt(p.Radius))
                {
                    kickCounter++;
                    return new Kick(p, new Vector2(-1000, 0));
                }

            }
            else
            {
                var p = Players[0];
                var force = SteeringStrategies.SlowDown(p);

                p.Force = force;
            }
            Console.WriteLine(kickCounter);
            return Kick.None;
        }
    }
}

