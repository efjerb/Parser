namespace RevitToRDFConverter
{
    partial class GraphDBForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.repositoryIdDropDown = new System.Windows.Forms.ComboBox();
            this.graphNameTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(231, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please pick the GraphDB repository from the list";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(201, 129);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // repositoryIdDropDown
            // 
            this.repositoryIdDropDown.FormattingEnabled = true;
            this.repositoryIdDropDown.Location = new System.Drawing.Point(15, 35);
            this.repositoryIdDropDown.Margin = new System.Windows.Forms.Padding(2);
            this.repositoryIdDropDown.Name = "repositoryIdDropDown";
            this.repositoryIdDropDown.Size = new System.Drawing.Size(111, 21);
            this.repositoryIdDropDown.TabIndex = 1;
            this.repositoryIdDropDown.SelectedIndexChanged += new System.EventHandler(this.repositoryIdDropDown_SelectedIndexChanged);
            // 
            // graphNameTextBox
            // 
            this.graphNameTextBox.Location = new System.Drawing.Point(16, 78);
            this.graphNameTextBox.Name = "graphNameTextBox";
            this.graphNameTextBox.Size = new System.Drawing.Size(110, 20);
            this.graphNameTextBox.TabIndex = 2;
            this.graphNameTextBox.TextChanged += new System.EventHandler(this.graphNameTextBox_TextChanged);
            this.graphNameTextBox.Enter += new System.EventHandler(this.graphNameTextBox_Enter);
            this.graphNameTextBox.Leave += new System.EventHandler(this.graphNameTextBox_Leave);
            // 
            // GraphDBForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 164);
            this.Controls.Add(this.graphNameTextBox);
            this.Controls.Add(this.repositoryIdDropDown);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label1);
            this.Name = "GraphDBForm";
            this.Text = "GraphDBForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GraphDBForm_FormClosing);
            this.Load += new System.EventHandler(this.GraphDBForms_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox repositoryIdDropDown;
        private System.Windows.Forms.TextBox graphNameTextBox;
    }
}