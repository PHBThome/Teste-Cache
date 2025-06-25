using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memoria_Cache
{
    // Classe principal da cache
    public class MemoriaCache
    {
        private ParametrosSimulacao _parametros;
        private List<ConjuntoCache> conjuntos;
        private int linhasPorConjunto;
        private int numConjuntos;
        private int offsetBits;
        private int indiceBits;
        private int tagBits;
        private Random random;

        // Estatísticas
        public int TotalLeituras { get; private set; }
        public int TotalEscritas { get; private set; }
        public int AcertosLeitura { get; private set; }
        public int AcertosEscrita { get; private set; }
        public int FalhasLeitura { get; private set; }
        public int FalhasEscrita { get; private set; }
        public int EscritasMP { get; private set; }
        public int LeiturasMP { get; private set; }
        public double TempoTotalAcesso { get; private set; }

        public MemoriaCache(ParametrosSimulacao parametros)
        {
            _parametros = parametros;
            linhasPorConjunto = parametros.Associatividade;
            numConjuntos = parametros.NumeroLinhas / parametros.Associatividade;

            conjuntos = new List<ConjuntoCache>(numConjuntos);
            for (int i = 0; i < numConjuntos; i++)
                conjuntos.Add(new ConjuntoCache(linhasPorConjunto));

            offsetBits = (int)Math.Log2(parametros.TamanhoLinha);
            indiceBits = (int)Math.Log2(numConjuntos);
            tagBits = 32 - offsetBits - indiceBits;
            random = new Random();
        }

        public void ProcessarOperacao(OperacaoMemoria op)
        {
            uint endereco = op.Endereco;
            uint offset = endereco & (uint)(_parametros.TamanhoLinha - 1);
            uint indice = (endereco >> offsetBits) & (uint)(numConjuntos - 1);
            uint tag = endereco >> (offsetBits + indiceBits);
            var conjunto = conjuntos[(int)indice];
            bool hit = false;
            int linhaHit = -1;
            for (int i = 0; i < conjunto.Linhas.Count; i++)
            {
                var linha = conjunto.Linhas[i];
                if (linha.Valida && linha.Tag == tag)
                {
                    hit = true;
                    linhaHit = i;
                    break;
                }
            }
            if (op.Tipo == 'R')
            {
                TotalLeituras++;
                if (hit)
                {
                    AcertosLeitura++;
                    TempoTotalAcesso += _parametros.TempoHit;
                    AtualizarLRU(conjunto, linhaHit);
                }
                else
                {
                    FalhasLeitura++;
                    LeiturasMP++;
                    TempoTotalAcesso += _parametros.TempoHit + _parametros.TempoLeituraMP;
                    SubstituirLinha(conjunto, tag, false);
                }
            }
            else if (op.Tipo == 'W')
            {
                TotalEscritas++;
                if (hit)
                {
                    AcertosEscrita++;
                    if (_parametros.PoliticaEscrita == 0) // write-through
                    {
                        EscritasMP++;
                        TempoTotalAcesso += _parametros.TempoHit + _parametros.TempoEscritaMP;
                    }
                    else // write-back
                    {
                        conjunto.Linhas[linhaHit].Suja = true;
                        TempoTotalAcesso += _parametros.TempoHit;
                    }
                    AtualizarLRU(conjunto, linhaHit);
                }
                else
                {
                    FalhasEscrita++;
                    if (_parametros.PoliticaEscrita == 0) // write-through, write-non-allocate
                    {
                        EscritasMP++;
                        TempoTotalAcesso += _parametros.TempoHit + _parametros.TempoEscritaMP;
                    }
                    else // write-back, write-allocate
                    {
                        LeiturasMP++;
                        TempoTotalAcesso += _parametros.TempoHit + _parametros.TempoLeituraMP;
                        int linhaSub = SubstituirLinha(conjunto, tag, true);
                        conjunto.Linhas[linhaSub].Suja = true;
                    }
                }
            }
        }

        private void AtualizarLRU(ConjuntoCache conjunto, int linhaAcessada)
        {
            if (_parametros.PoliticaSubstituicao == "LRU")
            {
                for (int i = 0; i < conjunto.Linhas.Count; i++)
                {
                    if (i == linhaAcessada)
                        conjunto.Linhas[i].ContadorLRU = 0;
                    else if (conjunto.Linhas[i].Valida)
                        conjunto.Linhas[i].ContadorLRU++;
                }
            }
        }

        private int SubstituirLinha(ConjuntoCache conjunto, uint tag, bool escrita)
        {
            int linhaSubstituir = -1;
            // Procura linha inválida
            for (int i = 0; i < conjunto.Linhas.Count; i++)
            {
                if (!conjunto.Linhas[i].Valida)
                {
                    linhaSubstituir = i;
                    break;
                }
            }
            // Se não houver, aplica política de substituição
            if (linhaSubstituir == -1)
            {
                if (_parametros.PoliticaSubstituicao == "LRU")
                {
                    int maxLRU = -1;
                    for (int i = 0; i < conjunto.Linhas.Count; i++)
                    {
                        if (conjunto.Linhas[i].ContadorLRU > maxLRU)
                        {
                            maxLRU = conjunto.Linhas[i].ContadorLRU;
                            linhaSubstituir = i;
                        }
                    }
                }
                else // Aleatoria
                {
                    linhaSubstituir = random.Next(conjunto.Linhas.Count);
                }
                // Se write-back e suja, escreve na MP
                if (_parametros.PoliticaEscrita == 1 && conjunto.Linhas[linhaSubstituir].Suja)
                {
                    EscritasMP++;
                    conjunto.Linhas[linhaSubstituir].Suja = false;
                }
            }
            // Atualiza linha
            conjunto.Linhas[linhaSubstituir].Valida = true;
            conjunto.Linhas[linhaSubstituir].Tag = tag;
            conjunto.Linhas[linhaSubstituir].Suja = escrita && _parametros.PoliticaEscrita == 1;
            conjunto.Linhas[linhaSubstituir].ContadorLRU = 0;
            // Atualiza LRU das outras
            if (_parametros.PoliticaSubstituicao == "LRU")
            {
                for (int i = 0; i < conjunto.Linhas.Count; i++)
                {
                    if (i != linhaSubstituir && conjunto.Linhas[i].Valida)
                        conjunto.Linhas[i].ContadorLRU++;
                }
            }
            return linhaSubstituir;
        }

        public void AtualizarMemoriaPrincipalFinal()
        {
            if (_parametros.PoliticaEscrita == 1) // write-back
            {
                foreach (var conjunto in conjuntos)
                {
                    foreach (var linha in conjunto.Linhas)
                    {
                        if (linha.Valida && linha.Suja)
                        {
                            EscritasMP++;
                            linha.Suja = false;
                        }
                    }
                }
            }
        }

        public double CalcularTempoMedioAcesso(int totalOperacoes)
        {
            return TempoTotalAcesso / totalOperacoes;
        }
    }
}
