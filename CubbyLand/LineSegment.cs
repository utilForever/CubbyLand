using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class LineSegment
    {
        public Point p0, p1;

        public LineSegment(Point p0, Point p1)
        {
            this.p0 = p0;
            this.p1 = p1;
        }

        public static double CompareLengths_MAX(LineSegment s0, LineSegment s1)
        {
            double l0 = Point.Distance(s0.p0, s0.p1);
            double l1 = Point.Distance(s1.p0, s1.p1);

            if (l0 < l1)
            {
                return 1;
            }
            if (l0 > l1)
            {
                return -1;
            }

            return 0;
        }

        public static double CompareLength(LineSegment e0, LineSegment e1)
        {
            return -CompareLengths_MAX(e0, e1);
        }
    }
}
