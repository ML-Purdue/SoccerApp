using System.Numerics;
using FootballSimulation;

namespace FootballSimulationApp
{
    public class TeamStrategyA : ITeamStrategy
    {
        public string Name => "Team Strategy A";

        public Kick Execute(ISimulation simulation, Team team)
        {
            //foreach (var p in team.Players)
            {
                var p = team.Players[0];
                var force = SteeringStrategies.Seek(p, Vector2.Zero, 50);
                p.SetForce(force);
                if ((p.Position - Vector2.Zero).Length() < p.Radius)
                    return new Kick(p, new Vector2(-100, 0));
            }

            return Kick.None;
        }
    }
}

