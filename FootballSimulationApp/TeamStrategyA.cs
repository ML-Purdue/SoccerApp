using System.Numerics;
using FootballSimulation;
using System;

namespace FootballSimulationApp
{
    public class TeamStrategyA : ITeamStrategy
    {
        public string Name => "Team Strategy A";
        bool hasKicked = false;
        int kickCounter = 0;
        public Kick Execute(ISimulation simulation, Team team)
        {
            //foreach (var p in team.Players)
            Console.WriteLine(team.Players[0].Velocity.X);
            Console.WriteLine(team.Players[0].Velocity.Y);
            Console.WriteLine();
            if (kickCounter < 34)
            {
                var p = team.Players[0];
                var force = SteeringStrategies.Seek(p, Vector2.Zero, 50);
               // var force = SteeringStrategies.Arrive(p, Vector2.Zero,p.MaxSpeed, 250);
                p.SetForce(force);
                if ((p.Position - Vector2.Zero).Length() < Math.Sqrt(p.Radius))
                {
                    kickCounter++;
                    return new Kick(p, new Vector2(-100, 0));
                }

            }
          else
            {
                var p = team.Players[0];
                var force = SteeringStrategies.SlowDown(p);
            
               p.SetForce(force);
            }
            Console.WriteLine(kickCounter);
            return Kick.None;
        }
    }
}

