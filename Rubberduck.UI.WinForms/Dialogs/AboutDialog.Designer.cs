namespace Rubberduck.UI.WinForms.Dialogs
{
    partial class AboutDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
            this.AboutControlHost = new System.Windows.Forms.Integration.ElementHost();
            this.AboutControl1 = new Rubberduck.UI.Xaml.Controls.AboutControl();
            this.SuspendLayout();
            // 
            // AboutControlHost
            // 
            this.AboutControlHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AboutControlHost.Location = new System.Drawing.Point(0, 0);
            this.AboutControlHost.Name = "AboutControlHost";
            this.AboutControlHost.Size = new System.Drawing.Size(790, 598);
            this.AboutControlHost.TabIndex = 0;
            this.AboutControlHost.Child = this.AboutControl1;
            // 
            // AboutDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 598);
            this.Controls.Add(this.AboutControlHost);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About Rubberduck";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost AboutControlHost;
        private Xaml.Controls.AboutControl AboutControl1;
    }
}