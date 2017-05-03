using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class Voronoi
    {
        public static int CompareByYThenX(Site s1, Site s2)
        {
            if (s1.coord.y < s2.coord.y)
                return -1;
            if (s1.coord.y > s2.coord.y)
                return 1;
            if (s1.coord.x < s2.coord.x)
                return -1;
            if (s1.coord.x > s2.coord.x)
                return 1;
            return 0;
        }

        public static int CompareByYThenX(Site s1, Point s2)
        {
            if (s1.coord.y < s2.y)
                return -1;
            if (s1.coord.y > s2.y)
                return 1;
            if (s1.coord.x < s2.x)
                return -1;
            if (s1.coord.x > s2.x)
                return 1;
            return 0;
        }
    }
}
