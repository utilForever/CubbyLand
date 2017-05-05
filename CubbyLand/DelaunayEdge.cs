using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class DelaunayEdge
    {
        public int index;
        public Center d0, d1;
        public Corner v0, v1;
        public Point midPoint;
        public int river;

        public void SetVoronoi(Corner v0, Corner v1)
        {
            this.v0 = v0;
            this.v1 = v1;

            midPoint = new Point((v0.loc.x + v1.loc.x) / 2, (v0.loc.y + v1.loc.y) / 2);
        }
    }
}
