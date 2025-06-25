using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memoria_Cache
{
    // Classe para representar um conjunto da cache
    public class ConjuntoCache
    {
        public List<LinhaCache> Linhas { get; set; }
        public ConjuntoCache(int associatividade)
        {
            Linhas = new List<LinhaCache>(associatividade);
            for (int i = 0; i < associatividade; i++)
                Linhas.Add(new LinhaCache());
        }
    }
}
