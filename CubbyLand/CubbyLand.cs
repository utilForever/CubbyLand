using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CubbyLand
{
    public partial class CubbyLand : Form
    {
        #region Drawing Variables

        private Graphics g;
        private bool drawBiomes = true;
        private bool drawRivers = true;
        private bool drawSites = true;
        private bool drawCorners = false;
        private bool drawDelaunay = false;
        private bool isRandom = true;
        private bool overlay = true;
        private bool lighting = true;
        private bool noisyEdges = true;
        private bool smoothBlending = true;

        #endregion Drawing Variables

        #region Voronoi Variables

        private static bool moreRandom = false;
        private int numSites = 6000;
        private int numLloyd = 2;
        private int bounds = 850;
        private int seed = 0;
        private VoronoiGraph voronoiGraph;
        private Random seedGen;
        private Bitmap img;
        private Bitmap img2;
        private Bitmap fImg;

        #endregion Voronoi Variables

        public CubbyLand()
        {
            InitializeComponent();
            g = this.MapPanel.CreateGraphics();

            seedGen = new Random();
        }

        #region Create Button

        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (nSitesBox.Text != "")
            {
                try
                {
                    numSites = int.Parse(nSitesBox.Text);
                }
                catch (Exception)
                {
                    numSites = 6000;
                }
            }
            else
            {
                numSites = 6000;
            }

            if (nLloydBox.Text != "")
            {
                try
                {
                    numLloyd = int.Parse(nLloydBox.Text);
                }
                catch (Exception)
                {
                    numLloyd = 2;
                }
            }
            else
            {
                numLloyd = 2;
            }
            if (BoundsBox.Text != "")
            {
                try
                {
                    bounds = int.Parse(BoundsBox.Text);
                }
                catch (Exception)
                {
                    bounds = 850;
                }
            }
            else
            {
                bounds = 850;
            }
            if (isRandom)
            {
                seed = seedGen.Next();
                seedBox.Text = seed + "";
            }
            else if (seedBox.Text != "")
            {
                try
                {
                    seed = int.Parse(seedBox.Text);
                }
                catch (Exception)
                {
                    seed = seedGen.Next();
                    seedBox.Text = seed + "";
                }
            }
            else
            {
                seed = seedGen.Next();
                seedBox.Text = seed + "";
            }
        }

        #endregion Create Button
    }
}
