using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class SiteList
    {
        private List<Site> sites;
        private int curr;
        private bool isSorted;

        public SiteList()
        {
            sites = new List<Site>();
            isSorted = false;
        }

        public int Push(Site s)
        {
            isSorted = false;
            sites.Add(s);
            return sites.Count;
        }

        public int GetLength()
        {
            return sites.Count;
        }

        public Site Next()
        {
            if (isSorted == false)
            {
                Console.WriteLine("Is not sorted!");
            }

            if (curr < sites.Count)
            {
                return sites[curr++];
            }
            else
            {
                return null;
            }
        }

        public Rectangle GetSitesBounds()
        {
            if (isSorted == false)
            {
                Site.SortSites(sites);
                curr = 0;
                isSorted = true;
            }

            double xMin;
            double xMax;
            double yMin;
            double yMax;

            if (sites.Count < 1)
            {
                return new Rectangle(0, 0, 0, 0);
            }

            xMin = double.MaxValue;
            xMax = double.MinValue;

            foreach (Site s in sites)
            {
                if (s.coord.x < xMin)
                {
                    xMin = s.coord.x;
                }
                if (s.coord.x > xMax)
                {
                    xMax = s.coord.x;
                }
            }

            yMin = sites[0].coord.y;
            yMax = sites[sites.Count - 1].coord.y;

            return new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        public List<Point> SiteCoords()
        {
            List<Point> coords = new List<Point>();

            foreach (Site s in sites)
            {
                coords.Add(s.coord);
            }

            return coords;
        }

        public List<Circle> Circles()
        {
            List<Circle> circles = new List<Circle>();

            foreach (Site s in sites)
            {
                double radius = 0;
                Edge nearestEdge = s.NearestEdge();

                if (!nearestEdge.IsPartOfConvexHull())
                {
                    radius = nearestEdge.SitesDistance() * 0.5;
                }

                circles.Add(new Circle(s.coord.x, s.coord.y, radius));
            }

            return circles;
        }

        public List<List<Point>> Regions(Rectangle bounds)
        {
            List<List<Point>> regions = new List<List<Point>>();

            foreach (Site s in sites)
            {
                regions.Add(s.Region(bounds));
            }

            return regions;
        }

        public void Dispose()
        {
            if (sites != null)
            {
                foreach (Site s in sites)
                {
                    s.Dispose();
                }

                sites.Clear();
                sites = null;
            }
        }
    }
}