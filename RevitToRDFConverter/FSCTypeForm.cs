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
    public partial class FSCTypeForm : Form
    {
        List<FSCTypeComponent> comps;
        public FSCTypeForm(List<FSCTypeComponent> comps)
        {
            this.comps = comps;
            InitializeComponent();
        }

        private void FSCTypeForm_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = comps;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
