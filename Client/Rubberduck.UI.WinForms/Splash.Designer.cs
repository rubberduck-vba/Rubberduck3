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
            VersionLabel = new System.Windows.Forms.Label();
            TitleLabel = new System.Windows.Forms.Label();
            StatusLabel = new System.Windows.Forms.Label();
            ContainerBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)ContainerBox).BeginInit();
            ContainerBox.SuspendLayout();
            SuspendLayout();
            // 
            // VersionLabel
            // 
            VersionLabel.AccessibleDescription = "Rubberduck version/build number";
            VersionLabel.AccessibleName = "VersionLabel";
            VersionLabel.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            VersionLabel.AutoSize = true;
            VersionLabel.BackColor = System.Drawing.Color.Transparent;
            VersionLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            VersionLabel.Location = new System.Drawing.Point(13, 9);
            VersionLabel.Name = "VersionLabel";
            VersionLabel.Size = new System.Drawing.Size(114, 21);
            VersionLabel.TabIndex = 2;
            VersionLabel.Text = "v3.0.x (debug)";
            VersionLabel.UseMnemonic = false;
            VersionLabel.UseWaitCursor = true;
            // 
            // TitleLabel
            // 
            TitleLabel.AccessibleDescription = "Application title (Rubberduck)";
            TitleLabel.AccessibleName = "TitleLabel";
            TitleLabel.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
            TitleLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TitleLabel.BackColor = System.Drawing.Color.White;
            TitleLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TitleLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            TitleLabel.Font = new System.Drawing.Font("Segoe UI Semilight", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleLabel.Location = new System.Drawing.Point(-10, 296);
            TitleLabel.Name = "TitleLabel";
            TitleLabel.Size = new System.Drawing.Size(340, 36);
            TitleLabel.TabIndex = 1;
            TitleLabel.Text = "Rubberduck";
            TitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            TitleLabel.UseWaitCursor = true;
            // 
            // StatusLabel
            // 
            StatusLabel.AccessibleDescription = "Current initialization status";
            StatusLabel.AccessibleName = "StatusLabel";
            StatusLabel.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            StatusLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            StatusLabel.AutoEllipsis = true;
            StatusLabel.BackColor = System.Drawing.Color.Transparent;
            StatusLabel.Font = new System.Drawing.Font("Segoe UI Light", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            StatusLabel.Location = new System.Drawing.Point(12, 332);
            StatusLabel.Name = "StatusLabel";
            StatusLabel.Size = new System.Drawing.Size(295, 19);
            StatusLabel.TabIndex = 3;
            StatusLabel.Text = "initializing...";
            StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            StatusLabel.UseWaitCursor = true;
            // 
            // ContainerBox
            // 
            ContainerBox.AccessibleDescription = "Splash artwork";
            ContainerBox.AccessibleName = "ContainerBox";
            ContainerBox.AccessibleRole = System.Windows.Forms.AccessibleRole.Graphic;
            ContainerBox.Controls.Add(StatusLabel);
            ContainerBox.Controls.Add(TitleLabel);
            ContainerBox.Controls.Add(VersionLabel);
            ContainerBox.Dock = System.Windows.Forms.DockStyle.Fill;
            ContainerBox.Image = (System.Drawing.Image)resources.GetObject("ContainerBox.Image");
            ContainerBox.InitialImage = null;
            ContainerBox.Location = new System.Drawing.Point(0, 0);
            ContainerBox.Name = "ContainerBox";
            ContainerBox.Size = new System.Drawing.Size(320, 360);
            ContainerBox.TabIndex = 0;
            ContainerBox.TabStop = false;
            ContainerBox.UseWaitCursor = true;
            // 
            // Splash
            // 
            AccessibleDescription = "Splash/startup screen";
            AccessibleName = "Splash";
            AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            BackColor = System.Drawing.SystemColors.Control;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            CausesValidation = false;
            ClientSize = new System.Drawing.Size(320, 360);
            ControlBox = false;
            Controls.Add(ContainerBox);
            Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Splash";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Rubberduck";
            TopMost = true;
            UseWaitCursor = true;
            ((System.ComponentModel.ISupportInitialize)ContainerBox).EndInit();
            ContainerBox.ResumeLayout(false);
            ContainerBox.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.PictureBox ContainerBox;
    }
}