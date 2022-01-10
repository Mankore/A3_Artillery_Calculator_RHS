using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A3_Arty_Calc
{
    class CoordHeight
    {
        public int x;
        public int y;
        public double height;

        public CoordHeight(int x, int y, double height)
        {
            this.x = x;
            this.y = y;
            this.height = height;
        }

        public string toString()
        {
            return $"x: {this.x}, y: {this.y}, height: {this.height}";
        }
    }
}
