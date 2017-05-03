namespace CubbyLand
{
    partial class CubbyLand
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ToolPanel = new System.Windows.Forms.Panel();
            this.SaveButton = new System.Windows.Forms.Button();
            this.LightingCheck = new System.Windows.Forms.CheckBox();
            this.BlendCheck = new System.Windows.Forms.CheckBox();
            this.OverlayCheck = new System.Windows.Forms.CheckBox();
            this.NoisyCheck = new System.Windows.Forms.CheckBox();
            this.RHeightCheck = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.BoundsBox = new System.Windows.Forms.TextBox();
            this.RandomCheck = new System.Windows.Forms.CheckBox();
            this.RefreshButton = new System.Windows.Forms.Button();
            this.DelaunayCheck = new System.Windows.Forms.CheckBox();
            this.SitesCheck = new System.Windows.Forms.CheckBox();
            this.CornerCheck = new System.Windows.Forms.CheckBox();
            this.RiverCheck = new System.Windows.Forms.CheckBox();
            this.BiomeCheck = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.seedBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.nLloydBox = new System.Windows.Forms.TextBox();
            this.nSitesBox = new System.Windows.Forms.TextBox();
            this.CreateButton = new System.Windows.Forms.Button();
            this.MapPanel = new System.Windows.Forms.Panel();
            this.MapImageBox = new System.Windows.Forms.PictureBox();
            this.ToolPanel.SuspendLayout();
            this.MapPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MapImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ToolPanel
            // 
            this.ToolPanel.BackColor = System.Drawing.SystemColors.Control;
            this.ToolPanel.Controls.Add(this.SaveButton);
            this.ToolPanel.Controls.Add(this.LightingCheck);
            this.ToolPanel.Controls.Add(this.BlendCheck);
            this.ToolPanel.Controls.Add(this.OverlayCheck);
            this.ToolPanel.Controls.Add(this.NoisyCheck);
            this.ToolPanel.Controls.Add(this.RHeightCheck);
            this.ToolPanel.Controls.Add(this.label4);
            this.ToolPanel.Controls.Add(this.BoundsBox);
            this.ToolPanel.Controls.Add(this.RandomCheck);
            this.ToolPanel.Controls.Add(this.RefreshButton);
            this.ToolPanel.Controls.Add(this.DelaunayCheck);
            this.ToolPanel.Controls.Add(this.SitesCheck);
            this.ToolPanel.Controls.Add(this.CornerCheck);
            this.ToolPanel.Controls.Add(this.RiverCheck);
            this.ToolPanel.Controls.Add(this.BiomeCheck);
            this.ToolPanel.Controls.Add(this.label3);
            this.ToolPanel.Controls.Add(this.seedBox);
            this.ToolPanel.Controls.Add(this.label2);
            this.ToolPanel.Controls.Add(this.label1);
            this.ToolPanel.Controls.Add(this.nLloydBox);
            this.ToolPanel.Controls.Add(this.nSitesBox);
            this.ToolPanel.Controls.Add(this.CreateButton);
            this.ToolPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ToolPanel.Location = new System.Drawing.Point(0, 844);
            this.ToolPanel.Name = "ToolPanel";
            this.ToolPanel.Size = new System.Drawing.Size(934, 68);
            this.ToolPanel.TabIndex = 0;
            // 
            // SaveButton
            // 
            this.SaveButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.SaveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.SaveButton.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveButton.Location = new System.Drawing.Point(856, 3);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 62);
            this.SaveButton.TabIndex = 17;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = false;
            // 
            // LightingCheck
            // 
            this.LightingCheck.AutoSize = true;
            this.LightingCheck.Checked = true;
            this.LightingCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.LightingCheck.Location = new System.Drawing.Point(623, 44);
            this.LightingCheck.Name = "LightingCheck";
            this.LightingCheck.Size = new System.Drawing.Size(63, 17);
            this.LightingCheck.TabIndex = 15;
            this.LightingCheck.Text = "Lighting";
            this.LightingCheck.UseVisualStyleBackColor = true;
            // 
            // BlendCheck
            // 
            this.BlendCheck.AutoSize = true;
            this.BlendCheck.Checked = true;
            this.BlendCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.BlendCheck.Location = new System.Drawing.Point(526, 25);
            this.BlendCheck.Name = "BlendCheck";
            this.BlendCheck.Size = new System.Drawing.Size(85, 17);
            this.BlendCheck.TabIndex = 11;
            this.BlendCheck.Text = "Biome Blend";
            this.BlendCheck.UseVisualStyleBackColor = true;
            // 
            // OverlayCheck
            // 
            this.OverlayCheck.AutoSize = true;
            this.OverlayCheck.Checked = true;
            this.OverlayCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.OverlayCheck.Location = new System.Drawing.Point(623, 6);
            this.OverlayCheck.Name = "OverlayCheck";
            this.OverlayCheck.Size = new System.Drawing.Size(92, 17);
            this.OverlayCheck.TabIndex = 13;
            this.OverlayCheck.Text = "Noise Overlay";
            this.OverlayCheck.UseVisualStyleBackColor = true;
            // 
            // NoisyCheck
            // 
            this.NoisyCheck.AutoSize = true;
            this.NoisyCheck.Checked = true;
            this.NoisyCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.NoisyCheck.Location = new System.Drawing.Point(623, 25);
            this.NoisyCheck.Name = "NoisyCheck";
            this.NoisyCheck.Size = new System.Drawing.Size(85, 17);
            this.NoisyCheck.TabIndex = 14;
            this.NoisyCheck.Text = "Noisy Edges";
            this.NoisyCheck.UseVisualStyleBackColor = true;
            // 
            // RHeightCheck
            // 
            this.RHeightCheck.AutoSize = true;
            this.RHeightCheck.Location = new System.Drawing.Point(371, 25);
            this.RHeightCheck.Name = "RHeightCheck";
            this.RHeightCheck.Size = new System.Drawing.Size(61, 17);
            this.RHeightCheck.TabIndex = 6;
            this.RHeightCheck.Tag = "Adds random elevation to give a different possibly better appearance";
            this.RHeightCheck.Text = "R. Elev";
            this.RHeightCheck.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(193, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 997;
            this.label4.Text = "Bounds (Size):";
            // 
            // BoundsBox
            // 
            this.BoundsBox.Location = new System.Drawing.Point(196, 36);
            this.BoundsBox.Name = "BoundsBox";
            this.BoundsBox.Size = new System.Drawing.Size(69, 20);
            this.BoundsBox.TabIndex = 3;
            // 
            // RandomCheck
            // 
            this.RandomCheck.AutoSize = true;
            this.RandomCheck.Checked = true;
            this.RandomCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.RandomCheck.Location = new System.Drawing.Point(371, 44);
            this.RandomCheck.Name = "RandomCheck";
            this.RandomCheck.Size = new System.Drawing.Size(66, 17);
            this.RandomCheck.TabIndex = 5;
            this.RandomCheck.Text = "Random";
            this.RandomCheck.UseVisualStyleBackColor = true;
            // 
            // RefreshButton
            // 
            this.RefreshButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.RefreshButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.RefreshButton.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RefreshButton.Location = new System.Drawing.Point(775, 3);
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(75, 62);
            this.RefreshButton.TabIndex = 16;
            this.RefreshButton.Text = "Refresh!";
            this.RefreshButton.UseVisualStyleBackColor = false;
            // 
            // DelaunayCheck
            // 
            this.DelaunayCheck.AutoSize = true;
            this.DelaunayCheck.Location = new System.Drawing.Point(449, 44);
            this.DelaunayCheck.Name = "DelaunayCheck";
            this.DelaunayCheck.Size = new System.Drawing.Size(71, 17);
            this.DelaunayCheck.TabIndex = 9;
            this.DelaunayCheck.Text = "Delaunay";
            this.DelaunayCheck.UseVisualStyleBackColor = true;
            // 
            // SitesCheck
            // 
            this.SitesCheck.AutoSize = true;
            this.SitesCheck.Location = new System.Drawing.Point(449, 6);
            this.SitesCheck.Name = "SitesCheck";
            this.SitesCheck.Size = new System.Drawing.Size(49, 17);
            this.SitesCheck.TabIndex = 7;
            this.SitesCheck.Text = "Sites";
            this.SitesCheck.UseVisualStyleBackColor = true;
            // 
            // CornerCheck
            // 
            this.CornerCheck.AutoSize = true;
            this.CornerCheck.Location = new System.Drawing.Point(449, 25);
            this.CornerCheck.Name = "CornerCheck";
            this.CornerCheck.Size = new System.Drawing.Size(62, 17);
            this.CornerCheck.TabIndex = 8;
            this.CornerCheck.Text = "Corners";
            this.CornerCheck.UseVisualStyleBackColor = true;
            // 
            // RiverCheck
            // 
            this.RiverCheck.AutoSize = true;
            this.RiverCheck.Checked = true;
            this.RiverCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.RiverCheck.Location = new System.Drawing.Point(526, 44);
            this.RiverCheck.Name = "RiverCheck";
            this.RiverCheck.Size = new System.Drawing.Size(56, 17);
            this.RiverCheck.TabIndex = 12;
            this.RiverCheck.Text = "Rivers";
            this.RiverCheck.UseVisualStyleBackColor = true;
            // 
            // BiomeCheck
            // 
            this.BiomeCheck.AutoSize = true;
            this.BiomeCheck.Checked = true;
            this.BiomeCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.BiomeCheck.Location = new System.Drawing.Point(526, 6);
            this.BiomeCheck.Name = "BiomeCheck";
            this.BiomeCheck.Size = new System.Drawing.Size(60, 17);
            this.BiomeCheck.TabIndex = 10;
            this.BiomeCheck.Text = "Biomes";
            this.BiomeCheck.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(274, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 996;
            this.label3.Text = "Seed:";
            // 
            // seedBox
            // 
            this.seedBox.Location = new System.Drawing.Point(273, 36);
            this.seedBox.Name = "seedBox";
            this.seedBox.Size = new System.Drawing.Size(92, 20);
            this.seedBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(147, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 998;
            this.label2.Text = "Lloyd:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(84, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 999;
            this.label1.Text = "Num Sites:";
            // 
            // nLloydBox
            // 
            this.nLloydBox.Location = new System.Drawing.Point(150, 36);
            this.nLloydBox.Name = "nLloydBox";
            this.nLloydBox.Size = new System.Drawing.Size(40, 20);
            this.nLloydBox.TabIndex = 2;
            // 
            // nSitesBox
            // 
            this.nSitesBox.Location = new System.Drawing.Point(84, 36);
            this.nSitesBox.Name = "nSitesBox";
            this.nSitesBox.Size = new System.Drawing.Size(60, 20);
            this.nSitesBox.TabIndex = 1;
            // 
            // CreateButton
            // 
            this.CreateButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.CreateButton.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CreateButton.Location = new System.Drawing.Point(3, 3);
            this.CreateButton.Name = "CreateButton";
            this.CreateButton.Size = new System.Drawing.Size(75, 62);
            this.CreateButton.TabIndex = 0;
            this.CreateButton.Text = "Create!";
            this.CreateButton.UseVisualStyleBackColor = false;
            this.CreateButton.Click += new System.EventHandler(this.CreateButton_Click);
            // 
            // MapPanel
            // 
            this.MapPanel.AutoScroll = true;
            this.MapPanel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.MapPanel.Controls.Add(this.MapImageBox);
            this.MapPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MapPanel.Location = new System.Drawing.Point(0, 0);
            this.MapPanel.Name = "MapPanel";
            this.MapPanel.Size = new System.Drawing.Size(934, 844);
            this.MapPanel.TabIndex = 1;
            // 
            // MapImageBox
            // 
            this.MapImageBox.Location = new System.Drawing.Point(0, 0);
            this.MapImageBox.Name = "MapImageBox";
            this.MapImageBox.Size = new System.Drawing.Size(932, 844);
            this.MapImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.MapImageBox.TabIndex = 0;
            this.MapImageBox.TabStop = false;
            // 
            // CubbyLand
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 912);
            this.Controls.Add(this.MapPanel);
            this.Controls.Add(this.ToolPanel);
            this.Name = "CubbyLand";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "CubbyLand";
            this.ToolPanel.ResumeLayout(false);
            this.ToolPanel.PerformLayout();
            this.MapPanel.ResumeLayout(false);
            this.MapPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MapImageBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel ToolPanel;
        private System.Windows.Forms.Button CreateButton;
        private System.Windows.Forms.Panel MapPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox nLloydBox;
        private System.Windows.Forms.TextBox nSitesBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox seedBox;
        private System.Windows.Forms.CheckBox RiverCheck;
        private System.Windows.Forms.CheckBox BiomeCheck;
        private System.Windows.Forms.CheckBox CornerCheck;
        private System.Windows.Forms.CheckBox DelaunayCheck;
        private System.Windows.Forms.CheckBox SitesCheck;
        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.CheckBox RandomCheck;
        private System.Windows.Forms.PictureBox MapImageBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox BoundsBox;
        private System.Windows.Forms.CheckBox RHeightCheck;
        private System.Windows.Forms.CheckBox BlendCheck;
        private System.Windows.Forms.CheckBox OverlayCheck;
        private System.Windows.Forms.CheckBox NoisyCheck;
        private System.Windows.Forms.CheckBox LightingCheck;
        private System.Windows.Forms.Button SaveButton;
    }
}