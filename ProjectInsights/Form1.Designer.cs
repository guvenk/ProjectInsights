using System.ComponentModel;
using System.Windows.Forms;

namespace ProjectInsights
{
    partial class FormInsights
    {
        IContainer components = null;
        Button btnShow;
        Label label1;
        Label lblSimilarity;
        TextBox txtContent;
        Label label2;
        TextBox txtProjectPath;
        Label lblError;
        TextBox txtSimilarity;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        void InitializeComponent()
        {
            AddAllControls();
            SetupBtnShow();
            SetupLabel1();
            SetupLblSimilarity();
            SetupTxtContent();
            SetupLabel2();
            SetupTxtProjectPath();
            SetupLblError();
            SetupTxtSimilarity();
            SetupFormInsights();
        }

        void AddAllControls()
        {
            AddButtons();
            AddLabels();
            AddTextBoxes();
            SuspendLayout();
        }

        void AddButtons()
        {
            btnShow = new System.Windows.Forms.Button();
        }

        void AddTextBoxes()
        {
            txtContent = new System.Windows.Forms.TextBox();
            txtProjectPath = new System.Windows.Forms.TextBox();
            txtSimilarity = new System.Windows.Forms.TextBox();
        }

        void AddLabels()
        {
            label1 = new System.Windows.Forms.Label();
            lblSimilarity = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            lblError = new System.Windows.Forms.Label();
        }

        void SetupFormInsights()
        {
            // 
            // FormInsights
            // 
            SetupFormProperties();
            AddControlsToForm();
            ResumeLayout(false);
            PerformLayout();
        }

        void AddControlsToForm()
        {
            AddTextBoxesToForm();
            AddLabelsToForm();
            AddButtonsToForm();
        }

        void AddButtonsToForm()
        {
            Controls.Add(btnShow);
        }

        void AddTextBoxesToForm()
        {
            Controls.Add(txtSimilarity);
            Controls.Add(txtProjectPath);
            Controls.Add(txtContent);
        }

        void AddLabelsToForm()
        {
            Controls.Add(lblError);
            Controls.Add(label2);
            Controls.Add(lblSimilarity);
            Controls.Add(label1);
        }

        void SetupFormProperties()
        {
            Name = "FormInsights";
            Text = "Project Insights";
            PositionForm();
            Load += new System.EventHandler(FormInsights_Load);
        }

        void PositionForm()
        {
            ClientSize = new System.Drawing.Size(831, 472);
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        }

        void SetupTxtSimilarity()
        {
            // 
            // txtSimilarity
            // 
            txtSimilarity.Location = new System.Drawing.Point(13, 78);
            txtSimilarity.Name = "txtSimilarity";
            txtSimilarity.Size = new System.Drawing.Size(129, 25);
            txtSimilarity.TabIndex = 8;
            txtSimilarity.Text = "70";
            txtSimilarity.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txtSimilarity_KeyPress);
        }

        void SetupLblError()
        {
            // 
            // lblError
            // 
            lblError.AutoSize = true;
            lblError.Location = new System.Drawing.Point(610, 12);
            lblError.Name = "lblError";
            lblError.Size = new System.Drawing.Size(0, 17);
            lblError.TabIndex = 7;
        }

        void SetupTxtProjectPath()
        {
            // 
            // txtProjectPath
            // 
            txtProjectPath.Location = new System.Drawing.Point(262, 9);
            txtProjectPath.Name = "txtProjectPath";
            txtProjectPath.Size = new System.Drawing.Size(342, 25);
            txtProjectPath.TabIndex = 6;
        }

        void SetupLabel2()
        {
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(148, 12);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(108, 17);
            label2.TabIndex = 5;
            label2.Text = "Project path (.git)";
        }

        void SetupTxtContent()
        {
            // 
            // txtContent
            // 
            PositionTxtContent();
            txtContent.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            txtContent.Multiline = true;
            txtContent.Name = "txtContent";
            txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            txtContent.TabIndex = 4;
        }

        void PositionTxtContent()
        {
            txtContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            txtContent.Location = new System.Drawing.Point(148, 77);
            txtContent.Size = new System.Drawing.Size(675, 382);
        }

        void SetupLblSimilarity()
        {
            // 
            // lblSimilarity
            // 
            lblSimilarity.AutoSize = true;
            lblSimilarity.Location = new System.Drawing.Point(12, 57);
            lblSimilarity.Name = "lblSimilarity";
            lblSimilarity.Size = new System.Drawing.Size(83, 17);
            lblSimilarity.TabIndex = 2;
            lblSimilarity.Text = "Similarity (%)";
        }

        void SetupLabel1()
        {
            // 
            // label1
            // 
            PositionLabel1();
            label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label1.Name = "label1";
            label1.TabIndex = 1;
            label1.Text = "Project Insights";
        }

        void PositionLabel1()
        {
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 9);
            label1.Size = new System.Drawing.Size(116, 21);
        }

        void SetupBtnShow()
        {
            // 
            // btnShow
            // 
            PositionBtnShow();
            btnShow.Name = "btnShow";
            btnShow.TabIndex = 0;
            btnShow.Text = "Show";
            btnShow.UseVisualStyleBackColor = true;
            btnShow.Click += new System.EventHandler(btnShow_Click);
        }

        void PositionBtnShow()
        {
            btnShow.Location = new System.Drawing.Point(12, 117);
            btnShow.Size = new System.Drawing.Size(130, 35);
        }
    }
}
