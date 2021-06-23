using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WFACodigosQR.Entidades;

using System.Drawing.Printing;

namespace WFACodigosQR
{
    public partial class frmUbicaciones : Form
    {
        public frmUbicaciones()
        {
            InitializeComponent();
        }

        string etiqueta = "";
        List<Ubicaciones> listaUbicaciones;

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            txtArchivo.Text = openFileDialog1.FileName;
            MostrarInformacion(txtArchivo.Text);
        }

        private void MostrarInformacion(string filePath)
        {
            try
            {
                string[] lineas = System.IO.File.ReadAllLines(filePath);

                string primeraLinea = lineas[0];

                string[] encabezados = primeraLinea.Split(',');

                listaUbicaciones = new List<Ubicaciones>();

                //For Data
                for (int i = 1; i < lineas.Length; i++)
                {
                    string[] dataWords = lineas[i].Split(',');

                    int columnIndex = 0;
                    foreach (string palabraEncabezado in encabezados)
                    {
                        Ubicaciones ubicaciones = new Ubicaciones();
                        ubicaciones.Ubicacion = dataWords[columnIndex++];
                        listaUbicaciones.Add(ubicaciones);
                    }
                }

                if (listaUbicaciones.Count > 0)
                {
                    dataGridView1.DataSource = listaUbicaciones;
                }

                MessageBox.Show("Carga Completa");
            }
            catch (Exception)
            {
                MessageBox.Show("Error al cargar");               
            }      
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ImprimirEtiquetas();            
        }

        public void ImprimirEtiquetas() 
        {
            foreach (var item in listaUbicaciones)
            {
                etiqueta = item.Ubicacion;
                ImprimirCaja();
            }

            MessageBox.Show("Impresion Correcta");
        }

        public void ImprimirCaja()
        {
            try
            {
                PrintDocument pd = new PrintDocument();

                pd.PrintPage += new PrintPageEventHandler(ImprimirEtiquetaCaja);
                // Especifica que impresora se utilizara!!

                pd.PrinterSettings.PrinterName = (string)comboBox1.SelectedItem;
                PageSettings pa = new PageSettings();
                pa.Margins = new Margins(0, 0, 0, 0);
                pd.DefaultPageSettings.Margins = pa.Margins;
                PaperSize ps = new PaperSize("Custom", 2200, 800);

                pd.DefaultPageSettings.PaperSize = ps;
                pd.DefaultPageSettings.Landscape = false;

                pd.Print();
            }
            catch (Exception exp)
            {
                MessageBox.Show("Error al imprimir: " + exp.Message);
            }
        }

        private void ImprimirEtiquetaCaja(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            MessagingToolkit.QRCode.Codec.QRCodeEncoder encoder = new MessagingToolkit.QRCode.Codec.QRCodeEncoder();
            encoder.QRCodeScale = 8;

            Font font14 = new Font("Arial", 40, FontStyle.Bold);            
            float leading = 230;
            float startX = 0;
            float startY = leading;
            float Offset = 0;
            float lineheight14 = font14.GetHeight() + leading;

            Brush brush = Brushes.Black;
            SizeF layoutSize = new SizeF(380 - Offset * 2, lineheight14);
            RectangleF layout = new RectangleF(new PointF(startX, startY + Offset), layoutSize);

            StringFormat formatLeft = new StringFormat(StringFormatFlags.NoClip);
            StringFormat formatCenter = new StringFormat(formatLeft);
            formatCenter.Alignment = StringAlignment.Center;

            Bitmap bmp = encoder.Encode(etiqueta);
            Graphics g = e.Graphics;
            g.DrawImage(bmp, 90, 25, 200, 200);
            g.DrawString(etiqueta, font14, brush, layout, formatCenter);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            PrintDocument prtdoc = new PrintDocument();
            string strDefaultPrinter = prtdoc.PrinterSettings.PrinterName;
            foreach (String strPrinter in PrinterSettings.InstalledPrinters)
            {
                comboBox1.Items.Add(strPrinter);
                if (strPrinter == strDefaultPrinter)
                {
                    comboBox1.SelectedIndex = comboBox1.Items.IndexOf(strPrinter);
                }
            }
        }

        private void btnSalir_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMenu_Click_1(object sender, EventArgs e)
        {
            Form menu = new frmMenu();
            menu.Show();
            this.Close();
        }
    }
}
