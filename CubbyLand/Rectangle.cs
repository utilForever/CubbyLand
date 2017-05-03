using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class Rectangle
    {
        public double x;
        public double y;
        public double width;
        public double height;
        public double right;
        public double bottom;
        public double left;
        public double top;

        public Rectangle(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.left = x;
            this.top = y;
            this.right = x + width;
            this.bottom = y + height;
        }

        public bool LiesOnAxes(Point p)
        {
            return ((Math.Abs(p.x - x) <= 1) || (Math.Abs(p.y - y) <= 1));
        }

        public bool InBounds(Point p)
        {
            return InBounds(p.x, p.y);
        }

        public bool InBounds(double x, double y)
        {
            return !(x < this.x || x > right || y < this.y || y > bottom);
        }
    }
}
