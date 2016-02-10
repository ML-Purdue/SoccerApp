using System.Drawing;
using System.Numerics;

namespace FootballSimulationApp
{
    internal static class GraphicsExtensions
    {
        public static void DrawRectangle(this Graphics g, Pen pen, RectangleF rect)
            => g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);

        public static void FillEllipse(this Graphics g, Brush brush, Vector2 center, float radiusX, float radiusY)
            => g.FillEllipse(brush, center.X - radiusX, center.Y - radiusY, radiusX, radiusY);

        public static void FillCircle(this Graphics g, Brush brush, Vector2 center, float radius)
            => FillEllipse(g, brush, center, radius, radius);
    }
}