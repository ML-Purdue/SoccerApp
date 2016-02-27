using System.Drawing;
using System.Numerics;

namespace FootballSimulationApp
{
    internal static class GraphicsExtensions
    {
        /// <summary>
        ///     Draws a rectangle.
        /// </summary>
        /// <param name="g">The drawing surface.</param>
        /// <param name="pen">Pen that determines the color, width, and style of the rectangle.</param>
        /// <param name="rect"></param>
        public static void DrawRectangle(this Graphics g, Pen pen, RectangleF rect)
            => g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);

        /// <summary>
        ///     Fills the interior of an ellipse defined by a center point and the radius on the x and y axes.
        /// </summary>
        /// <param name="g">The drawing surface.</param>
        /// <param name="brush">Brush that determines the characteristics of the fill.</param>
        /// <param name="center">The center point of the ellipse.</param>
        /// <param name="radiusX">The radius on the x axis.</param>
        /// <param name="radiusY">The radius on the y axis.</param>
        public static void FillEllipse(this Graphics g, Brush brush, Vector2 center, float radiusX, float radiusY)
            => g.FillEllipse(brush, center.X - radiusX/2, center.Y/2 - radiusY, radiusX, radiusY);

        /// <summary>
        ///     Fills the interior of a circle defined by a center point and the radius.
        /// </summary>
        /// <param name="g">The drawing surface.</param>
        /// <param name="brush">Brush that determines the characteristics of the fill.</param>
        /// <param name="center">The center point of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        public static void FillCircle(this Graphics g, Brush brush, Vector2 center, float radius)
            => g.FillEllipse(brush, center, radius, radius);

        /// <summary>
        ///     Draws an ellipse defined by a center point and the radius on the x and y axes.
        /// </summary>
        /// <param name="g">The drawing surface.</param>
        /// <param name="pen">Pen that determines the color, width, and style of the ellipse.</param>
        /// <param name="center">The center point of the ellipse.</param>
        /// <param name="radiusX">The radius on the x axis.</param>
        /// <param name="radiusY">The radius on the y axis.</param>
        public static void DrawEllipse(this Graphics g, Pen pen, Vector2 center, float radiusX, float radiusY)
            => g.DrawEllipse(pen, center.X - radiusX/2, center.Y - radiusY/2, radiusX, radiusY);

        /// <summary>
        ///     Draws a circle defined by a center point and the radius.
        /// </summary>
        /// <param name="g">The drawing surface.</param>
        /// <param name="pen">Pen that determines the color, width, and style of the circle.</param>
        /// <param name="center">The center point of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        public static void DrawCircle(this Graphics g, Pen pen, Vector2 center, float radius)
            => g.DrawEllipse(pen, center, radius, radius);
    }
}