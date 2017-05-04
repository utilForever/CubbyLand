using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class Site : ICoord
    {
        private static Stack<Site> pool = new Stack<Site>();

        public Point coord;
        public double weight;
        private int index;
        public List<Edge> edges;
        private List<Point> reg;
        private List<Orientation> edgeOrientations;

        private static double EPSILON = 0.005;

        public Site(Point p, int index, double weight)
        {
            Initialize(p, index, weight);
        }

        public static Site Create(Point p, int index, double weight)
        {
            if (pool.Count > 0)
            {
                return pool.Pop().Initialize(p, index, weight);
            }

            return new Site(p, index, weight);
        }

        public static void SortSites(List<Site> sites)
        {
            sites.Sort(new SiteComparer());
        }

        private static Boolean CloseEnough(Point p1, Point p2)
        {
            return Point.Distance(p1, p2) < EPSILON;
        }

        public static double Compare(Site s1, Site s2)
        {
            int rValue = CompareYThenX(s1, s2);

            int tempI;
            if (rValue == -1)
            {
                if (s1.index > s2.index)
                {
                    tempI = s1.index;
                    s1.index = s2.index;
                    s2.index = tempI;
                }
            }
            else if (rValue == 1)
            {
                if (s1.index < s2.index)
                {
                    tempI = s2.index;
                    s2.index = s1.index;
                    s1.index = tempI;
                }
            }

            return rValue;
        }

        private static int CompareYThenX(Site s1, Site s2)
        {
            if (s1.coord.y < s2.coord.y)
            {
                return -1;
            }
            if (s1.coord.y > s2.coord.y)
            {
                return 1;
            }
            if (s1.coord.x < s2.coord.x)
            {
                return -1;
            }
            if (s1.coord.x > s2.coord.x)
            {
                return 1;
            }

            return 0;
        }

        private Site Initialize(Point p, int index, double weight)
        {
            this.coord = p;
            this.index = index;
            this.weight = weight;
            edges = new List<Edge>();
            reg = null;
            return this;
        }

        private void Move(Point p)
        {
            Clear();
            coord = p;
        }

        private void Clear()
        {
            if (edges != null)
            {
                edges.Clear();
                edges = null;
            }

            if (edgeOrientations != null)
            {
                edgeOrientations.Clear();
                edgeOrientations = null;
            }

            if (reg != null)
            {
                reg.Clear();
                reg = null;
            }
        }

        public void AddEdge(Edge e)
        {
            edges.Add(e);
        }

        public Edge NearestEdge()
        {
            edges.Sort(new EdgeComparer());
            return edges[0];
        }

        public List<Site> NeighborSites()
        {
            if (edges == null || edges.Count < 1)
            {
                return new List<Site>();
            }

            if (edgeOrientations == null)
            {
                ReorderEdges();
            }

            List<Site> list = new List<Site>();
            foreach (Edge e in edges)
            {
                list.Add(NeighborSite(e));
            }

            return list;
        }

        private Site NeighborSite(Edge e)
        {
            if (this == e.GetLeftSite())
            {
                return e.GetRightSite();
            }
            if (this == e.GetRightSite())
            {
                return e.GetLeftSite();
            }

            return null;
        }

        public List<Point> Region(Rectangle bounds)
        {
            if (edges == null || edges.Count < 1)
            {
                return new List<Point>();
            }

            if (edgeOrientations == null)
            {
                ReorderEdges();
                reg = ClipToBounds(bounds);

                if ((new Polygon(reg)).GetWinding() == Winding.CW)
                {
                    reg.Reverse();
                }
            }

            return reg;
        }

        private void ReorderEdges()
        {
            EdgeReorderer er = new EdgeReorderer(edges, "v");
            edges = er.GetEdges();
            edgeOrientations = er.GetEdgeOrientations();
            er.Dispose();
        }

        public void Dispose()
        {
            coord = null;
            Clear();
            pool.Push(this);
        }

        private List<Point> ClipToBounds(Rectangle bounds)
        {
            List<Point> points = new List<Point>();
            int n = edges.Count;
            int i = 0;
            Edge e;

            while (i < n && (edges[i].GetVisible() == false))
            {
                i++;
            }

            if (i == n)
            {
                return new List<Point>();
            }

            e = edges[i];
            Orientation orient = edgeOrientations[i];

            points.Add(e.GetClippedEnds()[orient]);
            points.Add(e.GetClippedEnds()[Orientation.Other(orient)]);

            for (int j = i + 1; j < n; j++)
            {
                e = edges[j];

                if (e.GetVisible() == false)
                {
                    continue;
                }

                Connect(points, j, bounds, false);
            }

            Connect(points, i, bounds, true);

            return points;
        }

        private void Connect(List<Point> points, int j, Rectangle bounds, bool Closing)
        {
            Point rightPoint = points[points.Count - 1];
            Edge nEdge = edges[j];
            Orientation nOrient = edgeOrientations[j];
            Point nPoint = nEdge.GetClippedEnds()[nOrient];

            if (CloseEnough(rightPoint, nPoint))
            {
                if ((rightPoint.x != nPoint.x) && (rightPoint.y != nPoint.y))
                {
                    int rightCheck = BoundsCheck.Check(rightPoint, bounds);
                    int nCheck = BoundsCheck.Check(nPoint, bounds);

                    double pX;
                    double pY;

                    if ((rightCheck & BoundsCheck.RIGHT) != 0)
                    {
                        pX = bounds.right;
                        if ((nCheck & BoundsCheck.BOTTOM) != 0)
                        {
                            pY = bounds.bottom;
                            points.Add(new Point(pX, pY));
                        }
                        else if ((nCheck & BoundsCheck.TOP) != 0)
                        {
                            pY = bounds.top;
                            points.Add(new Point(pX, pY));
                        }
                        else if ((nCheck & BoundsCheck.LEFT) != 0)
                        {
                            if (((rightPoint.y - bounds.y + nPoint.y) - bounds.y) < bounds.height)
                            {
                                pY = bounds.top;
                            }
                            else
                            {
                                pY = bounds.bottom;
                            }

                            points.Add(new Point(pX, pY));
                            points.Add(new Point(bounds.left, pY));
                        }
                    }
                    else if ((rightCheck & BoundsCheck.LEFT) != 0)
                    {
                        pX = bounds.left;

                        if ((nCheck & BoundsCheck.BOTTOM) != 0)
                        {
                            pY = bounds.bottom;
                            points.Add(new Point(pX, pY));
                        }
                        else if ((nCheck & BoundsCheck.TOP) != 0)
                        {
                            pY = bounds.top;
                            points.Add(new Point(pX, pY));
                        }
                        else if ((nCheck & BoundsCheck.RIGHT) != 0)
                        {
                            if (((rightPoint.y - bounds.y + nPoint.y) - bounds.y) < bounds.height)
                            {
                                pY = bounds.top;
                            }
                            else
                            {
                                pY = bounds.bottom;
                            }

                            points.Add(new Point(pX, pY));
                            points.Add(new Point(bounds.right, pY));
                        }
                    }
                    else if ((rightCheck & BoundsCheck.TOP) != 0)
                    {
                        pY = bounds.top;

                        if ((nCheck & BoundsCheck.RIGHT) != 0)
                        {
                            pX = bounds.right;
                            points.Add(new Point(pX, pY));
                        }
                        else if ((nCheck & BoundsCheck.LEFT) != 0)
                        {
                            pX = bounds.left;
                            points.Add(new Point(pX, pY));
                        }
                        else if ((nCheck & BoundsCheck.BOTTOM) != 0)
                        {
                            if (((rightPoint.x - bounds.x + nPoint.x) - bounds.x) < bounds.width)
                            {
                                pX = bounds.left;
                            }
                            else
                            {
                                pX = bounds.right;
                            }

                            points.Add(new Point(pX, pY));
                            points.Add(new Point(pX, bounds.bottom));
                        }
                    }
                    else if ((rightCheck & BoundsCheck.BOTTOM) != 0)
                    {
                        pY = bounds.bottom;

                        if ((nCheck & BoundsCheck.RIGHT) != 0)
                        {
                            pX = bounds.right;
                            points.Add(new Point(pX, pY));
                        }
                        else if ((nCheck & BoundsCheck.LEFT) != 0)
                        {
                            pX = bounds.left;
                            points.Add(new Point(pX, pY));
                        }
                        else if ((nCheck & BoundsCheck.TOP) != 0)
                        {
                            if (((rightPoint.x - bounds.x + nPoint.x) - bounds.x) < bounds.width)
                            {
                                pX = bounds.left;
                            }
                            else
                            {
                                pX = bounds.right;
                            }

                            points.Add(new Point(pX, pY));
                            points.Add(new Point(pX, bounds.top));
                        }
                    }
                }

                if (Closing)
                {
                    return;
                }

                points.Add(nPoint);
            }

            Point nRightPoint = nEdge.GetClippedEnds()[Orientation.Other(nOrient)];

            if (!CloseEnough(points[0], nRightPoint))
            {
                points.Add(nRightPoint);
            }
        }

        public double Dist(ICoord p)
        {
            return Point.Distance(p.GetCoord(), this.coord);
        }

        public Point GetCoord()
        {
            return this.coord;
        }

        public class BoundsCheck
        {
            public const int TOP = 1;
            public const int BOTTOM = 2;
            public const int LEFT = 4;
            public const int RIGHT = 8;

            public static int Check(Point p, Rectangle b)
            {
                int value = 0;

                if (p.x == b.left)
                {
                    value |= LEFT;
                }
                if (p.x == b.right)
                {
                    value |= RIGHT;
                }
                if (p.x == b.top)
                {
                    value |= TOP;
                }
                if (p.x == b.bottom)
                {
                    value |= BOTTOM;
                }

                return value;
            }
        }

        private class SiteComparer : Comparer<Site>
        {
            public override int Compare(Site x, Site y)
            {
                return (int)Site.Compare(x, y);
            }
        }

        private class EdgeComparer : Comparer<Edge>
        {
            public override int Compare(Edge x, Edge y)
            {
                return (int)Edge.CompareSitesDistances(x, y);
            }
        }
    }
}
