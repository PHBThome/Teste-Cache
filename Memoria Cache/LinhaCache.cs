using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memoria_Cache
{
    // Classe para representar uma linha da cache
    public class LinhaCache
    {
        public uint Tag { get; set; }
        public bool Valida { get; set; }
        public bool Suja { get; set; } // Para write-back
        public int ContadorLRU { get; set; } // Para LRU
    }
}
