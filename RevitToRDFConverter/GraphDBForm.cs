using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RevitToRDFConverter
{
    public partial class GraphDBForm : Form
    {
        public List<string> repositoryList;
        public string repositoryId;
        public string graphName;
        public string graphNamePlaceholder;
        public GraphDBForm(List<string> repositoryList)
        {
            this.repositoryList = repositoryList;
            InitializeComponent();
            foreach (string repository in repositoryList)
            {
                repositoryIdDropDown.Items.Add(repository);
            }
            if (repositoryList.Contains("Testing"))
            {
                repositoryIdDropDown.SelectedIndex = repositoryList.FindIndex(a => a.Contains("Testing"));
                
                repositoryIdDropDown.SelectedText = "Testing";
            }
            else
            {
                repositoryIdDropDown.SelectedIndex = 0;
            }
        }

        private void GraphDBForms_Load(object sender, EventArgs e)
        {
            graphNamePlaceholder = "Enter a graph name";
            graphNameTextBox.Text = graphNamePlaceholder;
            graphNameTextBox.ForeColor = Color.LightGray;
        }

        private void okButton_Click(object sender, EventArgs e)
        {


            if (graphNameTextBox.Text == "" || graphNameTextBox.Text == graphNamePlaceholder)
            {
                TaskDialog.Show("No graph name", "Please enter a graph name");
                
            }
            else
            {
                repositoryId = repositoryIdDropDown.Text;

                graphName = graphNameTextBox.Text;

                okButton.DialogResult = DialogResult.OK;

                Close();
                return;
            }
        }

        private void repositoryIdTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void repositoryIdDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void graphNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void graphNameTextBox_Enter(object sender, EventArgs e)
        {
            if (graphNameTextBox.Text == graphNamePlaceholder)
            {
                graphNameTextBox.Text = "";
                graphNameTextBox.ForeColor = Color.Black;
            }
        }

        private void graphNameTextBox_Leave(object sender, EventArgs e)
        {
            if (graphNameTextBox.Text == "")
            {
                graphNameTextBox.Text = graphNamePlaceholder;
                graphNameTextBox.ForeColor = Color.LightGray;
            }
        }

        private void GraphDBForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
