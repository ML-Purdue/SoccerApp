using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using FootballSimulation;

namespace FootballSimulationApp
{
    internal sealed class SimulationDrawingStrategy : ISimulationDrawingStrategy, IDisposable
    {
        private readonly Brush _ballBrush;
        private readonly Pen _linePen;
        private readonly IEnumerable<Brush> _teamBrushes;
        private readonly IEnumerable<Pen> _teamPens;

        public SimulationDrawingStrategy(Color line, Color ball, IList<Color> teamColors)
        {
            _linePen = new Pen(line);
            _ballBrush = new SolidBrush(ball);
            _teamPens = from c in teamColors select new Pen(c);
            _teamBrushes = from c in teamColors select new SolidBrush(c);
        }

        public void Dispose()
        {
            _linePen.Dispose();
            _ballBrush.Dispose();
            _teamPens.ForEach(p => p.Dispose());
            _teamBrushes.ForEach(b => b.Dispose());
        }

        public void Draw(Graphics g, ISimulation s)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            DrawPitch(g, s);
            DrawTeams(g, s);
            DrawBall(g, s);
        }

        private void DrawPitch(Graphics g, ISimulation s)
        {
            var w = s.PitchBounds.Width;
            g.DrawEllipse(_linePen, -w/10, -w/10, w/5, w/5);
            g.DrawRectangle(_linePen, s.PitchBounds);
            g.DrawRectangle(_linePen, s.Teams[0].GoalBounds);
            g.DrawRectangle(_linePen, s.Teams[1].GoalBounds);
        }

        private static void DrawPointMass(IPointMass pointMass, Pen pen, Brush brush, Graphics g)
        {
            var triangle = new[] {new PointF(1, 0), new PointF(-0.7f, 0.7f), new PointF(-0.7f, -0.7f)};

            using (var m = new Matrix())
            {
                m.Translate(pointMass.Position.X, pointMass.Position.Y);
                m.Rotate((float) Math.Atan2(pointMass.Velocity.Y, pointMass.Velocity.X));
                m.Scale(pointMass.Radius/2, pointMass.Radius/2);
                m.TransformPoints(triangle);
            }

            g.FillPolygon(brush, triangle);
            g.DrawCircle(pen, pointMass.Position, pointMass.Radius);
        }

        private void DrawTeams(Graphics g, ISimulation s)
        {
            var pe = _teamPens.GetEnumerator();
            var be = _teamBrushes.GetEnumerator();

            foreach (var t in s.Teams)
            {
                pe.MoveNext();
                be.MoveNext();
                t.Players.ForEach(p => DrawPointMass(p, pe.Current, be.Current, g));
            }
        }

        private void DrawBall(Graphics g, ISimulation s) => g.FillCircle(_ballBrush, s.Ball.Position, s.Ball.Radius);
    }
}