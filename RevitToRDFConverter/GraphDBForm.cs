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

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            repositoryId = repositoryIdDropDown.Text;

            okButton.DialogResult = DialogResult.OK;

            Close();
            return;
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
    }
}
