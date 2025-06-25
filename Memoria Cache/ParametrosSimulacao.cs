using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memoria_Cache
{

    // Classe para armazenar os parâmetros da simulação
    public class ParametrosSimulacao
    {
        public int PoliticaEscrita { get; set; } // 0: write-through, 1: write-back
        public int TamanhoLinha { get; set; }
        public int NumeroLinhas { get; set; }
        public int Associatividade { get; set; }
        public int TempoHit { get; set; }
        public string PoliticaSubstituicao { get; set; } // "LRU" ou "Aleatoria"
        public int TempoLeituraMP { get; set; }
        public int TempoEscritaMP { get; set; }
        public string ArquivoOperacoes { get; set; }
    }
}
