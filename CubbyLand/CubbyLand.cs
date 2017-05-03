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
        private Random seedGen;
        private Bitmap img;
        private Bitmap img2;
        private Bitmap fImg;

        #endregion Voronoi Variables

        public CubbyLand()
        {
            InitializeComponent();
            g = this.MapPanel.CreateGraphics();
        }

        private void CubbyLand_Load(object sender, EventArgs e)
        {

        }
    }
}
