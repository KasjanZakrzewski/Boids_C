using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boids
{
    internal class Quad_Obstacles
    {
        private List<Quad_Obstacles> chidren;
        private List<Obstacle> obstacles;
        public int x1;
        public int y1;
        public int x2;
        public int y2;
        private int max;

        public Quad_Obstacles(int x1, int x2, int y1, int y2, int max)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
            this.max = max;

            this.obstacles = new List<Obstacle>();
            this.chidren = new List<Quad_Obstacles>();
        }

        public void split()
        {
            Quad_Obstacles quad = new Quad_Obstacles(x1, (x1 + x2) / 2, y1, (y1 + y2) / 2, max);
            this.chidren.Add(quad);

            quad = new Quad_Obstacles((x1 + x2) / 2, x2, y1, (y1 + y2) / 2, max);
            this.chidren.Add(quad);

            quad = new Quad_Obstacles(x1, (x1 + x2) / 2, (y1 + y2) / 2, y2, max);
            this.chidren.Add(quad);

            quad = new Quad_Obstacles((x1 + x2) / 2, x2, (y1 + y2) / 2, y2, max);
            this.chidren.Add(quad);
        }

        public void add_obstacle(Obstacle obstacle)
        {
            if (x1 <= obstacle.x && obstacle.x < x2 && y1 <= obstacle.y && obstacle.y < y2)
            {
                if (obstacles.Count() == max)
                {
                    if (chidren.Count() == 0)
                    {
                        split();
                    }
                    foreach (Quad_Obstacles child in chidren)
                    {
                        child.add_obstacle(obstacle);
                    }
                }
                else
                {
                    obstacles.Add(obstacle);
                }
            }
        }

        public List<Quad_Obstacles> getAll()
        {
            List<Quad_Obstacles> quads = new List<Quad_Obstacles>();

            quads.Add(this);

            foreach (Quad_Obstacles quad in this.chidren)
            {
                quads = quads.Concat(quad.getAll()).ToList();
            }

            return quads;
        }

        bool intersect(float x1, float x2, float y1, float y2)
        {
            return !(this.x2 < x1 ||
                    this.x1 > x2 ||
                    this.y1 > y2 ||
                    this.y2 < y1);
        }

        public List<Obstacle> find_in_square(float x1, float x2, float y1, float y2)
        {
            List<Obstacle> result = new List<Obstacle>();
            if (intersect(x1, x2, y1, y2))
            {
                foreach (Obstacle obstacle in this.obstacles)
                {
                    if (x1 <= obstacle.x &&
                        obstacle.x < x2 &&
                        y1 <= obstacle.y &&
                        obstacle.y < y2)
                    {
                        result.Add(obstacle);
                    }
                }

                foreach (Quad_Obstacles quad in this.chidren)
                {
                    result = result.Concat(quad.find_in_square(x1, x2, y1, y2)).ToList();
                }
            }
            return result;
        }
    }
}
