namespace jwpubtoc
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.LanguageComboBox = new System.Windows.Forms.ComboBox();
            this.PublicationListView = new System.Windows.Forms.ListView();
            this.RefreshButton = new System.Windows.Forms.Button();
            this.DownloadButton = new System.Windows.Forms.Button();
            this.TocButton = new System.Windows.Forms.Button();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.TocBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.DownloadBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.StatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // LanguageComboBox
            // 
            this.LanguageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LanguageComboBox.FormattingEnabled = true;
            this.LanguageComboBox.Location = new System.Drawing.Point(15, 15);
            this.LanguageComboBox.Name = "LanguageComboBox";
            this.LanguageComboBox.Size = new System.Drawing.Size(560, 20);
            this.LanguageComboBox.TabIndex = 0;
            this.LanguageComboBox.SelectedIndexChanged += new System.EventHandler(this.LanguageComboBox_SelectedIndexChanged);
            // 
            // PublicationListView
            // 
            this.PublicationListView.FullRowSelect = true;
            this.PublicationListView.GridLines = true;
            this.PublicationListView.HideSelection = false;
            this.PublicationListView.Location = new System.Drawing.Point(15, 50);
            this.PublicationListView.Name = "PublicationListView";
            this.PublicationListView.Size = new System.Drawing.Size(560, 250);
            this.PublicationListView.TabIndex = 1;
            this.PublicationListView.UseCompatibleStateImageBehavior = false;
            this.PublicationListView.View = System.Windows.Forms.View.Details;
            this.PublicationListView.DoubleClick += new System.EventHandler(this.PublicationListView_DoubleClick);
            // 
            // RefreshButton
            // 
            this.RefreshButton.Location = new System.Drawing.Point(100, 315);
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(100, 23);
            this.RefreshButton.TabIndex = 2;
            this.RefreshButton.Text = "Refresh";
            this.RefreshButton.UseVisualStyleBackColor = true;
            this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // DownloadButton
            // 
            this.DownloadButton.Location = new System.Drawing.Point(250, 315);
            this.DownloadButton.Name = "DownloadButton";
            this.DownloadButton.Size = new System.Drawing.Size(100, 23);
            this.DownloadButton.TabIndex = 3;
            this.DownloadButton.Text = "Download PDF";
            this.DownloadButton.UseVisualStyleBackColor = true;
            this.DownloadButton.Click += new System.EventHandler(this.DownloadButton_Click);
            // 
            // TocButton
            // 
            this.TocButton.Location = new System.Drawing.Point(400, 315);
            this.TocButton.Name = "TocButton";
            this.TocButton.Size = new System.Drawing.Size(100, 23);
            this.TocButton.TabIndex = 4;
            this.TocButton.Text = "Convert PDF";
            this.TocButton.UseVisualStyleBackColor = true;
            this.TocButton.Click += new System.EventHandler(this.TocButton_Click);
            // 
            // StatusStrip
            // 
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.StatusStrip.Location = new System.Drawing.Point(0, 353);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(594, 22);
            this.StatusStrip.TabIndex = 5;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // TocBackgroundWorker
            // 
            this.TocBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.TocBackgroundWorker_DoWork);
            this.TocBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.TocBackgroundWorker_RunWorkerCompleted);
            this.TocBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.TocBackgroundWorker_ProgressChanged);
            // 
            // DownloadBackgroundWorker
            // 
            this.DownloadBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DownloadBackgroundWorker_DoWork);
            this.DownloadBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.DownloadBackgroundWorker_RunWorkerCompleted);
            this.DownloadBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.DownloadBackgroundWorker_ProgressChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 375);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.TocButton);
            this.Controls.Add(this.DownloadButton);
            this.Controls.Add(this.RefreshButton);
            this.Controls.Add(this.PublicationListView);
            this.Controls.Add(this.LanguageComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "JW PDF Publication TOC Maker";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox LanguageComboBox;
        private System.Windows.Forms.ListView PublicationListView;
        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.Button DownloadButton;
        private System.Windows.Forms.Button TocButton;
        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.ComponentModel.BackgroundWorker TocBackgroundWorker;
        private System.ComponentModel.BackgroundWorker DownloadBackgroundWorker;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}

