﻿namespace RevitToRDFConverter
{
    partial class GraphDBURLForm
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
            this.urlBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // urlBox
            // 
            this.urlBox.Location = new System.Drawing.Point(12, 32);
            this.urlBox.Name = "urlBox";
            this.urlBox.Size = new System.Drawing.Size(188, 20);
            this.urlBox.TabIndex = 0;
            this.urlBox.TextChanged += new System.EventHandler(this.urlBox_TextChanged);
            this.urlBox.Enter += new System.EventHandler(this.urlBox_Enter);
            this.urlBox.Leave += new System.EventHandler(this.urlBox_Leave);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(125, 68);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Enter URL for GraphDB";
            // 
            // GraphDBURLForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(212, 103);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.urlBox);
            this.Name = "GraphDBURLForm";
            this.Text = "GraphDBURLForm";
            this.Load += new System.EventHandler(this.GraphDBURLForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox urlBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label1;
    }
}