using System;
using System.Drawing;
using System.Windows.Forms;

namespace FootballSimulationApp
{
    public partial class Form1 : Form
    {
        private readonly Bitmap _backBuffer;

        public Form1()
        {
            InitializeComponent();

            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.UserPaint, true);

            _backBuffer = new Bitmap(ClientSize.Width, ClientSize.Height);

            var timer = new Timer();
            timer.Interval = 1000/60;
            timer.Tick += Tick;
            timer.Start();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(_backBuffer, Point.Empty);
        }

        private void Tick(object sender, EventArgs e)
        {
            // TODO: Simulate game.

            Draw();
        }

        private void Draw()
        {
            using (var g = Graphics.FromImage(_backBuffer))
            {
                g.Clear(Color.Green);
                g.FillEllipse(Brushes.Red, 100, 100, 10, 10);

                // TODO: Draw pitch, goals, players, ball.
            }

            Invalidate();
        }
    }
}