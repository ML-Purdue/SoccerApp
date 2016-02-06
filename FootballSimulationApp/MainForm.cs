using System;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FootballSimulation;

namespace FootballSimulationApp
{
    internal partial class MainForm : Form
    {
        private readonly Color _backgroundColor = Color.Green;
        private readonly TimeSpan _maxElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 10);
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly TimeSpan _targetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 60);
        private TimeSpan _accumulatedTime;
        private Bitmap _backBuffer;
        private TimeSpan _lastTime;
        private Simulation Simulation;
        private Team Team1;
        private Team Team2;

        public MainForm()
        {
            InitializeComponent();

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer, true);

            Application.Idle += OnIdle;

            PointMass[][] players = new PointMass[2][];

            int windowW = ClientSize.Width;
            int windowH = ClientSize.Height;

            for (int i = 0; i < 2; i++)
            {
                players[i] = new PointMass[5];
                for (int j = 0; j < 5; j++)
                {
                    // (float mass, float radius, float maxForce, float maxSpeed, Vector2 position, Vector2 velocity)
                    Vector2 position = new Vector2();
                    position.X = windowW / 4 + i * windowW / 2;
                    position.Y = windowH / 10 * 3 + j * windowH / 10;

                    players[i][j] = new PointMass(100, 10, 100, 100, position, Vector2.Zero);
                }
            }

            // Fix if needed (coordinate system)

            int goalW = windowW / 20;
            int goalH = windowH / 4;
            RectangleF pitch = new RectangleF(0, 0, windowW, windowH);
            RectangleF team1Goal = new RectangleF(-goalW, windowH / 2 - goalH / 2, goalW, goalH);
            RectangleF team2Goal = new RectangleF(windowW, windowH / 2 - goalH / 2, goalW, goalH);

            Team1 = new Team(NullTeamStrategy.Instance, new ReadOnlyCollection<PointMass>(players[0]), team1Goal);
            Team2 = new Team(NullTeamStrategy.Instance, new ReadOnlyCollection<PointMass>(players[1]), team2Goal);

            PointMass ball = new PointMass(1, 5, 100, 100, new Vector2(windowW / 2, windowH / 2), Vector2.Zero);

            Simulation = new Simulation(new ReadOnlyCollection<Team>(new Team[] { Team1, Team2 }), ball, pitch, 0.05f);
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
            Simulation.Simulate((float)elapsedTime.TotalSeconds);
        }

        private static Rectangle GetBoundingRect(IPointMass pointMass)
        {
            var p = pointMass.Position;
            var r = pointMass.Radius;
            var result = new Rectangle((int)(p.X - r), (int)(p.Y - r), (int)r, (int)r);
            return result;
        }

        private static Rectangle ToRectangle(RectangleF r)
        {
            return new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
        }

        private void DrawTeam(Team t, Brush brush, Graphics g)
        {
            foreach (var p in t.Players)
            {
                g.FillEllipse(brush, GetBoundingRect(p));
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
            DrawTeam(Team1, Brushes.Red, g);
            DrawTeam(Team2, Brushes.Blue, g);
            g.DrawRectangle(Pens.White, ToRectangle(Simulation.PitchBounds));
            g.DrawRectangle(Pens.White, ToRectangle(Simulation.Teams[0].GoalBounds));
            g.DrawRectangle(Pens.White, ToRectangle(Simulation.Teams[1].GoalBounds));
            g.FillEllipse(Brushes.Magenta, GetBoundingRect(Simulation.Ball));
        }

        private void OnIdle(object sender, EventArgs e)
        {
            while (!IsMessageAvailable)
                OnTick();
        }

        private void OnTick()
        {
            var currentTime = _stopwatch.Elapsed;
            var elapsedTime = currentTime - _lastTime;

            _lastTime = currentTime;
            _accumulatedTime += elapsedTime > _maxElapsedTime ? _maxElapsedTime : elapsedTime;
            if (_accumulatedTime < _targetElapsedTime)
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