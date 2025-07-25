Este projeto é um simulador de memória cache associativa por conjunto, com arquitetura totalmente configurável. O objetivo é analisar o impacto de diferentes parâmetros da cache 
no desempenho do sistema.

##Descrição

O simulador processa um arquivo de operações (`.cache`) contendo endereços de memória e operações (leitura ou escrita). A partir de parâmetros definidos pelo usuário, o programa 
simula o comportamento de uma memória cache e gera estatísticas detalhadas sobre seu desempenho.

##Funcionalidades

- Suporte a políticas de escrita: **Write-through** e **Write-back**
- Políticas de substituição: **LRU** (Least Recently Used) e **Aleatória**
- Associatividade por conjunto configurável
- Tamanho da linha de cache e número total de linhas configuráveis
- Tempos de acesso configuráveis para cache e memória principal
- Geração de estatísticas:
  - Total de leituras e escritas
  - Taxa de acertos (global, leitura, escrita)
  - Tempo médio de acesso
  - Operações realizadas na memória principal

##Entradas

- Arquivo `.cache` com as operações:
  - Formato: `endereco_hexadecimal operação`, ex: `05fea840 W`
- Parâmetros definidos pelo usuário:
  - Política de escrita (0: Write-through | 1: Write-back)
  - Tamanho da linha (em bytes)
  - Número de linhas da cache
  - Associatividade (linhas por conjunto)
  - Tempo de hit na cache (em nanossegundos)
  - Política de substituição (LRU ou Aleatória)
  - Tempo de leitura e escrita da memória principal (em nanossegundos)
  - Caminho do arquivo de operações

##Como Executar

1. Abra o projeto no **Visual Studio**
2. Compile e execute (`Ctrl + F5`)
3. Insira os parâmetros solicitados via console
4. Veja os resultados da simulação diretamente no terminal
