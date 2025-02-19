using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RevitToRDFConverter
{
    public partial class GraphDBURLForm : Form
    {
        public string urlNamePlaceholder;
        public string url;
        public GraphDBURLForm()
        {
            InitializeComponent();
        }

        private void urlBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void GraphDBURLForm_Load(object sender, EventArgs e)
        {
            urlNamePlaceholder = "cme-pbms01.win.dtu.dk:7200";
            urlBox.Text = urlNamePlaceholder;
            urlBox.ForeColor = Color.LightGray;
        }

        private void okButton_Click(object sender, EventArgs e)
        {

            url = urlBox.Text;

            okButton.DialogResult = DialogResult.OK;

            Close();
            return;
            
        }

        private void urlBox_Enter(object sender, EventArgs e)
        {
            if (urlBox.Text == urlNamePlaceholder)
            {
                urlBox.Text = "";
                urlBox.ForeColor = Color.Black;
            }
        }

        private void urlBox_Leave(object sender, EventArgs e)
        {
            if (urlBox.Text == "")
            {
                urlBox.Text = urlNamePlaceholder;
                urlBox.ForeColor = Color.LightGray;
            }
        }
    }
}
