using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memoria_Cache
{
    // Classe para representar uma operação de memória
    public class OperacaoMemoria
    {
        public uint Endereco { get; set; }
        public char Tipo { get; set; } // 'R' ou 'W'
    }
}
