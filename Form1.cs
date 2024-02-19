using SkiaSharp.Views.Desktop;
using SkiaSharp;

using System.Windows.Forms;
using System.Diagnostics;

namespace Boids
{
    public partial class Form1 : Form
    {
        static int BOIDS = 200;
        static int BOIDS_SIZE = 5;
        static int OBSTACLES = 50;
        static int OBSTACLE_SIZE = 10;

        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        private SKControl skiaCanvas;
        static int width = 1200;
        static int height = 600;

        private Random randy;
        private List<Boid> boids;
        private Quad_Boids quad;
        private List<Obstacle> obstacles;
        private Quad_Obstacles quad_obstacle;

        // fps
        private Stopwatch stopwatch;
        private int frameCount;
        private double fps;
        public Form1()
        {
            stopwatch = new Stopwatch();
            frameCount = 0;
            fps = 0.0;

            InitializeComponent();
            this.Size = new Size(width + 17, height + 40);
            this.randy = new Random();

            quad = new Quad_Boids(0, width, 0, height, 4);

            this.boids = new List<Boid>();
            Boid boid;
            for (int i = 0;i < BOIDS; i++)
            {
                boid = new Boid(randy.Next(width), randy.Next(height));
                boids.Add(boid);
                quad.add_boid(boid);
            }


            quad_obstacle = new Quad_Obstacles(0, width, 0, height, 4);

            this.obstacles = new List<Obstacle>();
            Obstacle obstacle;
            for (int i = 0; i < OBSTACLES; i++)
            {
                obstacle = new Obstacle(randy.Next(width), randy.Next(height));
                obstacles.Add(obstacle);
                quad_obstacle.add_obstacle(obstacle);
            }

            //Boid boi = new Boid(1,1);

            InitializeSkiaCanvas();
            StartAnimation();
        }

        private void StartAnimation()
        {
            // Uruchom timer do odœwie¿ania ekranu
            myTimer.Interval = 30; // Oko³o 60 klatek na sekundê
            myTimer.Tick += Timer_Tick;
            myTimer.Start();

            stopwatch.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            frameCount++;

            if (stopwatch.ElapsedMilliseconds >= 1000)
            {
                fps = frameCount / (stopwatch.ElapsedMilliseconds / 1000.0);
                frameCount = 0;
                stopwatch.Restart();
            }

            quad = new Quad_Boids(0, width, 0, height, 4);
            foreach (Boid boid in boids)
            {
                quad.add_boid(boid);
            }

            foreach (Boid boid in boids)
            {
                boid.update(quad, quad_obstacle);
            }

            // Odœwie¿ kontrolkê SKControl, aby narysowaæ kwadrat na nowej pozycji
            skiaCanvas.Invalidate();
        }

        private void InitializeSkiaCanvas()
        {
            skiaCanvas = new SKControl();
            skiaCanvas.PaintSurface += SkiaCanvas_PaintSurface;

            skiaCanvas.Dock = DockStyle.Fill;
            Controls.Add(skiaCanvas);

        }

        private void SkiaCanvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            SKPaint paintBoid = new SKPaint
            {
                Color = SKColors.Aquamarine,
                IsAntialias = true
            };

            SKPaint paintQuad = new SKPaint
            {
                Style = SKPaintStyle.Stroke, // Ustaw styl na rysowanie samych konturów
                Color = SKColors.White, // Ustaw kolor na czarny
                StrokeWidth = 1, // Ustaw szerokoœæ linii na 2 piksele

            };

            SKPaint paintPerception = new SKPaint
            {
                Style = SKPaintStyle.Stroke, // Ustaw styl na rysowanie samych konturów
                Color = SKColors.BlueViolet, // Ustaw kolor na czarny
                StrokeWidth = 1, // Ustaw szerokoœæ linii na 2 piksele

            };

            // Wyczyœæ p³ótno
            canvas.Clear(SKColors.Black);

            SKPoint point;

            foreach (Quad_Boids q in quad.getAll()) {

                canvas.DrawRect(q.x1, q.y1, q.x2 - q.x1, q.y2 - q.y1, paintQuad);
            }

            foreach (Boid b in boids) {
                point = new SKPoint(b.position.x,b.position.y);
                canvas.DrawCircle(point, BOIDS_SIZE, paintBoid);
            }

            Boid boid = boids[0];
            canvas.DrawRect(boid.position.x - boid.perception, boid.position.y - boid.perception, 2*boid.perception, 2 * boid.perception, paintPerception);

            paintBoid.Color = SKColors.IndianRed;
            foreach (Obstacle o in obstacles)
            {
                point = new SKPoint(o.x, o.y);
                canvas.DrawCircle(point, OBSTACLE_SIZE, paintBoid);
            }

            // fps
            using (var paint = new SKPaint())
            {
                paint.Color = SKColors.White;
                paint.TextSize = 15;
                canvas.DrawText($"FPS: {fps:F1}", 10, 30, paint);
            }

        }
    }
}