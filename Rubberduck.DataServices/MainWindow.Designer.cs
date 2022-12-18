namespace Rubberduck.DataServices
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.StatusLabel = new System.Windows.Forms.Label();
            this.SessionStartLabel = new System.Windows.Forms.Label();
            this.ProcessMemoryLabel = new System.Windows.Forms.Label();
            this.ContentBox = new System.Windows.Forms.GroupBox();
            this.UserDefinedLocalsLabel = new System.Windows.Forms.Label();
            this.UserDefinedIdentifierRefsLabel = new System.Windows.Forms.Label();
            this.UserDefinedDeclarationsLabel = new System.Windows.Forms.Label();
            this.UserDefinedParametersLabel = new System.Windows.Forms.Label();
            this.UserDefinedMembersLabel = new System.Windows.Forms.Label();
            this.UserDefinedModulesLabel = new System.Windows.Forms.Label();
            this.LibraryLocalsLabel = new System.Windows.Forms.Label();
            this.LibraryIdentifierRefsLabel = new System.Windows.Forms.Label();
            this.LibraryDeclarationsLabel = new System.Windows.Forms.Label();
            this.LibraryParametersLabel = new System.Windows.Forms.Label();
            this.LibraryMembersLabel = new System.Windows.Forms.Label();
            this.UserDefinedProjectsLabel = new System.Windows.Forms.Label();
            this.LibraryModulesLabel = new System.Windows.Forms.Label();
            this.LibraryProjectsLabel = new System.Windows.Forms.Label();
            this.LibrariesLabel = new System.Windows.Forms.Label();
            this.UserDefinedLabel = new System.Windows.Forms.Label();
            this.IdentifierReferencesLabel = new System.Windows.Forms.Label();
            this.DeclarationsLabel = new System.Windows.Forms.Label();
            this.LocalsLabel = new System.Windows.Forms.Label();
            this.ParametersLabel = new System.Windows.Forms.Label();
            this.MembersLabel = new System.Windows.Forms.Label();
            this.ModulesLabel = new System.Windows.Forms.Label();
            this.ProjectsLabel = new System.Windows.Forms.Label();
            this.StatusStringLabel = new System.Windows.Forms.Label();
            this.SessionStartStringLabel = new System.Windows.Forms.Label();
            this.ProcessMemoryStringLabel = new System.Windows.Forms.Label();
            this.NotificationIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.ContentBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Location = new System.Drawing.Point(12, 9);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(47, 16);
            this.StatusLabel.TabIndex = 0;
            this.StatusLabel.Text = "Status:";
            // 
            // SessionStartLabel
            // 
            this.SessionStartLabel.AutoSize = true;
            this.SessionStartLabel.Location = new System.Drawing.Point(12, 29);
            this.SessionStartLabel.Name = "SessionStartLabel";
            this.SessionStartLabel.Size = new System.Drawing.Size(89, 16);
            this.SessionStartLabel.TabIndex = 0;
            this.SessionStartLabel.Text = "Session Start:";
            // 
            // ProcessMemoryLabel
            // 
            this.ProcessMemoryLabel.AutoSize = true;
            this.ProcessMemoryLabel.Location = new System.Drawing.Point(12, 49);
            this.ProcessMemoryLabel.Name = "ProcessMemoryLabel";
            this.ProcessMemoryLabel.Size = new System.Drawing.Size(112, 16);
            this.ProcessMemoryLabel.TabIndex = 0;
            this.ProcessMemoryLabel.Text = "Process Memory:";
            // 
            // ContentBox
            // 
            this.ContentBox.Controls.Add(this.UserDefinedLocalsLabel);
            this.ContentBox.Controls.Add(this.UserDefinedIdentifierRefsLabel);
            this.ContentBox.Controls.Add(this.UserDefinedDeclarationsLabel);
            this.ContentBox.Controls.Add(this.UserDefinedParametersLabel);
            this.ContentBox.Controls.Add(this.UserDefinedMembersLabel);
            this.ContentBox.Controls.Add(this.UserDefinedModulesLabel);
            this.ContentBox.Controls.Add(this.LibraryLocalsLabel);
            this.ContentBox.Controls.Add(this.LibraryIdentifierRefsLabel);
            this.ContentBox.Controls.Add(this.LibraryDeclarationsLabel);
            this.ContentBox.Controls.Add(this.LibraryParametersLabel);
            this.ContentBox.Controls.Add(this.LibraryMembersLabel);
            this.ContentBox.Controls.Add(this.UserDefinedProjectsLabel);
            this.ContentBox.Controls.Add(this.LibraryModulesLabel);
            this.ContentBox.Controls.Add(this.LibraryProjectsLabel);
            this.ContentBox.Controls.Add(this.LibrariesLabel);
            this.ContentBox.Controls.Add(this.UserDefinedLabel);
            this.ContentBox.Controls.Add(this.IdentifierReferencesLabel);
            this.ContentBox.Controls.Add(this.DeclarationsLabel);
            this.ContentBox.Controls.Add(this.LocalsLabel);
            this.ContentBox.Controls.Add(this.ParametersLabel);
            this.ContentBox.Controls.Add(this.MembersLabel);
            this.ContentBox.Controls.Add(this.ModulesLabel);
            this.ContentBox.Controls.Add(this.ProjectsLabel);
            this.ContentBox.Location = new System.Drawing.Point(12, 86);
            this.ContentBox.Name = "ContentBox";
            this.ContentBox.Size = new System.Drawing.Size(260, 205);
            this.ContentBox.TabIndex = 1;
            this.ContentBox.TabStop = false;
            this.ContentBox.Text = "Content";
            // 
            // UserDefinedLocalsLabel
            // 
            this.UserDefinedLocalsLabel.Location = new System.Drawing.Point(185, 103);
            this.UserDefinedLocalsLabel.Name = "UserDefinedLocalsLabel";
            this.UserDefinedLocalsLabel.Size = new System.Drawing.Size(52, 16);
            this.UserDefinedLocalsLabel.TabIndex = 2;
            this.UserDefinedLocalsLabel.Text = "0";
            this.UserDefinedLocalsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // UserDefinedIdentifierRefsLabel
            // 
            this.UserDefinedIdentifierRefsLabel.Location = new System.Drawing.Point(185, 135);
            this.UserDefinedIdentifierRefsLabel.Name = "UserDefinedIdentifierRefsLabel";
            this.UserDefinedIdentifierRefsLabel.Size = new System.Drawing.Size(52, 16);
            this.UserDefinedIdentifierRefsLabel.TabIndex = 2;
            this.UserDefinedIdentifierRefsLabel.Text = "0";
            this.UserDefinedIdentifierRefsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // UserDefinedDeclarationsLabel
            // 
            this.UserDefinedDeclarationsLabel.Location = new System.Drawing.Point(185, 119);
            this.UserDefinedDeclarationsLabel.Name = "UserDefinedDeclarationsLabel";
            this.UserDefinedDeclarationsLabel.Size = new System.Drawing.Size(52, 16);
            this.UserDefinedDeclarationsLabel.TabIndex = 2;
            this.UserDefinedDeclarationsLabel.Text = "0";
            this.UserDefinedDeclarationsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // UserDefinedParametersLabel
            // 
            this.UserDefinedParametersLabel.Location = new System.Drawing.Point(185, 87);
            this.UserDefinedParametersLabel.Name = "UserDefinedParametersLabel";
            this.UserDefinedParametersLabel.Size = new System.Drawing.Size(52, 16);
            this.UserDefinedParametersLabel.TabIndex = 2;
            this.UserDefinedParametersLabel.Text = "0";
            this.UserDefinedParametersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // UserDefinedMembersLabel
            // 
            this.UserDefinedMembersLabel.Location = new System.Drawing.Point(185, 71);
            this.UserDefinedMembersLabel.Name = "UserDefinedMembersLabel";
            this.UserDefinedMembersLabel.Size = new System.Drawing.Size(52, 16);
            this.UserDefinedMembersLabel.TabIndex = 2;
            this.UserDefinedMembersLabel.Text = "0";
            this.UserDefinedMembersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // UserDefinedModulesLabel
            // 
            this.UserDefinedModulesLabel.Location = new System.Drawing.Point(185, 55);
            this.UserDefinedModulesLabel.Name = "UserDefinedModulesLabel";
            this.UserDefinedModulesLabel.Size = new System.Drawing.Size(52, 16);
            this.UserDefinedModulesLabel.TabIndex = 2;
            this.UserDefinedModulesLabel.Text = "0";
            this.UserDefinedModulesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LibraryLocalsLabel
            // 
            this.LibraryLocalsLabel.Location = new System.Drawing.Point(83, 103);
            this.LibraryLocalsLabel.Name = "LibraryLocalsLabel";
            this.LibraryLocalsLabel.Size = new System.Drawing.Size(52, 16);
            this.LibraryLocalsLabel.TabIndex = 2;
            this.LibraryLocalsLabel.Text = "-";
            this.LibraryLocalsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LibraryIdentifierRefsLabel
            // 
            this.LibraryIdentifierRefsLabel.Location = new System.Drawing.Point(83, 135);
            this.LibraryIdentifierRefsLabel.Name = "LibraryIdentifierRefsLabel";
            this.LibraryIdentifierRefsLabel.Size = new System.Drawing.Size(52, 16);
            this.LibraryIdentifierRefsLabel.TabIndex = 2;
            this.LibraryIdentifierRefsLabel.Text = "0";
            this.LibraryIdentifierRefsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LibraryDeclarationsLabel
            // 
            this.LibraryDeclarationsLabel.Location = new System.Drawing.Point(83, 119);
            this.LibraryDeclarationsLabel.Name = "LibraryDeclarationsLabel";
            this.LibraryDeclarationsLabel.Size = new System.Drawing.Size(52, 16);
            this.LibraryDeclarationsLabel.TabIndex = 2;
            this.LibraryDeclarationsLabel.Text = "0";
            this.LibraryDeclarationsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LibraryParametersLabel
            // 
            this.LibraryParametersLabel.Location = new System.Drawing.Point(83, 87);
            this.LibraryParametersLabel.Name = "LibraryParametersLabel";
            this.LibraryParametersLabel.Size = new System.Drawing.Size(52, 16);
            this.LibraryParametersLabel.TabIndex = 2;
            this.LibraryParametersLabel.Text = "0";
            this.LibraryParametersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LibraryMembersLabel
            // 
            this.LibraryMembersLabel.Location = new System.Drawing.Point(83, 71);
            this.LibraryMembersLabel.Name = "LibraryMembersLabel";
            this.LibraryMembersLabel.Size = new System.Drawing.Size(52, 16);
            this.LibraryMembersLabel.TabIndex = 2;
            this.LibraryMembersLabel.Text = "0";
            this.LibraryMembersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // UserDefinedProjectsLabel
            // 
            this.UserDefinedProjectsLabel.Location = new System.Drawing.Point(185, 39);
            this.UserDefinedProjectsLabel.Name = "UserDefinedProjectsLabel";
            this.UserDefinedProjectsLabel.Size = new System.Drawing.Size(52, 16);
            this.UserDefinedProjectsLabel.TabIndex = 2;
            this.UserDefinedProjectsLabel.Text = "0";
            this.UserDefinedProjectsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LibraryModulesLabel
            // 
            this.LibraryModulesLabel.Location = new System.Drawing.Point(83, 55);
            this.LibraryModulesLabel.Name = "LibraryModulesLabel";
            this.LibraryModulesLabel.Size = new System.Drawing.Size(52, 16);
            this.LibraryModulesLabel.TabIndex = 2;
            this.LibraryModulesLabel.Text = "0";
            this.LibraryModulesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LibraryProjectsLabel
            // 
            this.LibraryProjectsLabel.Location = new System.Drawing.Point(83, 39);
            this.LibraryProjectsLabel.Name = "LibraryProjectsLabel";
            this.LibraryProjectsLabel.Size = new System.Drawing.Size(52, 16);
            this.LibraryProjectsLabel.TabIndex = 2;
            this.LibraryProjectsLabel.Text = "0";
            this.LibraryProjectsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LibrariesLabel
            // 
            this.LibrariesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LibrariesLabel.AutoSize = true;
            this.LibrariesLabel.Location = new System.Drawing.Point(80, 18);
            this.LibrariesLabel.Name = "LibrariesLabel";
            this.LibrariesLabel.Size = new System.Drawing.Size(55, 16);
            this.LibrariesLabel.TabIndex = 1;
            this.LibrariesLabel.Text = "libraries";
            this.LibrariesLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // UserDefinedLabel
            // 
            this.UserDefinedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UserDefinedLabel.AutoSize = true;
            this.UserDefinedLabel.Location = new System.Drawing.Point(155, 18);
            this.UserDefinedLabel.Name = "UserDefinedLabel";
            this.UserDefinedLabel.Size = new System.Drawing.Size(82, 16);
            this.UserDefinedLabel.TabIndex = 1;
            this.UserDefinedLabel.Text = "user-defined";
            this.UserDefinedLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // IdentifierReferencesLabel
            // 
            this.IdentifierReferencesLabel.Location = new System.Drawing.Point(6, 135);
            this.IdentifierReferencesLabel.Name = "IdentifierReferencesLabel";
            this.IdentifierReferencesLabel.Size = new System.Drawing.Size(84, 38);
            this.IdentifierReferencesLabel.TabIndex = 1;
            this.IdentifierReferencesLabel.Text = "Identifier references:";
            // 
            // DeclarationsLabel
            // 
            this.DeclarationsLabel.AutoSize = true;
            this.DeclarationsLabel.Location = new System.Drawing.Point(6, 119);
            this.DeclarationsLabel.Name = "DeclarationsLabel";
            this.DeclarationsLabel.Size = new System.Drawing.Size(86, 16);
            this.DeclarationsLabel.TabIndex = 1;
            this.DeclarationsLabel.Text = "Declarations:";
            // 
            // LocalsLabel
            // 
            this.LocalsLabel.AutoSize = true;
            this.LocalsLabel.Location = new System.Drawing.Point(6, 103);
            this.LocalsLabel.Name = "LocalsLabel";
            this.LocalsLabel.Size = new System.Drawing.Size(50, 16);
            this.LocalsLabel.TabIndex = 1;
            this.LocalsLabel.Text = "Locals:";
            // 
            // ParametersLabel
            // 
            this.ParametersLabel.AutoSize = true;
            this.ParametersLabel.Location = new System.Drawing.Point(6, 87);
            this.ParametersLabel.Name = "ParametersLabel";
            this.ParametersLabel.Size = new System.Drawing.Size(80, 16);
            this.ParametersLabel.TabIndex = 1;
            this.ParametersLabel.Text = "Parameters:";
            // 
            // MembersLabel
            // 
            this.MembersLabel.AutoSize = true;
            this.MembersLabel.Location = new System.Drawing.Point(6, 71);
            this.MembersLabel.Name = "MembersLabel";
            this.MembersLabel.Size = new System.Drawing.Size(67, 16);
            this.MembersLabel.TabIndex = 1;
            this.MembersLabel.Text = "Members:";
            // 
            // ModulesLabel
            // 
            this.ModulesLabel.AutoSize = true;
            this.ModulesLabel.Location = new System.Drawing.Point(6, 55);
            this.ModulesLabel.Name = "ModulesLabel";
            this.ModulesLabel.Size = new System.Drawing.Size(62, 16);
            this.ModulesLabel.TabIndex = 1;
            this.ModulesLabel.Text = "Modules:";
            // 
            // ProjectsLabel
            // 
            this.ProjectsLabel.AutoSize = true;
            this.ProjectsLabel.Location = new System.Drawing.Point(6, 39);
            this.ProjectsLabel.Name = "ProjectsLabel";
            this.ProjectsLabel.Size = new System.Drawing.Size(59, 16);
            this.ProjectsLabel.TabIndex = 1;
            this.ProjectsLabel.Text = "Projects:";
            // 
            // StatusStringLabel
            // 
            this.StatusStringLabel.AutoSize = true;
            this.StatusStringLabel.Location = new System.Drawing.Point(129, 9);
            this.StatusStringLabel.Name = "StatusStringLabel";
            this.StatusStringLabel.Size = new System.Drawing.Size(45, 16);
            this.StatusStringLabel.TabIndex = 0;
            this.StatusStringLabel.Text = "Online";
            // 
            // SessionStartStringLabel
            // 
            this.SessionStartStringLabel.AutoSize = true;
            this.SessionStartStringLabel.Location = new System.Drawing.Point(129, 29);
            this.SessionStartStringLabel.Name = "SessionStartStringLabel";
            this.SessionStartStringLabel.Size = new System.Drawing.Size(122, 16);
            this.SessionStartStringLabel.TabIndex = 0;
            this.SessionStartStringLabel.Text = "0000-00-00 00:00:00";
            // 
            // ProcessMemoryStringLabel
            // 
            this.ProcessMemoryStringLabel.AutoSize = true;
            this.ProcessMemoryStringLabel.Location = new System.Drawing.Point(129, 49);
            this.ProcessMemoryStringLabel.Name = "ProcessMemoryStringLabel";
            this.ProcessMemoryStringLabel.Size = new System.Drawing.Size(37, 16);
            this.ProcessMemoryStringLabel.TabIndex = 0;
            this.ProcessMemoryStringLabel.Text = "0 MB";
            // 
            // NotificationIcon
            // 
            this.NotificationIcon.BalloonTipTitle = "Rubberduck.DataServices";
            this.NotificationIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("NotificationIcon.Icon")));
            this.NotificationIcon.Text = "Rubberduck.DataServices";
            this.NotificationIcon.Visible = true;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 299);
            this.Controls.Add(this.ContentBox);
            this.Controls.Add(this.ProcessMemoryStringLabel);
            this.Controls.Add(this.ProcessMemoryLabel);
            this.Controls.Add(this.SessionStartStringLabel);
            this.Controls.Add(this.SessionStartLabel);
            this.Controls.Add(this.StatusStringLabel);
            this.Controls.Add(this.StatusLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Rubberduck.DataServices";
            this.ContentBox.ResumeLayout(false);
            this.ContentBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Label SessionStartLabel;
        private System.Windows.Forms.Label ProcessMemoryLabel;
        private System.Windows.Forms.GroupBox ContentBox;
        private System.Windows.Forms.Label UserDefinedLocalsLabel;
        private System.Windows.Forms.Label UserDefinedIdentifierRefsLabel;
        private System.Windows.Forms.Label UserDefinedDeclarationsLabel;
        private System.Windows.Forms.Label UserDefinedParametersLabel;
        private System.Windows.Forms.Label UserDefinedMembersLabel;
        private System.Windows.Forms.Label UserDefinedModulesLabel;
        private System.Windows.Forms.Label LibraryLocalsLabel;
        private System.Windows.Forms.Label LibraryIdentifierRefsLabel;
        private System.Windows.Forms.Label LibraryDeclarationsLabel;
        private System.Windows.Forms.Label LibraryParametersLabel;
        private System.Windows.Forms.Label LibraryMembersLabel;
        private System.Windows.Forms.Label UserDefinedProjectsLabel;
        private System.Windows.Forms.Label LibraryModulesLabel;
        private System.Windows.Forms.Label LibraryProjectsLabel;
        private System.Windows.Forms.Label LibrariesLabel;
        private System.Windows.Forms.Label UserDefinedLabel;
        private System.Windows.Forms.Label IdentifierReferencesLabel;
        private System.Windows.Forms.Label DeclarationsLabel;
        private System.Windows.Forms.Label LocalsLabel;
        private System.Windows.Forms.Label ParametersLabel;
        private System.Windows.Forms.Label MembersLabel;
        private System.Windows.Forms.Label ModulesLabel;
        private System.Windows.Forms.Label ProjectsLabel;
        private System.Windows.Forms.Label StatusStringLabel;
        private System.Windows.Forms.Label SessionStartStringLabel;
        private System.Windows.Forms.Label ProcessMemoryStringLabel;
        private System.Windows.Forms.NotifyIcon NotificationIcon;
    }
}

