namespace Rubberduck.UI.WinForms
{
    partial class Splash
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Splash));
            this.VersionLabel = new System.Windows.Forms.Label();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.ContainerBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.ContainerBox)).BeginInit();
            this.ContainerBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // VersionLabel
            // 
            this.VersionLabel.AccessibleDescription = "Rubberduck version/build number";
            this.VersionLabel.AccessibleName = "VersionLabel";
            this.VersionLabel.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.BackColor = System.Drawing.Color.Transparent;
            this.VersionLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VersionLabel.Location = new System.Drawing.Point(13, 9);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(143, 28);
            this.VersionLabel.TabIndex = 2;
            this.VersionLabel.Text = "v3.0.x (debug)";
            this.VersionLabel.UseMnemonic = false;
            this.VersionLabel.UseWaitCursor = true;
            // 
            // TitleLabel
            // 
            this.TitleLabel.AccessibleDescription = "Application title (Rubberduck)";
            this.TitleLabel.AccessibleName = "TitleLabel";
            this.TitleLabel.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
            this.TitleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TitleLabel.BackColor = System.Drawing.Color.White;
            this.TitleLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TitleLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TitleLabel.Font = new System.Drawing.Font("Segoe UI Semilight", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleLabel.Location = new System.Drawing.Point(-10, 296);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(340, 36);
            this.TitleLabel.TabIndex = 1;
            this.TitleLabel.Text = "Rubberduck";
            this.TitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.TitleLabel.UseWaitCursor = true;
            // 
            // StatusLabel
            // 
            this.StatusLabel.AccessibleDescription = "Current initialization status";
            this.StatusLabel.AccessibleName = "StatusLabel";
            this.StatusLabel.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.StatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StatusLabel.AutoEllipsis = true;
            this.StatusLabel.BackColor = System.Drawing.Color.Transparent;
            this.StatusLabel.Font = new System.Drawing.Font("Segoe UI Light", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusLabel.Location = new System.Drawing.Point(12, 332);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(295, 19);
            this.StatusLabel.TabIndex = 3;
            this.StatusLabel.Text = "initializing...";
            this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.StatusLabel.UseWaitCursor = true;
            // 
            // ContainerBox
            // 
            this.ContainerBox.AccessibleDescription = "Splash artwork";
            this.ContainerBox.AccessibleName = "ContainerBox";
            this.ContainerBox.AccessibleRole = System.Windows.Forms.AccessibleRole.Graphic;
            this.ContainerBox.Controls.Add(this.StatusLabel);
            this.ContainerBox.Controls.Add(this.TitleLabel);
            this.ContainerBox.Controls.Add(this.VersionLabel);
            this.ContainerBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContainerBox.Image = ((System.Drawing.Image)(resources.GetObject("ContainerBox.Image")));
            this.ContainerBox.InitialImage = null;
            this.ContainerBox.Location = new System.Drawing.Point(0, 0);
            this.ContainerBox.Name = "ContainerBox";
            this.ContainerBox.Size = new System.Drawing.Size(320, 360);
            this.ContainerBox.TabIndex = 0;
            this.ContainerBox.TabStop = false;
            this.ContainerBox.UseWaitCursor = true;
            // 
            // Splash
            // 
            this.AccessibleDescription = "Splash/startup screen";
            this.AccessibleName = "Splash";
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(320, 360);
            this.ControlBox = false;
            this.Controls.Add(this.ContainerBox);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Splash";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rubberduck";
            this.TopMost = true;
            this.UseWaitCursor = true;
            ((System.ComponentModel.ISupportInitialize)(this.ContainerBox)).EndInit();
            this.ContainerBox.ResumeLayout(false);
            this.ContainerBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.PictureBox ContainerBox;
    }
}