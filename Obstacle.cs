﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boids
{
    internal class Obstacle
    {
        public int x;
        public int y;

        public Obstacle(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
