using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFACodigosQR.Entidades
{
    public class Pallets
    {
        public string sku { get; set; }

        public string descripcion { get; set; }

        public string lote { get; set; }
        
        public string tag { get; set; }

        public int qty { get; set; }

        public string bin { get; set; }
    }
}
