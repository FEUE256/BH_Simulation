using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BH_Simulation
{
    public partial class MainWindow : Window
    {
        private List<Particle> particles = new();
        private DispatcherTimer timer = new DispatcherTimer();
        private const double BH_X = 400;   // Black hole center X
        private const double BH_Y = 300;   // Black hole center Y
        private const double BH_MASS = 5000;
        private const double BH_RADIUS = 50;

        public MainWindow()
        {
            InitializeComponent();
            InitParticles();
            timer.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
            timer.Tick += Timer_Tick;
        }

        private void InitParticles()
        {
            var rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                double x = rand.NextDouble() * 800;
                double y = rand.NextDouble() * 600;
                double vx = (rand.NextDouble() - 0.5) * 2;
                double vy = (rand.NextDouble() - 0.5) * 2;
                double charge = rand.NextDouble() > 0.5 ? 1 : -1;
                particles.Add(new Particle(x, y, vx, vy, charge));
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            FieldCanvas.Children.Clear();

            foreach (var p in particles)
            {
                ApplyGravity(p);
                ApplyWongEffect(p);

                // Draw particle
                var ellipse = new Ellipse
                {
                    Width = Math.Max(2, p.Energy),
                    Height = Math.Max(2, p.Energy),
                    Fill = p.Charge > 0 ? Brushes.Red : Brushes.Blue
                };

                Canvas.SetLeft(ellipse, p.X);
                Canvas.SetTop(ellipse, p.Y);
                FieldCanvas.Children.Add(ellipse);
            }

            // Draw black hole
            var bh = new Ellipse
            {
                Width = BH_RADIUS * 2,
                Height = BH_RADIUS * 2,
                Fill = Brushes.Black
            };
            Canvas.SetLeft(bh, BH_X - BH_RADIUS);
            Canvas.SetTop(bh, BH_Y - BH_RADIUS);
            FieldCanvas.Children.Add(bh);
        }

        private void ApplyGravity(Particle p)
        {
            double dx = BH_X - p.X;
            double dy = BH_Y - p.Y;
            double r2 = dx * dx + dy * dy;
            double force = BH_MASS / Math.Max(r2, 0.01);

            double r = Math.Sqrt(r2);
            double fx = dx / r * force;
            double fy = dy / r * force;

            p.VX += fx * 0.016; // dt ~16ms
            p.VY += fy * 0.016;

            p.X += p.VX;
            p.Y += p.VY;
        }

        private void ApplyWongEffect(Particle p)
        {
            double dx = BH_X - p.X;
            double dy = BH_Y - p.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);

            if (distance < BH_RADIUS * 2)
            {
                p.VX *= 1.5;
                p.VY *= 1.5;
                p.Charge *= -1;   // Abstract 5D effect
                p.Energy *= 0.8;  // Energy distortion
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }
    }

    public class Particle
    {
        public double X, Y;
        public double VX, VY;
        public double Charge;
        public double Energy;

        public Particle(double x, double y, double vx, double vy, double charge)
        {
            X = x; Y = y;
            VX = vx; VY = vy;
            Charge = charge;
            Energy = 4;
        }
    }
}
