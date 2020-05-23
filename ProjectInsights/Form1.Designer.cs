namespace ProjectInsights
{
    partial class FormInsights
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnShow = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSimilarity = new System.Windows.Forms.Label();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtProjectPath = new System.Windows.Forms.TextBox();
            this.lblError = new System.Windows.Forms.Label();
            this.txtSimilarity = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnShow
            // 
            this.btnShow.Location = new System.Drawing.Point(12, 117);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(130, 35);
            this.btnShow.TabIndex = 0;
            this.btnShow.Text = "Show";
            this.btnShow.UseVisualStyleBackColor = true;
            this.btnShow.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "Project Insights";
            // 
            // lblSimilarity
            // 
            this.lblSimilarity.AutoSize = true;
            this.lblSimilarity.Location = new System.Drawing.Point(12, 57);
            this.lblSimilarity.Name = "lblSimilarity";
            this.lblSimilarity.Size = new System.Drawing.Size(83, 17);
            this.lblSimilarity.TabIndex = 2;
            this.lblSimilarity.Text = "Similarity (%)";
            // 
            // txtContent
            // 
            this.txtContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtContent.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtContent.Location = new System.Drawing.Point(148, 77);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtContent.Size = new System.Drawing.Size(456, 382);
            this.txtContent.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(148, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Project path (.git)";
            // 
            // txtProjectPath
            // 
            this.txtProjectPath.Location = new System.Drawing.Point(262, 9);
            this.txtProjectPath.Name = "txtProjectPath";
            this.txtProjectPath.Size = new System.Drawing.Size(342, 25);
            this.txtProjectPath.TabIndex = 6;
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(610, 12);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(0, 17);
            this.lblError.TabIndex = 7;
            // 
            // txtSimilarity
            // 
            this.txtSimilarity.Location = new System.Drawing.Point(13, 78);
            this.txtSimilarity.Name = "txtSimilarity";
            this.txtSimilarity.Size = new System.Drawing.Size(129, 25);
            this.txtSimilarity.TabIndex = 8;
            this.txtSimilarity.Text = "70";
            this.txtSimilarity.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSimilarity_KeyPress);
            // 
            // FormInsights
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 472);
            this.Controls.Add(this.txtSimilarity);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.txtProjectPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtContent);
            this.Controls.Add(this.lblSimilarity);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnShow);
            this.Name = "FormInsights";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Project Insights";
            this.Load += new System.EventHandler(this.FormInsights_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSimilarity;
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtProjectPath;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.TextBox txtSimilarity;
    }
}
