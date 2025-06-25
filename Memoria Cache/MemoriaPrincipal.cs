using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memoria_Cache
{
    // Classe para simular a memória principal
    public class MemoriaPrincipal
    {
        public int TempoLeitura { get; set; }
        public int TempoEscrita { get; set; }
        public int TotalLeituras { get; set; }
        public int TotalEscritas { get; set; }
    }
}
