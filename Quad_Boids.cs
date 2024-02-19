using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boids
{
    internal class Quad_Boids
    {
        private List<Quad_Boids> chidren;
        private List<Boid> boids;
        public int x1;
        public int y1;
        public int x2;
        public int y2;
        private int max;

        public Quad_Boids(int x1, int x2, int y1, int y2, int max)
        {
            this.x1 = x1;
            this.x2 = x2;   
            this.y1 = y1;
            this.y2 = y2;
            this.max = max;

            this.boids = new List<Boid>();
            this.chidren = new List<Quad_Boids> ();
        }

        public void split()
        {
            Quad_Boids quad = new Quad_Boids(x1, (x1 + x2) / 2, y1, (y1 + y2) / 2, max);
            this.chidren.Add(quad);

            quad = new Quad_Boids((x1 + x2) / 2, x2, y1, (y1 + y2) / 2, max);
            this.chidren.Add(quad);

            quad = new Quad_Boids(x1, (x1 + x2) / 2, (y1 + y2) / 2, y2, max);
            this.chidren.Add(quad);

            quad = new Quad_Boids((x1 + x2) / 2, x2, (y1 + y2) / 2, y2, max);
            this.chidren.Add(quad);
        }

        public void add_boid(Boid boid)
        {
            if (x1 <= boid.position.x && boid.position.x < x2 && y1 <= boid.position.y && boid.position.y  < y2){
                if(boids.Count() == max)
                {
                    if (chidren.Count() == 0)
                    {
                        split();
                    }
                    foreach (Quad_Boids child in chidren)
                    {
                        child.add_boid(boid);
                    }
                }
                else
                {
                    boids.Add(boid);
                }
            }
        }

        public List<Quad_Boids> getAll()
        {
            List<Quad_Boids> quads = new List<Quad_Boids>();

            quads.Add(this);

            foreach(Quad_Boids quad in this.chidren)
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

        public List<Boid> find_in_square(float x1, float x2, float y1, float y2)
        {
            List<Boid> result = new List<Boid>();
            if (intersect(x1, x2, y1, y2)){
                foreach (Boid boid in this.boids)
                {
                    if (x1 <= boid.position.x && 
                        boid.position.x < x2  &&
                        y1 <= boid.position.y &&
                        boid.position.y < y2)
                    {
                        result.Add(boid);
                    }
                }

                foreach (Quad_Boids quad in this.chidren)
                {
                    result = result.Concat(quad.find_in_square(x1, x2, y1, y2)).ToList();
                }
            }
            return result;
        }

    }
}
