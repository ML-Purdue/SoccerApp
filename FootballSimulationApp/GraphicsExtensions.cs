using System.Drawing;
using System.Numerics;

namespace FootballSimulationApp
{
    internal static class GraphicsExtensions
    {
        public static void DrawRectangle(this Graphics g, Pen pen, RectangleF rect)
            => g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);

        public static void FillEllipse(this Graphics g, Brush brush, Vector2 center, float radiusX, float radiusY)
            => g.FillEllipse(brush, center.X - radiusX/2, center.Y/2 - radiusY, radiusX, radiusY);

        public static void FillCircle(this Graphics g, Brush brush, Vector2 center, float radius)
            => g.FillEllipse(brush, center, radius, radius);

        public static void DrawEllipse(this Graphics g, Pen pen, Vector2 center, float radiusX, float radiusY)
            => g.DrawEllipse(pen, center.X - radiusX/2, center.Y - radiusY/2, radiusX, radiusY);

        public static void DrawCircle(this Graphics g, Pen pen, Vector2 center, float radius)
            => g.DrawEllipse(pen, center, radius, radius);
    }
}