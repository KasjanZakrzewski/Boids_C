using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boids
{
    
    internal class Boid
    {
        static int WIDTH = 1200;
        static int HEIGHT = 600;
        static int PERCEPTION = 50;
        static int MAX_SPEED = 3;
        static int MAX_IN_VIEW = 7;
        static float SEPARATION = PERCEPTION/2;
        static int BOUND = 25;


        public int perception;
        public Vector position;
        private Vector velocity;

        public Boid(int x, int y) { 
            Random randy = new Random();
            
            this.perception = PERCEPTION;
            this.position = new Vector(x, y);
            this.velocity = new Vector((float)(randy.NextDouble()-0.5)*4, (float)(randy.NextDouble() - 0.5) * 4);
            this.velocity.mag(MAX_SPEED);

        }

        public void update(Quad_Boids quad, Quad_Obstacles quad_obstacles)
        {
            this.position.add(this.velocity);
            this.steer(quad, quad_obstacles);
            outside();
        }

        private void steer(Quad_Boids quad, Quad_Obstacles quad_obstacles)
        {
            List<Boid> boids;
            boids = quad.find_in_square(position.x - perception,
                                       position.x + perception,
                                       position.y - perception,
                                       position.y + perception);

            Vector avg_vel = new Vector(0, 0);   // Wektor uśrednionego kieruku ruchu
            Vector avg_pos = new Vector(0, 0);  // Wektor uśrednionej pozycji Boidów 
            Vector avg_sep = new Vector(0, 0);   // Wektor uśrednionego ... trzymania sie na dystans???
            Vector temp = new Vector(0, 0);      // Wektor pomocniczy
            
            foreach(Boid boid in boids)
            {
                if(this == boid)
                {
                    continue;
                }

                temp.cpy(position);
                temp.sub(boid.position);

                float m = (float)Math.Sqrt(Math.Pow(temp.x, 2) + Math.Pow(temp.y, 2));

                if(m < SEPARATION)
                {
                    temp.div(m);
                    avg_sep.add(temp);
                }

                temp.cpy(boid.position);
                temp.sub(position);
                temp.div(m / 2);

                avg_vel.add(boid.velocity);
                avg_pos.add(temp);
            }

            int l = boids.Count() - 1;
            if (l > 0)
            {
                avg_vel.div(l);
                avg_pos.div(l);
                avg_sep.div(l);

                perception = perception + MAX_IN_VIEW - l;
                if (perception >= PERCEPTION) // Ograniczanie wartości do maksymalnej
                {
                    perception = PERCEPTION;
                }  
            }
            else
            {
                perception = PERCEPTION;
            }

            // Wektor uśrednionego kieruku ruchu jest skalowany i dodawany do prędkości
            avg_vel.mag((float)0.02);
            velocity.add(avg_vel);

            // Wektor uśrednionej pozycji Boidów jest skalowany i dodawany do prędkości
            avg_pos.mag((float)0.01);
            velocity.add(avg_pos);

            // Wektor separacji Boidów jest skalowany i dodawany do prędkości
            avg_sep.mag((float)0.07);
            velocity.add(avg_sep);

            // Wektor uniknięcia przeszkody jest skalowany i dodawany do prędkości
            Vector avg_eve = evasion(quad_obstacles);
            avg_eve.mag((float)0.1);
            velocity.add(avg_eve);

            // Wektor uniknięcia krawędzi ekranu jest skalowany i dodawany do prędkości
            Vector bound = boudry();
            bound.mag((float)0.01);
            velocity.add(bound);

            // Wektor prędkości jest ograniczany do limitu
            velocity.limit(MAX_SPEED);
        }

        Vector evasion(Quad_Obstacles quad_obstacles)
        {
            List<Obstacle> obstacles = quad_obstacles.find_in_square(position.x - PERCEPTION,
                                    position.x + PERCEPTION,
                                    position.y - PERCEPTION,
                                    position.y + PERCEPTION);

            Vector avg = new Vector(0, 0);   // Uśredniony wektor uniku
            Vector temp = new Vector(0, 0);  // Wektor pomocniczy

            foreach (Obstacle obstacle in obstacles)
            {
                temp.cpy(position);
                temp.x -= obstacle.x;
                temp.y -= obstacle.y;

                float m = (float)Math.Sqrt(Math.Pow(temp.x, 2) + Math.Pow(temp.y, 2));

                // Przeskalowanie wektora, im bliżej jest przeszkoda tym silniejszy jest wektor 
                if (m > 0)
                {
                    temp.div(m);
                }

                // Dodajemy obliczony wektor do wektora średniej
                avg.add(temp);
            }

            int l = obstacles.Count() - 1;
            if (l > 0)
            {
                avg.div(l);
            }
            return avg;
        }

        void outside()
        {
            if (position.x > WIDTH) // Wypadł poza prawą krawędź ekranu
            {
                position.x -= WIDTH;
            }

            if (position.x < 0)     // Wypadł poza lewą krawędź ekranu
            {
                position.x += WIDTH;
            }

            if (position.y > HEIGHT)// Wypadł poza dolną krawędź ekranu
            {
                position.y -= HEIGHT;
            }

            if (position.y < 0)     // Wypadł poza górną krawędź ekranu
            {
                position.y += HEIGHT;
            }
        }

        Vector boudry()
        {
            float x = 0;
            float y = 0;

            if (position.x > WIDTH - BOUND){
                x = position.x - WIDTH;
            }

            if (position.x < BOUND)
            {
                x = position.x;
            }

            if (position.y > HEIGHT - BOUND)
            {
                y = position.y - HEIGHT;
            }

            if (position.y < BOUND)
            {
                y = position.y;
            }

            return new Vector(x, y);
        }
    }
}
