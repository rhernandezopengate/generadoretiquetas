using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WFACodigosQR.Entidades;

namespace WFACodigosQR
{
    public partial class frmPallets : Form
    {
        public frmPallets()
        {
            InitializeComponent();
        }

        Pallets etiqueta = null;
        List<Pallets> listaPallets;

        private void frmPallets_Load(object sender, EventArgs e)
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

        private void btnAbrirArchivo_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            txtArchivo.Text = openFileDialog1.FileName;
            MostrarInformacion(txtArchivo.Text);
        }

        private void MostrarInformacion(string filePath)
        {
            try
            {
                listaPallets = new List<Pallets>();
                string lineas = System.IO.File.ReadAllText(filePath);

                //string[] lines = File.ReadAllLines(path);
                //foreach (var line in lines)
                //{
                //    var firstValue = line.Split(new string[] { "        " }, StringSplitOptions.RemoveEmptyEntries)[0];
                //    Console.WriteLine(firstValue);
                //}

                DataTable dtCharge = new DataTable();

                dtCharge.Columns.AddRange(new DataColumn[6] {
                        //1
                        new DataColumn("Producto", typeof(string)),
                        //2                        
                        new DataColumn("Descripcion", typeof(string)),
                        //3
                        new DataColumn("Lote", typeof(string)),
                        //4
                        new DataColumn("Tag", typeof(string)),
                        //5
                        new DataColumn("QTY", typeof(string)),
                        //6    
                    new DataColumn("Bin", typeof(string)),
                    });


                foreach (string row in lineas.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        dtCharge.Rows.Add();
                        int i = 0;

                        //Execute a loop over the columns.
                        foreach (string cell in row.Split(','))
                        {
                            dtCharge.Rows[dtCharge.Rows.Count - 1][i] = cell;

                            i++;
                        }
                    }
                }

                foreach (DataRow row in dtCharge.Rows)
                {
                    Pallets ordenes = new Pallets();
                    ordenes.sku = row[0].ToString();
                    ordenes.descripcion = row[1].ToString();
                    ordenes.lote = row[2].ToString();
                    ordenes.tag = row[3].ToString();
                    ordenes.qty = int.Parse(row[4].ToString());
                    ordenes.bin = row[5].ToString();

                    listaPallets.Add(ordenes);                  
                }

                if (listaPallets.Count > 0)
                {
                    dataGridView1.DataSource = listaPallets;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha Ocurrido un error:" + ex.Message.ToString());
            }        
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            ImprimirEtiquetas();
        }

        public void ImprimirEtiquetas()
        {
            foreach (var item in listaPallets)
            {
                etiqueta = new Pallets();
                etiqueta = item;
                ConfiguracionesImpresora();
            }

            MessageBox.Show("Impresion Correcta");
        }

        public void ConfiguracionesImpresora()
        {
            try
            {
                PrintDocument pd = new PrintDocument();



                pd.PrintPage += new PrintPageEventHandler(ConfiguracionImpresion);
                // Especifica que impresora se utilizara!!

                pd.PrinterSettings.PrinterName = (string)comboBox1.SelectedItem;
                PageSettings pa = new PageSettings();
                pa.Margins = new Margins(0, 0, 0, 0);
                pd.DefaultPageSettings.Margins = pa.Margins;
                PaperSize ps = new PaperSize("Custom", 2200, 800);

                pd.DefaultPageSettings.PaperSize = ps;
                pd.DefaultPageSettings.Landscape = true;

                pd.Print();
            }
            catch (Exception exp)
            {
                MessageBox.Show("Error al imprimir: " + exp.Message);
            }
        }

        private void ConfiguracionImpresion(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            string sku = etiqueta.sku;
            string lote = etiqueta.lote;
            string descripcion = etiqueta.descripcion;
            string tag = etiqueta.tag;
            string qty = etiqueta.qty.ToString();
            string bin = etiqueta.bin;

            Font font14 = new Font("Arial", 15, FontStyle.Bold);
            float leading = 230;
            float startX = 0;
            float startY = 0;
            float Offset = 0;
            float lineheight14 = font14.GetHeight() + leading;

            Brush brush = Brushes.Black;
            

            StringFormat formatLeft = new StringFormat(StringFormatFlags.NoClip);
            StringFormat formatCenter = new StringFormat(formatLeft);
            formatCenter.Alignment = StringAlignment.Center;
            Zen.Barcode.Code128BarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;

            //SKU
            Bitmap bmp = (Bitmap)barcode.Draw(sku, 10);            
            Graphics g = e.Graphics;
            SizeF layoutSize = new SizeF(630 - Offset * 2, lineheight14);
            RectangleF layout = new RectangleF(new PointF(0, 0 + Offset), layoutSize);            
            g.DrawString("Product #:\n" + sku, font14, brush, layout, formatCenter);
            g.DrawImage(bmp, 220, 70, 200, 100);

            ////Lote
            Bitmap bmp1 = (Bitmap)barcode.Draw(lote, 10);
            Graphics g1 = e.Graphics;
            SizeF layoutSize1 = new SizeF(630 - Offset * 2, lineheight14);
            RectangleF layout1 = new RectangleF(new PointF(310, 0 + Offset), layoutSize1);            
            g1.DrawString("Lot #:\n" + lote, font14, brush, layout1, formatCenter);
            g1.DrawImage(bmp1, 530, 50, 200, 50);

            //Tag
            Bitmap bmp2 = (Bitmap)barcode.Draw(tag, 10);
            Graphics g2 = e.Graphics;
            SizeF layoutSize2 = new SizeF(630 - Offset * 2, lineheight14);
            RectangleF layout2 = new RectangleF(new PointF(0, 200 + Offset), layoutSize2);
            g2.DrawString("Tag/Serial #:\n" + tag, font14, brush, layout2, formatCenter);
            g2.DrawImage(bmp2, 220, 270, 200, 100);

            //QTY
            Bitmap bmp3 = (Bitmap)barcode.Draw(qty, 10);
            Graphics g3 = e.Graphics;
            SizeF layoutSize3 = new SizeF(630 - Offset * 2, lineheight14);
            RectangleF layout3 = new RectangleF(new PointF(310, 150 + Offset), layoutSize3);
            g3.DrawString("QTY:\n" + qty, font14, brush, layout3, formatCenter);
            g3.DrawImage(bmp3, 530, 200, 200, 50);

            Graphics g4 = e.Graphics;
            SizeF layoutSize4 = new SizeF(630 - Offset * 2, lineheight14);
            RectangleF layout4 = new RectangleF(new PointF(310, 300 + Offset), layoutSize4);
            g4.DrawString("BIN LOC:\n" + bin, font14, brush, layout4, formatCenter);            
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            Form menu = new frmMenu();
            menu.Show();
            this.Close();

        }
    }
}
