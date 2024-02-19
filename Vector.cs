using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Boids
{
    internal class Vector
    {
        public float x; 
        public float y;
        
        public Vector(float x, float y) { 
            this.x = x; 
            this.y = y;
        }

        public void add(Vector vector)
        {
            this.x += vector.x; 
            this.y += vector.y;
        }

        public void sub(Vector vector)
        {
            this.x -= vector.x;
            this.y -= vector.y;
        }

        public void cpy(Vector vector)
        {
            this.x = vector.x;
            this.y = vector.y;
        }

        public void div(float div)
        {
            this.x = this.x/div;
            this.y = this.y/div;
        }

        public void mag(float f)
        {
            this.x *= f;
            this.y *= f;
        }

        public void normalize()
        {
            float m = (float)Math.Sqrt(Math.Pow(this.x, 2) + Math.Pow(this.y, 2));
            if (m != 0)
            {
                this.x /= m;
                this.y /= m;
            }
        }

        public void limit(float f)
        {
            float m = (float)Math.Sqrt(Math.Pow(this.x, 2) + Math.Pow(this.y, 2));
            if (m > f)
            {
                this.x *= f/m;
                this.y *= f/m;
            }
        }
    }
}
