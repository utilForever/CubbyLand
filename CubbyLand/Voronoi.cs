using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class Voronoi
    {
        private SiteList sites;
        private Dictionary<Point, Site> sitesByLocation;
        private List<Triangle> triangles;
        private List<Edge> edges;
        private Rectangle bounds;

        public Voronoi(List<Point> points, Rectangle bounds)
        {
            Initialize(points, bounds);
            Fortunes();
        }

        public Voronoi(List<Point> points)
        {
            double maxW = 0;
            double maxH = 0;

            foreach (Point p in points)
            {
                maxW = Math.Max(maxW, p.x);
                maxH = Math.Max(maxH, p.y);
            }

            Initialize(points, new Rectangle(0, 0, maxW, maxH));
            Fortunes();
        }

        public Voronoi(int numSites, double maxW, double maxH, Random r)
        {
            List<Point> points = new List<Point>();

            for (int i = 0; i < numSites; i++)
            {
                points.Add(new Point(r.NextDouble() * maxW, r.NextDouble() * maxH));
            }

            Initialize(points, new Rectangle(0, 0, maxW, maxH));
            Fortunes();
        }

        private void Initialize(List<Point> points, Rectangle bounds)
        {
            sites = new SiteList();
            sitesByLocation = new Dictionary<Point, Site>();

            AddSites(points);

            this.bounds = bounds;
            this.triangles = new List<Triangle>();
            this.edges = new List<Edge>();
        }

        public Rectangle GetPlotBounds()
        {
            return this.bounds;
        }

        private void AddSites(List<Point> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                AddSite(points[i], i);
            }
        }

        private void AddSite(Point p, int index)
        {
            Random r = new Random();

            double weight = r.NextDouble() * 100;
            Site s = Site.Create(p, index, weight);
            sites.Push(s);
            sitesByLocation[p] = s;
        }

        public List<Edge> Edges()
        {
            return edges;
        }

        public List<Point> Region(Point p)
        {
            Site s = sitesByLocation[p];

            if (s == null)
            {
                return new List<Point>();
            }

            return s.Region(bounds);
        }

        public List<Point> NeighborSitesForSite(Point c)
        {
            List<Point> points = new List<Point>();
            Site s = sitesByLocation[c];

            if (s == null)
            {
                return points;
            }

            List<Site> sites = s.NeighborSites();

            foreach (Site n in sites)
            {
                points.Add(n.coord);
            }

            return points;
        }

        public List<Circle> Circles()
        {
            return this.sites.Circles();
        }

        private List<Edge> SelectEdgeForSitePoint(Point coord, List<Edge> edgesToTest)
        {
            List<Edge> filtered = new List<Edge>();

            foreach (Edge e in edgesToTest)
            {
                if (((e.GetLeftSite() != null && e.GetLeftSite().coord == coord) ||
                     (e.GetRightSite() != null && e.GetRightSite().coord == coord)))
                {
                    filtered.Add(e);
                }
            }
            return filtered;
        }

        private List<LineSegment> VisibleLineSegments(List<Edge> edges)
        {
            List<LineSegment> s = new List<LineSegment>();

            foreach (Edge e in edges)
            {
                if (e.GetVisible())
                {
                    Point p1 = e.GetClippedEnds()[Orientation.LEFT];
                    Point p2 = e.GetClippedEnds()[Orientation.RIGHT];

                    s.Add(new LineSegment(p1, p2));
                }
            }

            return s;
        }

        private List<LineSegment> DelaunayLinesForEdges(List<Edge> edges)
        {
            List<LineSegment> s = new List<LineSegment>();

            foreach (Edge e in edges)
            {
                s.Add(e.DelaunayLine());
            }

            return s;
        }

        public List<LineSegment> VoronoiBoundsForSite(Point c)
        {
            return VisibleLineSegments(SelectEdgeForSitePoint(c, this.edges));
        }

        public List<LineSegment> DelaunayLinesForSite(Point c)
        {
            return DelaunayLinesForEdges(SelectEdgeForSitePoint(c, this.edges));
        }

        public List<LineSegment> VoronoiDiagram()
        {
            return VisibleLineSegments(this.edges);
        }

        public List<LineSegment> Hull()
        {
            return DelaunayLinesForEdges(HullEdges());
        }

        private List<Edge> HullEdges()
        {
            List<Edge> filtered = new List<Edge>();

            foreach (Edge e in edges)
            {
                if (e.IsPartOfConvexHull())
                {
                    filtered.Add(e);
                }
            }

            return filtered;
        }

        public List<Point> HullPointsInOrder()
        {
            List<Edge> hullEdges = HullEdges();
            List<Point> points = new List<Point>();

            if (hullEdges.Count < 1)
            {
                return points;
            }

            EdgeReorderer re = new EdgeReorderer(hullEdges, "s");
            hullEdges = re.GetEdges();
            List<Orientation> orientations = re.GetEdgeOrientations();
            re.Dispose();

            Orientation orient;

            int n = hullEdges.Count;
            for (int i = 0; i < n; ++i)
            {
                Edge e = hullEdges[i];
                orient = orientations[i];
                points.Add(e.Site(orient).coord);
            }

            return points;
        }

        public List<List<Point>> Regions()
        {
            return sites.Regions(bounds);
        }

        public List<Point> SiteCoords()
        {
            return sites.SiteCoords();
        }

        private void Fortunes()
        {
            Site nSite;
            Site bottomSite;
            Site topSite;
            Site tSite;

            Vertex v1;
            Vertex v2;

            Point newIntStar = null;

            Orientation lr;
            HalfEdge lBnd;
            HalfEdge rBnd;
            HalfEdge llBnd;
            HalfEdge rrBnd;
            HalfEdge bisector;

            Edge e;

            Rectangle dataBounds = sites.GetSitesBounds();

            int sqrtNSites = (int)Math.Sqrt(sites.GetLength() + 4);
            HalfEdgePriorityQueue heap = new HalfEdgePriorityQueue(dataBounds.y, dataBounds.height, sqrtNSites);
            EdgeList edgeList = new EdgeList(dataBounds.x, dataBounds.width, sqrtNSites);
            List<HalfEdge> halfEdges = new List<HalfEdge>();
            List<Vertex> vertices = new List<Vertex>();

            Site lowestSite = this.sites.Next();
            nSite = sites.Next();

            for (;;)
            {
                if (heap.Empty() == false)
                {
                    newIntStar = heap.Min();
                }

                if (nSite != null && (heap.Empty() || CompareByYThenX(nSite, newIntStar) < 0))
                {
                    lBnd = edgeList.EdgeListLeftNeighbor(nSite.coord);
                    rBnd = lBnd.edgeListRightNeighbor;
                    bottomSite = RightRegion(lBnd, lowestSite);
                    e = Edge.CreateBisectingEdge(bottomSite, nSite);
                    this.edges.Add(e);

                    bisector = HalfEdge.Create(e, Orientation.LEFT);
                    halfEdges.Add(bisector);
                    //Console.WriteLine(lBnd.edgeListLeftNeighbor);
                    //Console.WriteLine(lBnd.edgeListRightNeighbor);
                    edgeList.Insert(lBnd, bisector);

                    if ((v2 = Vertex.Intersect(lBnd, bisector)) != null)
                    {
                        vertices.Add(v2);
                        heap.Remove(lBnd);
                        lBnd.v = v2;
                        lBnd.yStar = v2.GetCoord().y + nSite.Dist(v2);
                        heap.Insert(lBnd);
                    }

                    lBnd = bisector;
                    bisector = HalfEdge.Create(e, Orientation.RIGHT);
                    halfEdges.Add(bisector);
                    edgeList.Insert(lBnd, bisector);

                    if ((v2 = Vertex.Intersect(bisector, rBnd)) != null)
                    {
                        vertices.Add(v2);
                        bisector.v = v2;
                        bisector.yStar = v2.GetCoord().y + nSite.Dist(v2);
                        heap.Insert(bisector);
                    }

                    nSite = this.sites.Next();
                }
                else if (heap.Empty() == false)
                {
                    lBnd = heap.ExtractMin();
                    llBnd = lBnd.edgeListLeftNeighbor;
                    rBnd = lBnd.edgeListRightNeighbor;
                    rrBnd = rBnd.edgeListRightNeighbor;

                    bottomSite = LeftRegion(lBnd, lowestSite);
                    topSite = RightRegion(rBnd, lowestSite);

                    v1 = lBnd.v;
                    v1.SetIndex();
                    lBnd.e.SetVertex(lBnd.orientation, v1);
                    rBnd.e.SetVertex(rBnd.orientation, v1);
                    edgeList.Remove(lBnd);
                    heap.Remove(rBnd);
                    edgeList.Remove(rBnd);
                    lr = Orientation.LEFT;

                    if (bottomSite.coord.y > topSite.coord.y)
                    {
                        tSite = bottomSite;
                        bottomSite = topSite;
                        topSite = tSite;
                        lr = Orientation.RIGHT;
                    }

                    e = Edge.CreateBisectingEdge(bottomSite, topSite);
                    this.edges.Add(e);
                    bisector = HalfEdge.Create(e, lr);
                    halfEdges.Add(bisector);
                    edgeList.Insert(llBnd, bisector);
                    e.SetVertex(Orientation.Other(lr), v1);

                    if ((v2 = Vertex.Intersect(llBnd, bisector)) != null)
                    {
                        vertices.Add(v2);
                        heap.Remove(llBnd);
                        llBnd.v = v2;
                        llBnd.yStar = v2.GetCoord().y + bottomSite.Dist(v2);
                        heap.Insert(llBnd);
                    }

                    if ((v2 = Vertex.Intersect(bisector, rrBnd)) != null)
                    {
                        vertices.Add(v2);
                        bisector.v = v2;
                        bisector.yStar = v2.GetCoord().y + bottomSite.Dist(v2);
                        heap.Insert(bisector);
                    }
                }
                else
                {
                    break;
                }
            }

            heap.Dispose();
            edgeList.Dispose();

            foreach (HalfEdge hE in halfEdges)
            {
                hE.Kill();
            }

            halfEdges.Clear();

            foreach (Edge tE in this.edges)
            {
                tE.ClipVertices(bounds);
            }

            foreach (Vertex v0 in vertices)
            {
                v0.Dispose();
            }

            vertices.Clear();
        }

        Site LeftRegion(HalfEdge hE, Site lowestSite)
        {
            Edge e = hE.e;

            if (e == null)
            {
                return lowestSite;
            }

            return e.Site(hE.orientation);
        }

        Site RightRegion(HalfEdge hE, Site lowestSite)
        {
            Edge e = hE.e;

            if (e == null)
            {
                return lowestSite;
            }

            return e.Site(Orientation.Other(hE.orientation));
        }

        public static int CompareByYThenX(Site s1, Site s2)
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

        public static int CompareByYThenX(Site s1, Point s2)
        {
            if (s1.coord.y < s2.y)
            {
                return -1;
            }
            if (s1.coord.y > s2.y)
            {
                return 1;
            }
            if (s1.coord.x < s2.x)
            {
                return -1;
            }
            if (s1.coord.x > s2.x)
            {
                return 1;
            }

            return 0;
        }

        public void Dispose()
        {
            int i;
            int n;

            if (sites != null)
            {
                sites.Dispose();
                sites = null;
            }

            if (triangles != null)
            {
                n = triangles.Count;

                for (i = 0; i < n; i++)
                {
                    triangles[i].Dispose();
                }

                triangles.Clear();
                triangles = null;
            }

            if (edges != null)
            {
                n = edges.Count;

                for (i = 0; i < n; i++)
                {
                    edges[i].Dispose();
                }

                edges.Clear();
                edges = null;
            }

            bounds = null;
            sitesByLocation = null;
        }
    }
}