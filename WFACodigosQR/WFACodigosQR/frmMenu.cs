using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WFACodigosQR
{
    public partial class frmMenu : Form
    {
        public frmMenu()
        {
            InitializeComponent();
        }

        private void frmMenu_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form ubicaciones = new frmUbicaciones();
            ubicaciones.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form pallets = new frmPallets();
            pallets.Show();
            this.Hide();
        }
    }
}
