
namespace BeatSaberSongArchiver
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.txtBsLocation = new System.Windows.Forms.TextBox();
            this.btnLocate = new System.Windows.Forms.Button();
            this.bsFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.btnArchive = new System.Windows.Forms.Button();
            this.audioSettings = new System.Windows.Forms.GroupBox();
            this.audioBitrate = new System.Windows.Forms.ComboBox();
            this.coverSettings = new System.Windows.Forms.GroupBox();
            this.coverSize = new System.Windows.Forms.ComboBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.audioSettings.SuspendLayout();
            this.coverSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Beat Saber Location :";
            // 
            // txtBsLocation
            // 
            this.txtBsLocation.Location = new System.Drawing.Point(12, 25);
            this.txtBsLocation.Name = "txtBsLocation";
            this.txtBsLocation.Size = new System.Drawing.Size(376, 20);
            this.txtBsLocation.TabIndex = 1;
            // 
            // btnLocate
            // 
            this.btnLocate.Location = new System.Drawing.Point(394, 24);
            this.btnLocate.Name = "btnLocate";
            this.btnLocate.Size = new System.Drawing.Size(75, 23);
            this.btnLocate.TabIndex = 2;
            this.btnLocate.Text = "Locate";
            this.btnLocate.UseVisualStyleBackColor = true;
            this.btnLocate.Click += new System.EventHandler(this.btnLocate_Click);
            // 
            // btnArchive
            // 
            this.btnArchive.Location = new System.Drawing.Point(394, 172);
            this.btnArchive.Name = "btnArchive";
            this.btnArchive.Size = new System.Drawing.Size(75, 23);
            this.btnArchive.TabIndex = 13;
            this.btnArchive.Text = "Archive";
            this.btnArchive.UseVisualStyleBackColor = true;
            this.btnArchive.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // audioSettings
            // 
            this.audioSettings.Controls.Add(this.audioBitrate);
            this.audioSettings.Location = new System.Drawing.Point(12, 53);
            this.audioSettings.Name = "audioSettings";
            this.audioSettings.Size = new System.Drawing.Size(457, 51);
            this.audioSettings.TabIndex = 3;
            this.audioSettings.TabStop = false;
            this.audioSettings.Text = "Audio compression";
            // 
            // audioBitrate
            // 
            this.audioBitrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.audioBitrate.FormattingEnabled = true;
            this.audioBitrate.Items.AddRange(new object[] {
            "No compression",
            "64 Kbps",
            "96 Kbps",
            "128 Kbps",
            "160 Kpbs",
            "192 Kbps",
            "256 Kbps",
            "320 Kbps"});
            this.audioBitrate.Location = new System.Drawing.Point(6, 19);
            this.audioBitrate.Name = "audioBitrate";
            this.audioBitrate.Size = new System.Drawing.Size(121, 21);
            this.audioBitrate.TabIndex = 4;
            // 
            // coverSettings
            // 
            this.coverSettings.Controls.Add(this.coverSize);
            this.coverSettings.Location = new System.Drawing.Point(12, 110);
            this.coverSettings.Name = "coverSettings";
            this.coverSettings.Size = new System.Drawing.Size(457, 51);
            this.coverSettings.TabIndex = 5;
            this.coverSettings.TabStop = false;
            this.coverSettings.Text = "Cover compression";
            // 
            // coverSize
            // 
            this.coverSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.coverSize.FormattingEnabled = true;
            this.coverSize.Items.AddRange(new object[] {
            "No compression",
            "1024x1024 px",
            "512x512 px",
            "256x256 px",
            "128x128 px"});
            this.coverSize.Location = new System.Drawing.Point(6, 19);
            this.coverSize.Name = "coverSize";
            this.coverSize.Size = new System.Drawing.Size(121, 21);
            this.coverSize.TabIndex = 5;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 172);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(376, 23);
            this.progressBar.TabIndex = 12;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 208);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.coverSettings);
            this.Controls.Add(this.audioSettings);
            this.Controls.Add(this.btnArchive);
            this.Controls.Add(this.btnLocate);
            this.Controls.Add(this.txtBsLocation);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Beat Saber Song Archiver";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.audioSettings.ResumeLayout(false);
            this.coverSettings.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBsLocation;
        private System.Windows.Forms.Button btnLocate;
        private System.Windows.Forms.FolderBrowserDialog bsFolderDialog;
        private System.Windows.Forms.Button btnArchive;
        private System.Windows.Forms.GroupBox audioSettings;
        private System.Windows.Forms.GroupBox coverSettings;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ComboBox audioBitrate;
        private System.Windows.Forms.ComboBox coverSize;
    }
}

