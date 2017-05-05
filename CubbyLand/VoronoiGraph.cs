using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public abstract class VoronoiGraph
    {
        #region Variables

        public bool MORE_RANDOM = false;

        public List<DelaunayEdge> edges = new List<DelaunayEdge>();
        public List<Corner> corners = new List<Corner>();
        public List<Center> centers = new List<Center>();
        public Rectangle bounds;
        private Random r;
        protected System.Drawing.Color OCEAN, RIVER, LAKE, BEACH;
        public System.Drawing.Bitmap pixelCenterMap;
        private int bumps;
        private double[,] noise;
        private const double ISLAND_FACTOR = 1.15;
        private double startAngle;
        private double dipAngle;
        private double dipWidth;

        // Implemented in GraphImplementation
        abstract protected Enum GetBiome(Center p);

        abstract protected System.Drawing.Color GetColor(Enum biome);

        #endregion Variables

        #region Constructor

        public VoronoiGraph(Voronoi v, int numLloyd, Random r, bool more)
        {
            // MORE_RANDOM adds more height to everything using the random number generator
            this.MORE_RANDOM = more;
            this.r = r;

            // Angles used for Radial Map Generation
            bumps = r.Next(5) + 1;
            startAngle = r.NextDouble() * 2 * Math.PI;
            dipAngle = r.NextDouble() * 2 * Math.PI;
            dipWidth = r.NextDouble() * 0.5 + 0.2;

            // Gets the bounds
            bounds = v.GetPlotBounds();

            // Applies Lloyd Relaxation to the points (moving the points to the center of the voronoi polygons,
            // then recreating the voronoi graph, makes things look more appealing to the eye)
            for (int i = 0; i < numLloyd; ++i)
            {
                List<Point> points = v.SiteCoords();

                foreach (Point p in points)
                {
                    List<Point> region = v.Region(p);
                    double x = 0;
                    double y = 0;

                    foreach (Point c in region)
                    {
                        x += c.x;
                        y += c.y;
                    }

                    x /= region.Count;
                    y /= region.Count;
                    p.x = x;
                    p.y = y;
                }

                v = new Voronoi(points, v.GetPlotBounds());
            }

            // Generates Simplex noise graph for Simplex Noise map generation
            MakeNoise(r.Next());

            // Sets up the graph
            BuildGraph(v);

            // Fixes the corners because the Lloyd relaxation will have ruined them
            ImproveCorners();

            // Assigns the elevations, determines oceans and land
            AssignCornerElevations();
            AssignOceanCoastAndLand();
            RedistributeElevations(LandCorners());
            AssignPolygonElevations();

            // Calculates the slopes, rivers, and general moisture 
            CalculateDownSlopes();
            CreateRivers();
            AssignCornerMoisture();
            RedistributeMoisture(LandCorners());
            AssignPolygonMoisture();

            // Assigns biomes based on height/moisture
            AssignBiomes();

            // Creates noisy edges for each voronoi polygon for a more interesting look (toggleable)
            BuildNoisyEdges();

            // Unused
            pixelCenterMap = new System.Drawing.Bitmap((int) bounds.width, (int) bounds.width);
        }

        #endregion Constructor

        #region ImproveCorners

        private void ImproveCorners()
        {
            Point[] nPoint = new Point[corners.Count];

            foreach (Corner c in corners)
            {
                if (c.border)
                {
                    nPoint[c.index] = c.loc;
                }
                else
                {
                    double x = 0;
                    double y = 0;

                    foreach (Center cn in c.touches)
                    {
                        x += cn.loc.x;
                        y += cn.loc.y;
                    }

                    nPoint[c.index] = new Point(x / c.touches.Count, y / c.touches.Count);
                }
            }

            foreach (Corner c in corners)
            {
                c.loc = nPoint[c.index];
            }

            foreach (DelaunayEdge dE in edges)
            {
                if (dE.v0 != null && dE.v1 != null)
                {
                    dE.SetVoronoi(dE.v0, dE.v1);
                    dE.midPoint = InterpolatePoint(dE.v0.loc, dE.v1.loc, 0.5);
                }
            }
        }

        #endregion ImproveCorners

        #region DrawTriangle (Normal Polygon Rendering)

        // Draws a triangle, for the non-noisy edge map drawing
        private void DrawTriangle(System.Drawing.Graphics g, Corner c1, Corner c2, Center cn,
            System.Drawing.Color color)
        {
            int[] x = new int[3];
            int[] y = new int[3];
            System.Drawing.Point[] points = new System.Drawing.Point[3];

            points[0] = new System.Drawing.Point((int) cn.loc.x, (int) cn.loc.y);
            points[1] = new System.Drawing.Point((int) c1.loc.x, (int) c1.loc.y);
            points[2] = new System.Drawing.Point((int) c2.loc.x, (int) c2.loc.y);

            g.FillPolygon(new System.Drawing.SolidBrush(color), points);
        }

        #endregion DrawTriangle (Normal Polygon Rendering)

        #region CloseEnough

        // Determines if two doubles are within a range
        private bool CloseEnough(double d1, double d2, double diff)
        {
            return Math.Abs(d1 - d2) <= diff;
        }

        #endregion CloseEnough

        #region CreateMap

        // Returns a bitmap consisting of the drawn map (also will paint to Graphics g if that line is uncommented)
        public System.Drawing.Bitmap CreateMap(System.Drawing.Graphics g, bool drawBiomes, bool drawRivers,
            bool drawSites, bool drawCorners, bool drawDelaunay, bool noisyEdges, bool smoothBlending, bool lighting)
        {
            int size = (int) bounds.width;
            System.Drawing.Bitmap img = new System.Drawing.Bitmap(size, size);
            System.Drawing.Graphics g2 = System.Drawing.Graphics.FromImage(img);

            if (drawDelaunay && !drawBiomes)
            {
                g2.Clear(System.Drawing.Color.FromArgb(192, 192, 192));
            }
            else
            {
                g2.Clear(System.Drawing.Color.FromArgb(68, 68, 122));
            }

            Paint(g2, drawBiomes, drawRivers, drawSites, drawCorners, drawDelaunay, false, noisyEdges, smoothBlending,
                lighting);

            return img;
        }

        #endregion CreateMap

        #region Smooth Color Blending

        // Interpolates the color of two polygons to blend the edges
        private System.Drawing.Color BlendBiome(System.Drawing.Color c, Center p, Center q)
        {
            if (q != null && p.water == q.water)
            {
                return InterpolateColor(GetColor(p.biome), GetColor(q.biome), 0.25);
            }

            return c;
        }

        // Color interpolation function
        private System.Drawing.Color InterpolateColor(System.Drawing.Color c1, System.Drawing.Color c2, double f)
        {
            int r = (int) ((1 - f) * (c1.R) + f * (c2.R));
            int g = (int) ((1 - f) * (c1.G) + f * (c2.G));
            int b = (int) ((1 - f) * (c1.B) + f * (c2.B));

            if (r > 255)
            {
                r = 255;
            }
            else if (r < 0)
            {
                r = 0;
            }

            if (g > 255)
            {
                g = 255;
            }
            else if (g < 0)
            {
                g = 0;
            }

            if (b > 255)
            {
                b = 255;
            }
            else if (b < 0)
            {
                b = 0;
            }

            return System.Drawing.Color.FromArgb(r, g, b);
        }

        #endregion Smooth Color Blending

        #region RenderPolygon

        // Draws a voronoi polygon
        private void DrawPolygon(System.Drawing.Graphics g, Center c, System.Drawing.Color color, bool noisyEdges,
            bool smoothBlending, bool lighting, bool drawBiomes)
        {
            Corner eC1 = null;
            Corner eC2 = null;
            c.area = 0;

            foreach (Center n in c.neighbors)
            {
                DelaunayEdge e = EdgeWithCenters(c, n);

                //Experimental Color (Biome Blending)
                if (smoothBlending && drawBiomes)
                {
                    color = BlendBiome(color, c, n);
                }

                //Experimental Lighting (Light Vectors)
                if (lighting && drawBiomes)
                {
                    color = ColorWithSlope(color, c, n, e);
                }


                // If noisy edges is off, draw triangles to create voronoi polygons (center to vertices of each edge)
                if (!noisyEdges)
                {
                    if (e.v0 == null)
                    {
                        continue;
                    }

                    Corner corner1A = e.v0.border ? e.v0 : e.v1;
                    if (corner1A.border)
                    {
                        if (eC1 == null)
                        {
                            eC1 = corner1A;
                        }
                        else
                        {
                            eC2 = corner1A;
                        }
                    }

                    DrawTriangle(g, e.v0, e.v1, c, color);

                    c.area = Math.Abs(c.loc.x * (e.v0.loc.y - e.v1.loc.y) + e.v0.loc.x * (e.v1.loc.y - c.loc.y) +
                                      e.v1.loc.x * (c.loc.y - e.v0.loc.y)) / 2;
                }

                //Experimental Polygon Drawing using noisy (natural) edges
                if (noisyEdges)
                {
                    // For each of the noisy edge paths fill the polygon created betweeen them and the center
                    if (path0[e.index] != null)
                    {
                        List<Point> tPath = path0[e.index];

                        System.Drawing.Point[] tA = new System.Drawing.Point[tPath.Count + 2];
                        tA[0] = new System.Drawing.Point((int) c.loc.x, (int) c.loc.y);
                        for (int i = 0; i < tPath.Count; i++)
                        {
                            tA[i + 1] = new System.Drawing.Point((int) tPath[i].x, (int) tPath[i].y);
                        }
                        tA[tA.Count() - 1] = new System.Drawing.Point((int) c.loc.x, (int) c.loc.y);

                        g.FillPolygon(new System.Drawing.SolidBrush(color), tA);
                    }

                    if (path1[e.index] != null)
                    {
                        List<Point> tPath = path1[e.index];

                        System.Drawing.Point[] tA = new System.Drawing.Point[tPath.Count + 2];
                        tA[0] = new System.Drawing.Point((int) c.loc.x, (int) c.loc.y);
                        for (int i = 0; i < tPath.Count; i++)
                        {
                            tA[i + 1] = new System.Drawing.Point((int) tPath[i].x, (int) tPath[i].y);
                        }
                        tA[tA.Count() - 1] = new System.Drawing.Point((int) c.loc.x, (int) c.loc.y);

                        g.FillPolygon(new System.Drawing.SolidBrush(color), tA);
                    }
                }
            }

            if (eC2 != null)
            {
                if (CloseEnough(eC1.loc.x, eC2.loc.x, 1))
                {
                    DrawTriangle(g, eC1, eC2, c, color);
                }
                else
                {
                    System.Drawing.Point[] points = new System.Drawing.Point[4];
                    points[0] = new System.Drawing.Point((int) c.loc.x, (int) c.loc.y);
                    points[1] = new System.Drawing.Point((int) eC1.loc.x, (int) eC1.loc.y);

                    int tX = (int) ((CloseEnough(eC1.loc.x, bounds.x, 1) || CloseEnough(eC2.loc.x, bounds.x, 0.5))
                        ? bounds.x
                        : bounds.right);
                    int tY = (int) ((CloseEnough(eC1.loc.y, bounds.y, 1) || CloseEnough(eC2.loc.y, bounds.y, 0.5))
                        ? bounds.y
                        : bounds.bottom);

                    points[2] = new System.Drawing.Point(tX, tY);
                    points[3] = new System.Drawing.Point((int) eC2.loc.x, (int) eC2.loc.y);

                    g.FillPolygon(new System.Drawing.SolidBrush(color), points);
                    c.area += 0;
                }
            }
        }

        #endregion RenderPolygon

        #region Paint

        // Paints the map to the graphics
        public void Paint(System.Drawing.Graphics g, bool drawBiomes, bool drawRivers, bool drawSites, bool drawCorners,
            bool drawDelaunay, bool drawVoronoi, bool noisyEdges, bool smoothBlending, bool lighting)
        {
            int numSites = centers.Count;

            // If we want to just see the dalaunay, set the color of the graph to gray
            System.Drawing.Color[] defaultColors = null;
            if (!drawBiomes && drawDelaunay)
            {
                defaultColors = new System.Drawing.Color[numSites];
                for (int i = 0; i < defaultColors.Count(); i++)
                {
                    defaultColors[i] = System.Drawing.Color.FromArgb(192, 192, 192);
                }
            }
            // Otherwise show colorful voronoi polygons!
            else if (!drawBiomes)
            {
                defaultColors = new System.Drawing.Color[numSites];
                for (int i = 0; i < defaultColors.Count(); i++)
                {
                    defaultColors[i] = System.Drawing.Color.FromArgb(r.Next(255), r.Next(255), r.Next(255));
                }
            }

            // Draw the voronoi polygons
            foreach (Center c in centers)
            {
                DrawPolygon(g, c, drawBiomes ? GetColor(c.biome) : defaultColors[c.index], noisyEdges, smoothBlending,
                    lighting, drawBiomes);
            }

            // Draw the delaunay edges if wanted, draw rivers if wanted
            foreach (DelaunayEdge e in edges)
            {
                if (drawDelaunay)
                {
                    g.DrawLine(System.Drawing.Pens.Yellow, (int) e.d0.loc.x, (int) e.d0.loc.y, (int) e.d1.loc.x,
                        (int) e.d1.loc.y);
                }
                if (drawRivers && e.river > 0)
                {
                    g.DrawLine(new System.Drawing.Pen(RIVER, (1 + (int) Math.Sqrt(e.river * 2))), (int) e.v0.loc.x,
                        (int) e.v0.loc.y, (int) e.v1.loc.x, (int) e.v1.loc.y);
                }
            }

            // Draw the centers of each voronoi polygon
            if (drawSites)
            {
                foreach (Center c in centers)
                {
                    g.FillEllipse(System.Drawing.Brushes.Black, (int) (c.loc.x - 2), (int) (c.loc.y - 2), 4, 4);
                }
            }

            // Draw the corners of eahc voronoi polygon
            if (drawCorners)
            {
                foreach (Corner c in corners)
                {
                    g.FillEllipse(System.Drawing.Brushes.Black, (int) (c.loc.x - 2), (int) (c.loc.y - 2), 4, 4);
                }
            }

            // Draw the bounds
            g.DrawRectangle(System.Drawing.Pens.White, (int) bounds.x, (int) bounds.y, (int) bounds.width,
                (int) bounds.height);
        }

        #endregion Paint

        #region BuildGraph

        private void BuildGraph(Voronoi v)
        {
            Dictionary<Point, Center> pCMap = new Dictionary<Point, Center>();
            List<Point> points = v.SiteCoords();

            // For each point create a center
            foreach (Point p in points)
            {
                Center c = new Center();
                c.loc = p;
                c.index = centers.Count;
                centers.Add(c);
                pCMap[p] = c;
            }

            // For each center, assign a region
            foreach (Center c in centers)
            {
                v.Region(c.loc);
            }

            List<Edge> libEdges = v.Edges();
            Dictionary<int, Corner> pCRMap = new Dictionary<int, Corner>();

            // For each edge in the voronoi edges
            foreach (Edge e in libEdges)
            {
                LineSegment vEdge = e.VoronoiEdge();
                LineSegment dEdge = e.DelaunayLine();
                DelaunayEdge dE = new DelaunayEdge();
                dE.index = edges.Count;
                edges.Add(dE);

                // Setup midpoints for viable voronoi edges for noisy edge generation
                if (vEdge.p0 != null && vEdge.p1 != null)
                {
                    dE.midPoint = InterpolatePoint(vEdge.p0, vEdge.p1, 0.5);
                }

                // Create corners for the voronoi vertices of the edge
                dE.v0 = MakeCorner(pCRMap, vEdge.p0);
                dE.v1 = MakeCorner(pCRMap, vEdge.p1);
                dE.d0 = pCMap[dEdge.p0];
                dE.d1 = pCMap[dEdge.p1];

                // Add borders for delaunay edges
                if (dE.d0 != null)
                {
                    dE.d0.borders.Add(dE);
                }
                if (dE.d1 != null)
                {
                    dE.d1.borders.Add(dE);
                }
                if (dE.v0 != null)
                {
                    dE.v0.protrudes.Add(dE);
                }
                if (dE.v1 != null)
                {
                    dE.v1.protrudes.Add(dE);
                }

                if (dE.d0 != null && dE.d1 != null)
                {
                    AddToCenterList(dE.d0.neighbors, dE.d1);
                    AddToCenterList(dE.d1.neighbors, dE.d0);
                }
                if (dE.v0 != null && dE.v1 != null)
                {
                    AddToCornerList(dE.v0.adjacent, dE.v1);
                    AddToCornerList(dE.v1.adjacent, dE.v0);
                }

                if (dE.d0 != null)
                {
                    AddToCornerList(dE.d0.corners, dE.v0);
                    AddToCornerList(dE.d0.corners, dE.v1);
                }
                if (dE.d1 != null)
                {
                    AddToCornerList(dE.d1.corners, dE.v0);
                    AddToCornerList(dE.d1.corners, dE.v1);
                }

                if (dE.v0 != null)
                {
                    AddToCenterList(dE.v0.touches, dE.d0);
                    AddToCenterList(dE.v0.touches, dE.d1);
                }
                if (dE.v1 != null)
                {
                    AddToCenterList(dE.v1.touches, dE.d0);
                    AddToCenterList(dE.v1.touches, dE.d1);
                }
            }
        }

        #endregion BuildGraph

        #region Add To <Corner/Center> List

        private void AddToCornerList(List<Corner> l, Corner c)
        {
            if (c != null && !l.Contains(c))
            {
                l.Add(c);
            }
        }

        private void AddToCenterList(List<Center> l, Center c)
        {
            if (c != null && !l.Contains(c))
            {
                l.Add(c);
            }
        }

        #endregion Add To <Corner/Center> List

        #region MakeCorner

        private Corner MakeCorner(Dictionary<int, Corner> pCRMap, Point p)
        {
            if (p == null)
            {
                return null;
            }

            int index = (int) ((int) p.x + (int) (p.y) * bounds.width * 2);

            Corner c;
            if (!pCRMap.ContainsKey(index))
            {
                c = new Corner();
                c.loc = p;
                c.border = bounds.LiesOnAxes(p);
                c.index = corners.Count;
                corners.Add(c);
                pCRMap[index] = c;
            }
            else
            {
                c = pCRMap[index];
            }

            return c;
        }

        #endregion MakeCorner

        #region Ocean/Land Gen

        private bool IsWater(Point p)
        {
            p = new Point(2 * (p.x / bounds.width - 0.5), 2 * (p.y / bounds.height - 0.5));
            double angle = Math.Atan2(p.y, p.x);
            double length = 0.5 * (Math.Max(Math.Abs(p.x), Math.Abs(p.y)) + p.Length());
            double r1 = 0.5 + 0.40 * Math.Sin(startAngle + bumps * angle + Math.Cos((bumps + 3) * angle));
            double r2 = 0.7 - 0.20 * Math.Sin(startAngle + bumps * angle - Math.Cos((bumps + 2) * angle));
            if (Math.Abs(angle - dipAngle) < dipWidth
                || Math.Abs(angle - dipAngle + 2 * Math.PI) < dipWidth
                || Math.Abs(angle - dipAngle - 2 * Math.PI) < dipWidth)
            {
                r1 = 0.2;
                r2 = 0.2;
            }

            return !(length < r1 || (length > r1 * ISLAND_FACTOR && length < r2));
        }

        private bool IsWaterNoise(Point p)
        {
            p = new Point(2 * (p.x / bounds.width - 0.5), 2 * (p.y / bounds.height - 0.5));
            int tX = (int) ((p.x + 1) * 128);
            int tY = (int) ((p.y + 1) * 128);

            if (tX < 0)
            {
                tX = 0;
            }
            else if (tX > 255)
            {
                tX = 255;
            }

            if (tY < 0)
            {
                tY = 0;
            }
            else if (tY > 255)
            {
                tY = 255;
            }

            return noise[tX, tY] < 0.3 + 0.3 * p.L2();
        }

        private void AssignOceanCoastAndLand()
        {
            Queue<Center> queue = new Queue<Center>();
            double waterThreshold = 0.40;

            foreach (Center c in centers)
            {
                int numWater = 0;

                foreach (Corner cr in c.corners)
                {
                    if (cr.border)
                    {
                        c.border = true;
                        c.water = true;
                        c.ocean = true;
                        queue.Enqueue(c);
                    }
                    if (cr.water)
                    {
                        numWater++;
                    }
                }

                c.water = (c.ocean || ((double) numWater / c.corners.Count >= waterThreshold));
            }

            while (queue.Count > 0)
            {
                Center c = queue.Dequeue();

                foreach (Center n in c.neighbors)
                {
                    if (n.water && !n.ocean)
                    {
                        n.ocean = true;
                        queue.Enqueue(n);
                    }
                }
            }

            foreach (Center c in centers)
            {
                bool oNeighbor = false;
                bool lNeighbor = false;

                foreach (Center n in c.neighbors)
                {
                    oNeighbor |= n.ocean;
                    lNeighbor |= !n.water;
                }

                c.coast = oNeighbor & lNeighbor;
            }

            foreach (Corner c in corners)
            {
                int nO = 0;
                int nL = 0;

                foreach (Center cn in c.touches)
                {
                    nO += cn.ocean ? 1 : 0;
                    nL += !cn.water ? 1 : 0;
                }

                c.ocean = nO == c.touches.Count;
                c.coast = nO > 0 && nL > 0;
                c.water = c.border || ((nL != c.touches.Count) && !c.coast);
            }
        }

        private List<Corner> LandCorners()
        {
            List<Corner> l = new List<Corner>();

            foreach (Corner c in corners)
            {
                if (!c.ocean && !c.coast)
                {
                    l.Add(c);
                }
            }

            return l;
        }

        #endregion Ocean/Land Gen

        #region Elevation

        private void AssignCornerElevations()
        {
            Queue<Corner> queue = new Queue<Corner>();

            foreach (Corner c in corners)
            {
                c.water = IsWaterNoise(c.loc);
                if (c.border)
                {
                    c.elevation = 0;
                    queue.Enqueue(c);
                }
                else
                {
                    c.elevation = Double.MaxValue;
                }
            }

            while (queue.Count > 0)
            {
                Corner c = queue.Dequeue();

                foreach (Corner a in c.adjacent)
                {
                    double newE = 0.01 + c.elevation;

                    if (!c.water && !a.water)
                    {
                        newE += 1;

                        if (MORE_RANDOM)
                        {
                            newE += r.NextDouble();
                        }
                    }

                    if (newE < a.elevation)
                    {
                        a.elevation = newE;
                        queue.Enqueue(a);
                    }
                }
            }
        }

        private void RedistributeElevations(List<Corner> lC)
        {
            lC.Sort(new CornerComparer());

            double SCALE_FACTOR = 1.2;

            for (int i = 0; i < lC.Count; ++i)
            {
                double y = (double) i / (lC.Count - 1);
                double x = Math.Sqrt(SCALE_FACTOR) - Math.Sqrt(SCALE_FACTOR * (1 - y));
                x = Math.Min(x, 1);
                lC[i].elevation = x;
            }

            foreach (Corner c in corners)
            {
                if (c.ocean || c.coast)
                {
                    c.elevation = 0.0;
                }
            }
        }

        private void AssignPolygonElevations()
        {
            foreach (Center cn in centers)
            {
                double total = 0;

                foreach (Corner cr in cn.corners)
                {
                    total += cr.elevation;
                }

                cn.elevation = total / cn.corners.Count;
            }
        }

        #endregion Elevation

        #region Rivers

        private void CalculateDownSlopes()
        {
            foreach (Corner c in corners)
            {
                Corner down = c;

                foreach (Corner a in c.adjacent)
                {
                    if (a.elevation <= down.elevation)
                    {
                        down = a;
                    }
                }

                c.downslope = down;
            }
        }

        private void CreateRivers()
        {
            for (int i = 0; i < bounds.width / 2; i++)
            {
                Corner c = corners[r.Next(corners.Count)];

                if (c.ocean || c.elevation < 0.3 || c.elevation > 0.9)
                {
                    continue;
                }

                if (r.Next(5) != 0)
                {
                    continue;
                }

                while (!c.coast)
                {
                    if (c == c.downslope)
                    {
                        break;
                    }

                    DelaunayEdge dE = LookupEdgeFromCorner(c, c.downslope);

                    if (!dE.v0.water || !dE.v1.water)
                    {
                        dE.river++;
                        c.river++;
                        c.downslope.river++;
                    }

                    c = c.downslope;
                }
            }
        }

        #endregion Rivers

        #region LookupEdge From ...

        private DelaunayEdge LookupEdgeFromCorner(Corner c, Corner down)
        {
            foreach (DelaunayEdge dE in c.protrudes)
            {
                if (dE.v0 == down || dE.v1 == down)
                {
                    return dE;
                }
            }

            return null;
        }

        private DelaunayEdge LookupEdgeFromCenter(Center p, Center r)
        {
            foreach (DelaunayEdge e in p.borders)
                if (e.d0 == r || e.d1 == r)
                    return e;
            return null;
        }

        // Gets the edges connecting two polygons based on their respective centers
        private DelaunayEdge EdgeWithCenters(Center c1, Center c2)
        {
            foreach (DelaunayEdge dE in c1.borders)
            {
                if (dE.d0 == c2 || dE.d1 == c2)
                {
                    return dE;
                }
            }

            return null;
        }

        #endregion LookupEdge From ...

        #region Moisture

        private void AssignCornerMoisture()
        {
            Queue<Corner> queue = new Queue<Corner>();

            foreach (Corner c in corners)
            {
                if ((c.water || c.river > 0) && !c.ocean)
                {
                    c.moisture = c.river > 0 ? Math.Min(3.0, (0.2 * c.river)) : 1.0;
                    queue.Enqueue(c);
                }
                else
                {
                    c.moisture = 0.0;
                }
            }

            while (queue.Count > 0)
            {
                Corner c = queue.Dequeue();

                foreach (Corner a in c.adjacent)
                {
                    double nMoisture = 0.9 * c.moisture;

                    if (nMoisture > a.moisture)
                    {
                        a.moisture = nMoisture;
                        queue.Enqueue(a);
                    }
                }
            }

            foreach (Corner c in corners)
            {
                if (c.ocean || c.coast)
                {
                    c.moisture = 1.0;
                }
            }
        }

        private void RedistributeMoisture(List<Corner> lc)
        {
            lc.Sort(new CornerMoistureComparer());

            for (int i = 0; i < lc.Count; ++i)
            {
                lc[i].moisture = (double) i / (lc.Count - 1);
            }
        }

        private void AssignPolygonMoisture()
        {
            foreach (Center c in centers)
            {
                double total = 0.0;

                foreach (Corner cr in c.corners)
                {
                    total += cr.moisture;
                }

                c.moisture = total / c.corners.Count;
            }
        }

        #endregion Moisture

        #region AssignBiomes

        private void AssignBiomes()
        {
            foreach (Center c in centers)
            {
                c.biome = GetBiome(c);
            }
        }

        #endregion AssignBiomes

        #region SimplexNoise Generation

        public void MakeNoise(int seed)
        {
            SimplexNoise sNoise = new SimplexNoise(256, 0.57, seed);
            noise = sNoise.GenerateMap(256, 256);
        }

        #endregion Simplex Noise Generation

        #region NoisyEdges

        private List<Point>[] path0;
        private List<Point>[] path1;
        private const double NLT = 0.5;

        public void BuildNoisyEdges()
        {
            path0 = new List<Point>[edges.Count];
            path1 = new List<Point>[edges.Count];

            foreach (Center p in centers)
            {
                foreach (DelaunayEdge edge in p.borders)
                {
                    if (edge.d0 != null && edge.d1 != null && edge.v0 != null && edge.v1 != null)
                    {
                        if (path0[edge.index] == null)
                        {
                            Point t = InterpolatePoint(edge.v0.loc, edge.d0.loc, NLT);
                            Point q = InterpolatePoint(edge.v0.loc, edge.d1.loc, NLT);
                            Point r = InterpolatePoint(edge.v1.loc, edge.d0.loc, NLT);
                            Point s = InterpolatePoint(edge.v1.loc, edge.d1.loc, NLT);

                            int minL = 10;
                            if (edge.d0.biome != edge.d1.biome) minL = 3;
                            if (edge.d0.ocean && edge.d1.ocean) minL = 100;
                            if (edge.d0.coast || edge.d1.coast) minL = 1;
                            if (edge.river > 0) minL = 1;

                            path0[edge.index] = BuildNoisyLineSegments(edge.v0.loc, t, edge.midPoint, q, minL);
                            path1[edge.index] = BuildNoisyLineSegments(edge.v1.loc, s, edge.midPoint, r, minL);
                        }
                    }
                }
            }
        }

        private List<Point> BuildNoisyLineSegments(Point A, Point B, Point C, Point D, int minL)
        {
            List<Point> points = new List<Point>();

            points.Add(A);
            Subdivide(points, A, B, C, D, minL);
            points.Add(C);

            return points;
        }

        private void Subdivide(List<Point> points, Point A, Point B, Point C, Point D, int minL)
        {
            if (PointLength(A, C) < minL || PointLength(B, D) < minL)
            {
                return;
            }

            double p = (r.NextDouble() * 0.6) + 0.2;
            double q = (r.NextDouble() * 0.6) + 0.2;

            Point E = InterpolatePoint(A, D, p);
            Point F = InterpolatePoint(B, C, p);
            Point G = InterpolatePoint(A, B, q);
            Point I = InterpolatePoint(D, C, q);
            Point H = InterpolatePoint(E, F, q);

            double s = 1.0 - ((r.NextDouble() * 0.8) - 0.4);
            double t = 1.0 - ((r.NextDouble() * 0.8) - 0.4);

            Subdivide(points, A, InterpolatePoint(G, B, s), H, InterpolatePoint(E, D, t), minL);
            points.Add(H);
            Subdivide(points, H, InterpolatePoint(F, C, s), C, InterpolatePoint(I, D, t), minL);
        }

        private double PointLength(Point p1, Point p2)
        {
            double nX = (p1.x - p2.x);
            double nY = (p1.y - p2.y);
            return Math.Sqrt((nX * nX) + (nY * nY));
        }

        private Point InterpolatePoint(Point p1, Point p2, double f)
        {
            double x = f * p1.x + ((1 - f) * p2.x);
            double y = f * p1.y + ((1 - f) * p2.y);

            return new Point(x, y);
        }

        #endregion NoisyEdges

        #region Lighting

        Vector3D lightVector = new Vector3D(-1.0, -1.0, 0.0);

        public double CalculateLighting(Center p, Corner r, Corner s)
        {
            Vector3D A = new Vector3D(p.loc.x, p.loc.y, p.elevation);
            Vector3D B = new Vector3D(r.loc.x, r.loc.y, r.elevation);
            Vector3D C = new Vector3D(s.loc.x, s.loc.y, s.elevation);
            Vector3D normal = Vector3D.CrossProduct(Vector3D.Subtract(B, A), Vector3D.Subtract(C, A));

            if (normal.z < 0)
            {
                normal.x *= -1.0;
                normal.y *= -1.0;
                normal.z *= -1.0;
            }
            normal = Vector3D.Normalize(normal);

            double light = 0.5 + 35 * Vector3D.DotProduct(normal, lightVector);
            if (light < 0)
            {
                light = 0.0;
            }
            if (light > 1)
            {
                light = 1.0;
            }
            if (light == Double.NaN)
            {
                light = 0.0;
            }

            return light;
        }

        public System.Drawing.Color ColorWithSlope(System.Drawing.Color c, Center p, Center q, DelaunayEdge e)
        {
            Corner r = e.v0;
            Corner s = e.v1;
            if (r == null || s == null)
            {
                return c;
            }
            else if (p.water)
            {
                return c;
            }

            if (q != null && p.water == q.water)
            {
                c = InterpolateColor(c, GetColor(q.biome), 0.4);
            }

            System.Drawing.Color low = InterpolateColor(c, System.Drawing.Color.FromArgb(51, 51, 51), 0.7);
            System.Drawing.Color high = InterpolateColor(c, System.Drawing.Color.FromArgb(255, 255, 255), 0.3);

            double light = CalculateLighting(p, r, s);
            if (light < 0.5)
            {
                return InterpolateColor(low, c, light * 2);
            }
            else
            {
                return InterpolateColor(c, high, light * 2 - 1);
            }
        }

        #endregion Lighting

        #region Vector3D

        public class Vector3D
        {
            public double x, y, z;

            public Vector3D(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public static Vector3D Subtract(Vector3D v1, Vector3D v2)
            {
                return new Vector3D(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
            }

            public static double DotProduct(Vector3D v1, Vector3D v2)
            {
                return (v1.x * v2.x) + (v1.y * v2.y) + (v1.z * v2.z);
            }

            public static Vector3D CrossProduct(Vector3D v1, Vector3D v2)
            {
                double nX = v1.y * v2.z - v1.z * v2.y;
                double nY = v1.z * v2.x - v1.x * v2.z;
                double nZ = v1.x * v2.y - v1.y * v2.x;

                return new Vector3D(nX, nY, nZ);
            }

            public static Vector3D Normalize(Vector3D v)
            {
                if (Magnitude(v) <= 0)
                {
                    return new Vector3D(0, 0, 0);
                }

                double nX = v.x / Magnitude(v);
                double nY = v.y / Magnitude(v);
                double nZ = v.z / Magnitude(v);

                return new Vector3D(nX, nY, nZ);
            }

            public static double Magnitude(Vector3D v)
            {
                return Math.Sqrt((v.x * v.x) + (v.y * v.y) + (v.z * v.z));
            }

            public String Print()
            {
                return this.x.ToString("#.##") + "," + this.y.ToString("#.##") + "," + this.z.ToString("#.##");
            }
        }

        #endregion Vector3D

        #region Comparers

        public class CornerComparer : Comparer<Corner>
        {
            public override int Compare(Corner x, Corner y)
            {
                if (x.elevation > y.elevation)
                {
                    return 1;
                }
                else if (x.elevation < y.elevation)
                {
                    return -1;
                }

                return 0;
            }
        }

        public class CornerMoistureComparer : Comparer<Corner>
        {
            public override int Compare(Corner x, Corner y)
            {
                if (x.moisture > y.moisture)
                {
                    return 1;
                }
                else if (x.moisture < y.moisture)
                {
                    return -1;
                }
                return 0;
            }
        }

        #endregion Comparers
    }
}