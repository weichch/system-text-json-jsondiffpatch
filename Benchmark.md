# Benchmarks

## Hardware and Software

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1645 (21H1/May2021Update)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.200
  [Host]     : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  Job-ILXIOY : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```

## Comparison Modes

_All benchmarks are generated using the same small JSON object used in the **Newtonsoft Json vs System.Text.Json** section below._

|   Method | FileSize |     Mean |   Median |      Min |      Max |      P80 |      P95 | Allocated |
|--------- |--------- |---------:|---------:|---------:|---------:|---------:|---------:|----------:|
|  RawText |    Small | 100.9 μs | 100.3 μs | 98.36 μs | 105.7 μs | 102.0 μs | 104.9 μs |     77 KB |
| Semantic |    Small | 102.2 μs | 101.9 μs | 99.37 μs | 107.0 μs | 103.1 μs | 105.6 μs |     76 KB |

## Newtonsoft Json vs System.Text.Json

_All benchmarks for `SystemTextJson` methods are generated with `JsonElementComparison.Semantic` option._

### Diff (including RFC JsonPatch)

|             Method | FileSize |        Mean |      Median |         Min |         Max |         P80 |         P95 | Allocated |
|------------------- |--------- |------------:|------------:|------------:|------------:|------------:|------------:|----------:|
|     **SystemTextJson** |    **Small** |    **79.09 μs** |    **78.25 μs** |    **75.72 μs** |    **84.89 μs** |    **81.91 μs** |    **84.16 μs** |     **66 KB** |
|            JsonNet |    Small |    91.98 μs |    92.15 μs |    90.06 μs |    94.50 μs |    92.77 μs |    93.78 μs |    132 KB |
| SystemTextJson_Rfc |    Small |    94.88 μs |    94.74 μs |    93.34 μs |    97.80 μs |    95.69 μs |    96.86 μs |     87 KB |
|        JsonNet_Rfc |    Small |   106.41 μs |   106.01 μs |   103.58 μs |   110.69 μs |   107.38 μs |   109.75 μs |    150 KB |
|     **SystemTextJson** |    **Large** | **3,717.44 μs** | **3,700.53 μs** | **3,577.00 μs** | **3,913.15 μs** | **3,766.22 μs** | **3,901.23 μs** |  **3,258 KB** |
|            JsonNet |    Large | 4,104.18 μs | 4,085.60 μs | 3,922.80 μs | 4,343.10 μs | 4,199.56 μs | 4,273.98 μs |  4,386 KB |
| SystemTextJson_Rfc |    Large | 4,900.93 μs | 4,890.86 μs | 4,772.28 μs | 5,128.16 μs | 4,958.30 μs | 5,021.94 μs |  4,561 KB |
|        JsonNet_Rfc |    Large | 5,569.83 μs | 5,535.12 μs | 5,354.93 μs | 5,976.46 μs | 5,682.46 μs | 5,822.63 μs |  6,147 KB |

### DeepEquals

|         Method | FileSize |        Mean |      Median |         Min |         Max |         P80 |         P95 | Allocated |
|--------------- |--------- |------------:|------------:|------------:|------------:|------------:|------------:|----------:|
| **SystemTextJson** |    **Small** |    **52.92 μs** |    **52.89 μs** |    **52.00 μs** |    **54.46 μs** |    **53.31 μs** |    **53.90 μs** |     **39 KB** |
|        JsonNet |    Small |    58.82 μs |    58.77 μs |    57.78 μs |    60.41 μs |    59.16 μs |    59.74 μs |     91 KB |
| **SystemTextJson** |    **Large** | **2,099.55 μs** | **2,090.78 μs** | **1,963.92 μs** | **2,302.56 μs** | **2,161.80 μs** | **2,223.10 μs** |  **1,631 KB** |
|        JsonNet |    Large | 2,296.54 μs | 2,293.76 μs | 2,239.68 μs | 2,393.52 μs | 2,323.09 μs | 2,378.88 μs |  2,426 KB |

### Patch

|         Method | FileSize |        Mean |      Median |         Min |         Max |         P80 |         P95 | Allocated |
|--------------- |--------- |------------:|------------:|------------:|------------:|------------:|------------:|----------:|
| **SystemTextJson** |    **Small** |    **35.45 μs** |    **35.42 μs** |    **34.43 μs** |    **36.97 μs** |    **35.86 μs** |    **36.52 μs** |     **35 KB** |
|        JsonNet |    Small |    95.50 μs |    95.35 μs |    94.14 μs |    97.36 μs |    96.28 μs |    96.70 μs |    162 KB |
| **SystemTextJson** |    **Large** | **1,945.77 μs** | **1,935.61 μs** | **1,799.91 μs** | **2,203.39 μs** | **2,047.02 μs** | **2,093.61 μs** |  **1,732 KB** |
|        JsonNet |    Large | 4,324.16 μs | 4,315.50 μs | 4,184.21 μs | 4,506.67 μs | 4,378.94 μs | 4,433.86 μs |  5,088 KB |

### DeepClone

|         Method | FileSize |        Mean |      Median |         Min |         Max |         P80 |         P95 | Allocated |
|--------------- |--------- |------------:|------------:|------------:|------------:|------------:|------------:|----------:|
| **SystemTextJson** |    **Small** |    **28.98 μs** |    **29.05 μs** |    **27.99 μs** |    **29.53 μs** |    **29.29 μs** |    **29.42 μs** |     **40 KB** |
|        JsonNet |    Small |    42.99 μs |    42.84 μs |    41.90 μs |    45.02 μs |    43.41 μs |    44.70 μs |     70 KB |
| **SystemTextJson** |    **Large** | **1,251.60 μs** | **1,247.97 μs** | **1,192.19 μs** | **1,323.97 μs** | **1,276.05 μs** | **1,310.40 μs** |  **1,675 KB** |
|        JsonNet |    Large | 1,708.43 μs | 1,706.69 μs | 1,664.39 μs | 1,783.04 μs | 1,731.47 μs | 1,759.00 μs |  2,128 KB |
