using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FootballSimulation;

namespace FootballSimulationApp
{
    internal partial class MainForm : Form
    {
        private readonly Color _backgroundColor = Color.Green;
        private readonly TimeSpan _maxElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond/10);
        private readonly Simulation _simulation;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly TimeSpan _targetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond/60);
        private TimeSpan _accumulatedTime;
        private Bitmap _backBuffer;
        private TimeSpan _lastTime;

        public MainForm()
        {
            InitializeComponent();

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer, true);

            Application.Idle += OnIdle;

            const float w = 1000;
            const float h = 500;
            const float goalW = w / 20;
            const float goalH = h / 4;

            var team1Players = new PointMass[5];
            var team2Players = new PointMass[5];
            
            for (var j = 0; j < 5; j++)
            {
                team1Players[j] = new PointMass(100, 10, 100, 100,
                    new Vector2(-w/4 + w/2, -h/4 + j*h/8), Vector2.Zero);
                team2Players[j] = new PointMass(100, 10, 100, 100,
                    new Vector2(-w / 4, -h / 4 + j * h / 8), Vector2.Zero);
            }
            
            var pitch = new RectangleF(-w/2, -h/2, w, h);
            var team1Goal = new RectangleF(-w/2 - goalW, -goalH/2, goalW, goalH);
            var team2Goal = new RectangleF(w/2, -goalH/2, goalW, goalH);
            var team1 = new Team(NullTeamStrategy.Instance, new ReadOnlyCollection<PointMass>(team1Players), team1Goal);
            var team2 = new Team(NullTeamStrategy.Instance, new ReadOnlyCollection<PointMass>(team2Players), team2Goal);
            var ball = new PointMass(1, 5, 100, 100, Vector2.Zero, Vector2.Zero);

            _simulation = new Simulation(new ReadOnlyCollection<Team>(new[] {team1, team2}), ball, pitch, 0.05f);
        }

        private static bool IsMessageAvailable
        {
            get
            {
                NativeMethods.Message message;
                return NativeMethods.PeekMessage(out message, IntPtr.Zero, 0, 0, 0);
            }
        }

        private void Update(TimeSpan elapsedTime)
        {
            _simulation.Simulate((float) elapsedTime.TotalSeconds);
        }

        private static void DrawTeam(ITeam t, Brush brush, Graphics g)
        {
            foreach (var p in t.Players)
            {
                g.FillCircle(brush, p.Position, p.Radius);
                var norm = Vector2.Normalize(p.Velocity);
                g.DrawLine(Pens.Black, p.Position.X, p.Position.Y, p.Position.X + norm.X, p.Position.Y + norm.Y);
            }
        }

        private void Draw(Graphics g)
        {
            var innerAspectRatio = _simulation.PitchBounds.Width/_simulation.PitchBounds.Height;
            var outerAspectRatio = (float) ClientSize.Width/ClientSize.Height;
            var resizeFactor = 0.8f*(innerAspectRatio >= outerAspectRatio
                ? ClientSize.Width/_simulation.PitchBounds.Width
                : ClientSize.Height/_simulation.PitchBounds.Height);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(_backgroundColor);
            g.TranslateTransform(ClientSize.Width/2f, ClientSize.Height/2f);
            g.ScaleTransform(resizeFactor, resizeFactor);
            DrawTeam(_simulation.Teams[0], Brushes.Red, g);
            DrawTeam(_simulation.Teams[1], Brushes.Blue, g);
            g.DrawRectangle(Pens.White, _simulation.PitchBounds);
            g.DrawRectangle(Pens.White, _simulation.Teams[0].GoalBounds);
            g.DrawRectangle(Pens.White, _simulation.Teams[1].GoalBounds);
            var w = _simulation.PitchBounds.Width;
            g.DrawEllipse(Pens.White, -w/10, -w/10, w/5, w/5);
            g.FillCircle(Brushes.Magenta, _simulation.Ball.Position + new Vector2(_simulation.Ball.Radius/2),
                _simulation.Ball.Radius);
        }

        private void OnIdle(object sender, EventArgs e)
        {
            while (!IsMessageAvailable)
                OnTick();
        }

        private TimeSpan GetElapsedTime()
        {
            var currentTime = _stopwatch.Elapsed;
            var elapsedTime = currentTime - _lastTime;

            _lastTime = currentTime;
            return elapsedTime > _maxElapsedTime ? _maxElapsedTime : elapsedTime;
        }

        private void OnTick()
        {
            if ((_accumulatedTime += GetElapsedTime()) < _targetElapsedTime)
                return;

            Update(_targetElapsedTime);
            while ((_accumulatedTime -= _targetElapsedTime) >= _targetElapsedTime)
                Update(_targetElapsedTime);

            Invalidate();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _backBuffer = new Bitmap(ClientSize.Width, ClientSize.Height);
            _stopwatch.Start();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            _backBuffer?.Dispose();
            _backBuffer = new Bitmap(ClientSize.Width, ClientSize.Height);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            using (var g = Graphics.FromImage(_backBuffer))
                Draw(g);

            e.Graphics.DrawImageUnscaled(_backBuffer, Point.Empty);
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) => new AboutBox().Show();

        private static class NativeMethods
        {
            [DllImport("User32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool PeekMessage(
                out Message message,
                IntPtr hWnd,
                uint filterMin,
                uint filterMax,
                uint flags);

            [StructLayout(LayoutKind.Sequential)]
            public struct Message
            {
                private readonly IntPtr hWnd;
                private readonly uint Msg;
                private readonly IntPtr wParam;
                private readonly IntPtr lParam;
                private readonly uint Time;
                private readonly Point Point;
            }
        }
    }
}