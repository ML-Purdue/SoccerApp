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
        private readonly TimeSpan _maxElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond/10);
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly TimeSpan _targetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond/60);
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

            for (int i = 0; i < 2; i++)
            {
                players[i] = new PointMass[5];
                for (int j = 0; j < 5; j++)
                {
                    // (float mass, float radius, float maxForce, float maxSpeed, Vector2 position, Vector2 velocity)
                    Vector2 position = new Vector2();
                    position.X = 300 + 100 * j;
                    position.Y = 200 + 600 * i;

                    Vector2 velocity = new Vector2();
                    velocity.X = 0;
                    velocity.Y = 0;

                    players[i][j] = new PointMass(100, 10, 100, 100, position, velocity);
                }
            }
            /*
            // Fix if needed (coordinate system)
            RectangleF pitch = new RectangleF(0, 0, 4200, 6600);
            RectangleF team1Goal = new RectangleF(2100 - 240, 0, 480 , 160);
            RectangleF team2Goal = new RectangleF(2100 - 240, 6600, 480, 160);

            Team1 = new Team(NullTeamStrategy.Instance, new ReadOnlyCollection<PointMass>(players[0]), team1Goal);
            Team2 = new Team(NullTeamStrategy.Instance, new ReadOnlyCollection<PointMass>(players[1]), team2Goal);

            PointMass ball = new PointMass(10, 5, 100, 100, new Vector2(2100, 3300), new Vector2(0, 0));

            Simulation = new Simulation(new ReadOnlyCollection<Team>(new Team[]{ Team1, Team2 }), ball, pitch); */

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
            // TODO: Add update code.
        }

        private void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(_backgroundColor);

            // TODO: Add drawing code.
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