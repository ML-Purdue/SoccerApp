using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FootballSimulationApp
{
    public partial class Form1 : Form
    {
        private Bitmap _backBuffer;
        
        public Form1()
        {
            InitializeComponent();

            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.UserPaint, true);

            var timer = new Timer();
            timer.Interval = 1000/60;
            timer.Tick += Tick;
            timer.Start();
        }

        private void CreateBackBuffer()
        {
            _backBuffer?.Dispose();
            _backBuffer = new Bitmap(ClientSize.Width, ClientSize.Height);
        }

        private void Tick(object sender, EventArgs e)
        {
            // TODO: Simulate game.

            Draw();
        }

        private void Draw()
        {
            if (_backBuffer == null)
                return;

            using (var g = Graphics.FromImage(_backBuffer))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Green);
                g.FillEllipse(Brushes.Red, 100, 100, 10, 10);

                // TODO: Draw pitch, goals, players, ball.
            }

            Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e) => CreateBackBuffer();

        private void Form1_ResizeEnd(object sender, EventArgs e) => CreateBackBuffer();

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (_backBuffer != null)
                e.Graphics.DrawImageUnscaled(_backBuffer, Point.Empty);
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {

            throw new NotImplementedException();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) => new AboutBox().Show();
    }
}