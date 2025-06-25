using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Memoria_Cache
{  
    internal class Program
    {
        static void Main(string[] args)
        {
            bool continuar;
            do
            {
                var parametros = LerParametros();
                var operacoes = LerOperacoes(parametros.ArquivoOperacoes);
                var cache = new MemoriaCache(parametros);
                foreach (var op in operacoes)
                    cache.ProcessarOperacao(op);
                cache.AtualizarMemoriaPrincipalFinal();
                ImprimirResultados(parametros, operacoes, cache);
                Console.WriteLine("-------------------");
                Console.WriteLine("Deseja fazer outro teste? (s/n)");
                char opcao = char.ToLower(char.Parse(Console.ReadLine()));
                continuar = (opcao == 's');
            }
            while(continuar);
        }

        static ParametrosSimulacao LerParametros()
        {
            Console.WriteLine("Digite o caminho do arquivo de operações:");
            string arquivo = Console.ReadLine();
            Console.WriteLine("Política de escrita (0=write-through, 1=write-back):");
            int politicaEscrita = int.Parse(Console.ReadLine());
            Console.WriteLine("Tamanho da linha (bytes, potência de 2):");
            int tamanhoLinha = int.Parse(Console.ReadLine());
            Console.WriteLine("Número de linhas (potência de 2):");
            int numeroLinhas = int.Parse(Console.ReadLine());
            Console.WriteLine("Associatividade (linhas por conjunto, potência de 2):");
            int associatividade = int.Parse(Console.ReadLine());
            Console.WriteLine("Tempo de acesso (hit) na cache (ns):");
            int tempoHit = int.Parse(Console.ReadLine());
            Console.WriteLine("Política de substituição (LRU/Aleatoria):");
            string politicaSubstituicao = Console.ReadLine();
            Console.WriteLine("Tempo de leitura da memória principal (ns):");
            int tempoLeituraMP = int.Parse(Console.ReadLine());
            Console.WriteLine("Tempo de escrita da memória principal (ns):");
            int tempoEscritaMP = int.Parse(Console.ReadLine());

            return new ParametrosSimulacao
            {
                PoliticaEscrita = politicaEscrita,
                TamanhoLinha = tamanhoLinha,
                NumeroLinhas = numeroLinhas,
                Associatividade = associatividade,
                TempoHit = tempoHit,
                PoliticaSubstituicao = politicaSubstituicao.Trim().ToUpperInvariant(),
                TempoLeituraMP = tempoLeituraMP,
                TempoEscritaMP = tempoEscritaMP,
                ArquivoOperacoes = arquivo
            };
        }

        static List<OperacaoMemoria> LerOperacoes(string arquivo)
        {
            var lista = new List<OperacaoMemoria>();
            foreach (var linha in File.ReadLines(arquivo))
            {
                if (string.IsNullOrWhiteSpace(linha)) continue;
                var partes = linha.Trim().Split(' ');
                uint endereco = uint.Parse(partes[0], NumberStyles.HexNumber);
                char tipo = partes[1][0];
                lista.Add(new OperacaoMemoria { Endereco = endereco, Tipo = tipo });
            }
            return lista;
        }

        static void ImprimirResultados(ParametrosSimulacao parametros, List<OperacaoMemoria> operacoes, MemoriaCache cache)
        {
            int totalLeituras = cache.TotalLeituras;
            int totalEscritas = cache.TotalEscritas;
            int total = totalLeituras + totalEscritas;
            Console.WriteLine("\n--- RESULTADOS DA SIMULAÇÃO ---");
            Console.WriteLine($"Arquivo de operações: {parametros.ArquivoOperacoes}");
            Console.WriteLine($"Política de escrita: {(parametros.PoliticaEscrita == 0 ? "write-through" : "write-back")}");
            Console.WriteLine($"Tamanho da linha: {parametros.TamanhoLinha} bytes");
            Console.WriteLine($"Número de linhas: {parametros.NumeroLinhas}");
            Console.WriteLine($"Associatividade: {parametros.Associatividade}");
            Console.WriteLine($"Tempo de acesso (hit): {parametros.TempoHit} ns");
            Console.WriteLine($"Política de substituição: {parametros.PoliticaSubstituicao}");
            Console.WriteLine($"Tempo de leitura MP: {parametros.TempoLeituraMP} ns");
            Console.WriteLine($"Tempo de escrita MP: {parametros.TempoEscritaMP} ns");
            Console.WriteLine($"Total de operações: {total}");
            Console.WriteLine($"Total de leituras: {totalLeituras}");
            Console.WriteLine($"Total de escritas: {totalEscritas}");
            Console.WriteLine($"Leituras na memória principal: {cache.LeiturasMP}");
            Console.WriteLine($"Escritas na memória principal: {cache.EscritasMP}");
            double taxaAcertoLeitura = totalLeituras > 0 ? (double)cache.AcertosLeitura / totalLeituras : 0;
            double taxaAcertoEscrita = totalEscritas > 0 ? (double)cache.AcertosEscrita / totalEscritas : 0;
            double taxaAcertoGlobal = total > 0 ? (double)(cache.AcertosLeitura + cache.AcertosEscrita) / total : 0;
            Console.WriteLine($"Taxa de acerto leitura: {taxaAcertoLeitura:F4} ({cache.AcertosLeitura})");
            Console.WriteLine($"Taxa de acerto escrita: {taxaAcertoEscrita:F4} ({cache.AcertosEscrita})");
            Console.WriteLine($"Taxa de acerto global: {taxaAcertoGlobal:F4} ({cache.AcertosLeitura + cache.AcertosEscrita})");
            double tempoMedio = cache.CalcularTempoMedioAcesso(total);
            Console.WriteLine($"Tempo médio de acesso à cache: {tempoMedio:F4} ns");
        }
    }
}
