namespace Rubberduck.UI.WinForms
{
    partial class EditorShellWindow
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.EditorShellHost = new System.Windows.Forms.Integration.ElementHost();
            //this.editorShellControl1 = new Rubberduck.UI.Xaml.Controls.EditorShellControl();
            this.SuspendLayout();
            // 
            // EditorShellHost
            // 
            this.EditorShellHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EditorShellHost.Location = new System.Drawing.Point(0, 0);
            this.EditorShellHost.Name = "EditorShellHost";
            this.EditorShellHost.Size = new System.Drawing.Size(884, 699);
            this.EditorShellHost.TabIndex = 0;
            //this.EditorShellHost.Child = this.editorShellControl1;
            // 
            // EditorShellWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.EditorShellHost);
            this.Name = "EditorShellWindow";
            this.Size = new System.Drawing.Size(884, 699);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost EditorShellHost;
        //private Xaml.Controls.EditorShellControl editorShellControl1;
    }
}
