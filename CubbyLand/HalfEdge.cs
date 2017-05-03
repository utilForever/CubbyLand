using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class HalfEdge
    {
        private static Stack<HalfEdge> pool = new Stack<HalfEdge>();

        public HalfEdge edgeListLeftNeighbor;
        public HalfEdge edgeListRightNeighbor;
        public HalfEdge nextInPriorityQueue;
        public Edge e;
        public Orientation orientation;
        public Vertex v;
        public double yStar;

        public HalfEdge(Edge e, Orientation orientation)
        {
            Initialize(e, orientation);
        }

        public static HalfEdge Create(Edge e, Orientation orientation)
        {
            if (pool.Count > 0)
            {
                return pool.Pop().Initialize(e, orientation);
            }
            else
            {
                return new HalfEdge(e, orientation);
            }
        }

        public static HalfEdge CreateDummy()
        {
            return Create(null, null);
        }

        private HalfEdge Initialize(Edge e, Orientation orientation)
        {
            this.e = e;
            this.orientation = orientation;
            nextInPriorityQueue = null;
            v = null;
            return this;
        }

        public void Dispose()
        {
            if (edgeListLeftNeighbor != null || edgeListRightNeighbor != null)
            {
                return;
            }
            if (nextInPriorityQueue != null)
            {
                return;
            }

            e = null;
            orientation = null;
            v = null;
            pool.Push(this);
        }

        public void Kill()
        {
            edgeListLeftNeighbor = null;
            edgeListRightNeighbor = null;
            nextInPriorityQueue = null;
            e = null;
            orientation = null;
            v = null;
            pool.Push(this);
        }

        public bool IsLeftOf(Point p)
        {
            Site top;
            bool rightOfSite;
            bool above;
            bool fast;
            double dXP;
            double dYP;
            double dXS;
            double t1;
            double t2;
            double t3;
            double y1;

            top = e.GetRightSite();
            rightOfSite = p.x > top.coord.x;

            if (rightOfSite && this.orientation == Orientation.LEFT)
            {
                return true;
            }
            if (!rightOfSite && this.orientation == Orientation.RIGHT)
            {
                return false;
            }

            if (e.a == 1.0)
            {
                dYP = p.y - top.coord.y;
                dXP = p.x - top.coord.x;
                fast = false;

                if ((!rightOfSite && e.b < 0.0) || (rightOfSite && (e.b >= 0.0)))
                {
                    above = dYP >= (e.b * dXP);
                    fast = above;
                }
                else
                {
                    above = (p.x + (p.y * e.b)) > e.c;
                    if (e.b < 0.0)
                        above = !above;
                    if (!above)
                        fast = true;
                }

                if (!fast)
                {
                    dXS = top.coord.x - e.GetLeftSite().coord.x;
                    above = (e.b * ((dXP * dXP) - (dYP * dYP))) < (dXS * dYP * (1.0 + ((2.0 * dXP) / dXS) + (e.b * e.b)));
                    if (e.b < 0.0)
                        above = !above;
                }
            }
            else
            {
                y1 = e.c - (e.a * p.x);
                t1 = p.y - y1;
                t2 = p.x - top.coord.x;
                t3 = y1 - top.coord.y;
                above = (t1 * t1) > ((t2 * t2) + (t3 * t3));
            }

            return (this.orientation == Orientation.LEFT) ? above : (!above);
        }
    }
}
