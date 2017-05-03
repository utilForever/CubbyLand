using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class Edge
    {
        private static Stack<Edge> pool = new Stack<Edge>();
        public static Edge DELETED = new Edge();

        private Vertex leftVertex;
        private Vertex rightVertex;
        public double a;
        public double b;
        public double c;
        private static int numEdges = 0;
        private Dictionary<Orientation, Point> clippedVertices;
        private Dictionary<Orientation, Site> sites;
        private int index;

        private Edge()
        {
            index = numEdges++;
            Initialize();
        }

        public static Edge CreateBisectingEdge(Site site0, Site site1)
        {
            double dX;
            double dY;
            double absdX;
            double absdY;
            double a;
            double b;
            double c;

            dX = site1.coord.x - site0.coord.x;
            dY = site1.coord.y - site0.coord.y;
            absdX = dX > 0 ? dX : -dX;
            absdY = dY > 0 ? dY : -dY;

            c = (site0.coord.x * dX) + (site0.coord.y * dY) + (((dX * dX) + (dY * dY)) * 0.5);

            if (absdX > absdY)
            {
                a = 1.0;
                b = dY / dX;
                c /= dX;
            }
            else
            {
                b = 1.0;
                a = dX / dY;
                c /= dY;
            }

            Edge e = Edge.Create();

            e.SetLeftSite(site0);
            e.SetRightSite(site1);
            site0.AddEdge(e);
            site1.AddEdge(e);

            e.leftVertex = null;
            e.rightVertex = null;

            e.a = a;
            e.b = b;
            e.c = c;

            return e;
        }

        private static Edge Create()
        {
            Edge e;

            if (pool.Count > 0)
            {
                e = pool.Pop();
                e.Initialize();
            }
            else
            {
                e = new Edge();
            }

            return e;
        }

        private void Initialize()
        {
            sites = new Dictionary<Orientation, Site>();
        }

        public LineSegment DelaunayLine()
        {
            return new LineSegment(GetLeftSite().coord, GetRightSite().coord);
        }

        public LineSegment VoronoiEdge()
        {
            if (!GetVisible())
            {
                return new LineSegment(null, null);
            }

            return new LineSegment(clippedVertices[Orientation.LEFT], clippedVertices[Orientation.RIGHT]);
        }

        public Vertex GetLeftVertex()
        {
            return leftVertex;
        }

        public Vertex GetRightVertex()
        {
            return rightVertex;
        }

        public Vertex GetVertex(Orientation orientation)
        {
            return (orientation == Orientation.LEFT) ? leftVertex : rightVertex;
        }

        public void SetVertex(Orientation orientation, Vertex v)
        {
            if (orientation == Orientation.LEFT)
            {
                leftVertex = v;
            }
            else
            {
                rightVertex = v;
            }
        }

        public bool IsPartOfConvexHull()
        {
            return (leftVertex == null || rightVertex == null);
        }

        public Dictionary<Orientation, Point> GetClippedEnds()
        {
            return clippedVertices;
        }

        public bool GetVisible()
        {
            return clippedVertices != null;
        }

        public double SitesDistance()
        {
            return Point.Distance(GetLeftSite().coord, GetRightSite().coord);
        }

        public static double CompareSitesDistances_MAX(Edge e0, Edge e1)
        {
            double l0 = e0.SitesDistance();
            double l1 = e1.SitesDistance();

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

        public static double CompareSitesDistances(Edge e0, Edge e1)
        {
            return -CompareSitesDistances_MAX(e0, e1);
        }

        public Site GetLeftSite()
        {
            return sites[Orientation.LEFT];
        }

        public Site GetRightSite()
        {
            return sites[Orientation.RIGHT];
        }

        public void SetLeftSite(Site s)
        {
            sites[Orientation.LEFT] = s;
        }

        public void SetRightSite(Site s)
        {
            sites[Orientation.RIGHT] = s;
        }
        
        public Site Site(Orientation orientation)
        {
            return sites[orientation];
        }

        public void Dispose()
        {
            leftVertex = null;
            rightVertex = null;

            if (clippedVertices != null)
            {
                clippedVertices.Clear();
                clippedVertices = null;
            }

            sites.Clear();
            sites = null;

            pool.Push(this);
        }

        public void ClipVertices(Rectangle bounds)
        {
            double xMin = bounds.x;
            double yMin = bounds.y;
            double xMax = bounds.right;
            double yMax = bounds.bottom;

            Vertex v0, v1;
            double x0, x1, y0, y1;

            if (a == 1.0 && b >= 0.0)
            {
                v0 = rightVertex;
                v1 = leftVertex;
            }
            else
            {
                v0 = leftVertex;
                v1 = rightVertex;
            }

            if (a == 1.0)
            {
                y0 = yMin;
                if ((v0 != null) && (v0.GetCoord().y > yMin))
                {
                    y0 = v0.GetCoord().y;
                }
                if (y0 > yMax)
                {
                    return;
                }

                x0 = c - b * y0;

                y1 = yMax;
                if ((v1 != null) && (v1.GetCoord().y < yMax))
                {
                    y1 = v1.GetCoord().y;
                }
                if (y1 < yMin)
                {
                    return;
                }

                x1 = c - (b * y1);

                if (((x0 > xMax) && (x1 > xMax)) || ((x0 < xMin) && (x1 < xMin)))
                {
                    return;
                }

                if (x0 > xMax)
                {
                    x0 = xMax;
                    y0 = (c - x0) / b;
                }
                else if (x0 < xMin)
                {
                    x0 = xMin;
                    y0 = (c - x0) / b;
                }

                if (x1 > xMax)
                {
                    x1 = xMax;
                    y1 = (c - x1) / b;
                }
                else if (x1 < xMin)
                {
                    x1 = xMin;
                    y1 = (c - x1) / b;
                }
            }
            else
            {
                x0 = xMin;
                if (v0 != null && v0.GetCoord().x > xMin)
                {
                    x0 = v0.GetCoord().x;
                }
                if (x0 > xMax)
                {
                    return;
                }
                y0 = c - (a * x0);

                x1 = xMax;
                if ((v1 != null) && (v1.GetCoord().x < xMax))
                {
                    x1 = v1.GetCoord().x;
                }
                if (x1 < xMin)
                {
                    return;
                }
                y1 = c - (a * x1);

                if (((y0 > yMax) && (y1 > yMax)) || ((y0 < yMin) && (y1 < yMin)))
                {
                    return;
                }

                if (y0 > yMax)
                {
                    y0 = yMax;
                    x0 = (c - y0) / a;
                }
                else if (y0 < yMin)
                {
                    y0 = yMin;
                    x0 = (c - y0) / a;
                }

                if (y1 > yMax)
                {
                    y1 = yMax;
                    x1 = (c - y1) / a;
                }
                else if (y1 < yMin)
                {
                    y1 = yMin;
                    x1 = (c - y1) / a;
                }
            }

            clippedVertices = new Dictionary<Orientation, Point>();
            if (v0 == leftVertex)
            {
                clippedVertices[Orientation.LEFT] = new Point(x0, y0);
                clippedVertices[Orientation.RIGHT] = new Point(x1, y1);
            }
            else
            {
                clippedVertices[Orientation.RIGHT] = new Point(x0, y0);
                clippedVertices[Orientation.LEFT] = new Point(x1, y1);
            }
        }
    }
}
