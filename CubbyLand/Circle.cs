using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class Circle
    {
        public Point center;
        public double radius;

        public Circle(double centerX, double centerY, double radius)
        {
            this.center = new Point(centerX, centerY);
            this.radius = radius;
        }
    }
}
