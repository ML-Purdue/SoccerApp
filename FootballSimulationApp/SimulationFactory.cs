using System.Collections.ObjectModel;
using System.Drawing;
using System.Numerics;
using FootballSimulation;

namespace FootballSimulationApp
{
    public static class SimulationFactory
    {
        public static Simulation Create2V2Simulation()
        {
            const float w = 1000;
            const float h = 500;
            const float goalW = w / 20;
            const float goalH = h / 4;
            const float mass = 1;
            const float radius = 20;
            const float maxForce = 100;
            const float maxSpeed = 100;

            var team1Players = new PointMass[5];
            var team2Players = new PointMass[5];

            for (var j = 0; j < 5; j++)
            {
                team1Players[j] = new PointMass(mass, radius, maxForce, maxSpeed,
                    new Vector2(-w / 4 + w / 2, -h / 4 + j * h / 8), Vector2.Zero);
                team2Players[j] = new PointMass(mass, radius, maxForce, maxSpeed,
                    new Vector2(-w / 4, -h / 4 + j * h / 8), Vector2.Zero);
            }

            var pitch = new RectangleF(-w / 2, -h / 2, w, h);
            var team1Goal = new RectangleF(-w / 2 - goalW, -goalH / 2, goalW, goalH);
            var team2Goal = new RectangleF(w / 2, -goalH / 2, goalW, goalH);
            var team1 = new Team(NullTeamStrategy.Instance, new ReadOnlyCollection<PointMass>(team1Players), team1Goal);
            var team2 = new Team(NullTeamStrategy.Instance, new ReadOnlyCollection<PointMass>(team2Players), team2Goal);
            var ball = new PointMass(1, 5, 100, 100, Vector2.Zero, Vector2.Zero);

            return new Simulation(new ReadOnlyCollection<Team>(new[] { team1, team2 }), ball, pitch, 0.05f);
        }
     }
}