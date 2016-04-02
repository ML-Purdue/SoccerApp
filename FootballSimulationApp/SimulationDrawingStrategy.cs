using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using FootballSimulation;

namespace FootballSimulationApp
{
    internal sealed class SimulationDrawingStrategy : ISimulationDrawingStrategy
    {
        private readonly Brush _ballBrush;
        private readonly Pen _linePen;
        private readonly IEnumerable<Brush> _teamBrushes;
        private readonly IEnumerable<Pen> _teamPens;
        private readonly Font _font;

        /// <summary>
        ///     Initializes a new instance of this class with the specified colors used for drawing the game.
        /// </summary>
        /// <param name="line">The line color.</param>
        /// <param name="ball">The ball color.</param>
        /// <param name="teamColors">
        ///     The colors of each of the teams. Colors should be in order of the <c>Teams</c> property in
        ///     <see cref="ISimulation" />.
        /// </param>
        /// <param name="font">Font to draw debug text.</param>
        public SimulationDrawingStrategy(Color line, Color ball, IList<Color> teamColors, Font font)
        {
            _linePen = new Pen(line);
            _ballBrush = new SolidBrush(ball);
            _teamPens = from c in teamColors select new Pen(c);
            _teamBrushes = from c in teamColors select new SolidBrush(c);
            _font = font;
        }

        /// <summary>
        ///     Disposes the pens and brushes created by this class.
        /// </summary>
        public void Dispose()
        {
            _linePen.Dispose();
            _ballBrush.Dispose();
            _teamPens.ForEach(p => p.Dispose());
            _teamBrushes.ForEach(b => b.Dispose());
        }

        /// <summary>
        ///     Draws the simualation with the specified colors.
        /// </summary>
        /// <param name="g">The graphics context.</param>
        /// <param name="s">The simulation to draw.</param>
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
            g.DrawEllipse(_linePen, -w / 10, -w / 10, w / 5, w / 5);
            g.DrawRectangle(_linePen, s.PitchBounds);
            g.DrawRectangle(_linePen, s.Teams[0].GoalBounds);
            g.DrawRectangle(_linePen, s.Teams[1].GoalBounds);
        }

        private static void DrawPointMass(IPointMass pointMass, Pen pen, Brush brush, Graphics g)
        {
            var triangle = new[] { new PointF(1, 0), new PointF(-0.7f, 0.7f), new PointF(-0.7f, -0.7f) };

            using (var m = new Matrix())
            {
                m.Translate(pointMass.Position.X, pointMass.Position.Y);
                m.Rotate((float)(Math.Atan2(pointMass.Velocity.Y, pointMass.Velocity.X) * 180 / Math.PI));
                m.Scale(pointMass.Radius*0.9f, pointMass.Radius*0.9f);
                m.TransformPoints(triangle);
            }

            g.FillPolygon(brush, triangle);
            g.DrawCircle(pen, pointMass.Position, pointMass.Radius);
        }

        private static void DrawDebugInfo(IPointMass pointMass, Brush brush, Font font, Graphics g)
        {
        }

        private void DrawTeams(Graphics g, ISimulation s)
        {
            var pe = _teamPens.GetEnumerator();
            var be = _teamBrushes.GetEnumerator();

            foreach (var t in s.Teams)
            {
                pe.MoveNext();
                be.MoveNext();
                t.Players.ForEach(p => {
                    DrawPointMass(p, pe.Current, be.Current, g);
                    DrawDebugInfo(p, be.Current, _font, g);
                });

                if (t is KeepawayTeam)
                    ((KeepawayTeam)t).DrawDebugInfo(g);
            }
        }

        private void DrawBall(Graphics g, ISimulation s)
        {
            g.FillCircle(_ballBrush, s.Ball.Position, s.Ball.Radius);
            DrawDebugInfo(s.Ball, _ballBrush, _font, g);
        }
    }
}