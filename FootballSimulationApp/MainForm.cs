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

            var w = ClientSize.Width;
            var h = ClientSize.Height;
            var players = new PointMass[2][];

            for (var i = 0; i < 2; i++)
            {
                players[i] = new PointMass[5];
                for (var j = 0; j < 5; j++)
                {
                    players[i][j] = new PointMass(100, 10, 100, 100,
                        new Vector2(w/4 + i*w/2, h/10*3 + j*h/10),
                        Vector2.Zero);
                }
            }

            var goalW = w/20;
            var goalH = h/4;
            var pitch = new RectangleF(0, 0, w, h);
            var team1Goal = new RectangleF(-goalW, h/2 - goalH/2, goalW, goalH);
            var team2Goal = new RectangleF(w, h/2 - goalH/2, goalW, goalH);
            var team1 = new Team(NullTeamStrategy.Instance, new ReadOnlyCollection<PointMass>(players[0]), team1Goal);
            var team2 = new Team(NullTeamStrategy.Instance, new ReadOnlyCollection<PointMass>(players[1]), team2Goal);
            var ball = new PointMass(1, 5, 100, 100, new Vector2(w, h)/2, Vector2.Zero);

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
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(_backgroundColor);
            g.ScaleTransform(0.8f, 0.8f);
            g.TranslateTransform(100, 50);
            DrawTeam(_simulation.Teams[0], Brushes.Red, g);
            DrawTeam(_simulation.Teams[1], Brushes.Blue, g);
            g.DrawRectangle(Pens.White, _simulation.PitchBounds);
            g.DrawRectangle(Pens.White, _simulation.Teams[0].GoalBounds);
            g.DrawRectangle(Pens.White, _simulation.Teams[1].GoalBounds);
            g.FillCircle(Brushes.Magenta, _simulation.Ball.Position, _simulation.Ball.Radius);
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