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

|   Method | FileSize |      Mean |    Median |       Min |       Max |       P80 |       P95 | Allocated |
|--------- |--------- |----------:|----------:|----------:|----------:|----------:|----------:|----------:|
|  RawText |    Small |  94.05 μs |  94.09 μs |  93.13 μs |  95.36 μs |  94.35 μs |  94.80 μs |     75 KB |
| Semantic |    Small | 104.65 μs | 104.27 μs | 102.67 μs | 107.86 μs | 105.55 μs | 107.76 μs |     75 KB |

\* _All benchmarks are generated using the same small JSON object used in the **System.Text.Json vs Newtonsoft Json** section below, with array move detection enabled (default)._

## System.Text.Json vs Newtonsoft Json

### Diff (including RFC JsonPatch)

|             Method | FileSize |        Mean |      Median |         Min |         Max |         P80 |         P95 | Allocated |
|------------------- |--------- |------------:|------------:|------------:|------------:|------------:|------------:|----------:|
|     **SystemTextJson** |    **Small** |    **76.93 μs** |    **76.88 μs** |    **75.62 μs** |    **79.28 μs** |    **77.43 μs** |    **78.11 μs** |     **67 KB** |
|            JsonNet |    Small |    84.97 μs |    84.75 μs |    83.72 μs |    87.68 μs |    85.64 μs |    86.38 μs |    132 KB |
| SystemTextJson_Rfc |    Small |    91.88 μs |    91.71 μs |    90.70 μs |    95.01 μs |    92.37 μs |    94.37 μs |     89 KB |
|        JsonNet_Rfc |    Small |   102.15 μs |   102.10 μs |   100.49 μs |   104.37 μs |   102.58 μs |   103.29 μs |    150 KB |
|     **SystemTextJson** |    **Large** | **3,739.64 μs** | **3,734.25 μs** | **3,626.78 μs** | **3,902.76 μs** | **3,781.22 μs** | **3,844.92 μs** |  **3,365 KB** |
|            JsonNet |    Large | 3,846.70 μs | 3,850.62 μs | 3,760.20 μs | 3,917.07 μs | 3,887.43 μs | 3,896.80 μs |  4,386 KB |
| SystemTextJson_Rfc |    Large | 4,897.11 μs | 4,868.30 μs | 4,722.99 μs | 5,196.12 μs | 4,930.06 μs | 5,159.49 μs |  4,667 KB |
|        JsonNet_Rfc |    Large | 5,260.99 μs | 5,249.26 μs | 5,121.82 μs | 5,487.74 μs | 5,322.84 μs | 5,460.47 μs |  6,147 KB |

### DeepEquals

|                  Method | FileSize |        Mean |      Median |         Min |         Max |         P80 |         P95 | Allocated |
|------------------------ |--------- |------------:|------------:|------------:|------------:|------------:|------------:|----------:|
|     **SystemTextJson_Node** |    **Small** |    **55.10 μs** |    **54.96 μs** |    **54.14 μs** |    **56.93 μs** |    **55.49 μs** |    **56.57 μs** |     **38 KB** |
| SystemTextJson_Document |    Small |    40.63 μs |    40.58 μs |    40.08 μs |    41.27 μs |    40.80 μs |    41.12 μs |     26 KB |
|                 JsonNet |    Small |    57.84 μs |    57.62 μs |    57.17 μs |    59.40 μs |    58.07 μs |    58.98 μs |     91 KB |
|     **SystemTextJson_Node** |    **Large** | **2,143.34 μs** | **2,125.71 μs** | **2,048.46 μs** | **2,328.43 μs** | **2,194.35 μs** | **2,266.60 μs** |  **1,571 KB** |
| SystemTextJson_Document |    Large | 1,372.31 μs | 1,371.00 μs | 1,352.61 μs | 1,391.00 μs | 1,379.30 μs | 1,388.30 μs |    920 KB |
|                 JsonNet |    Large | 2,208.71 μs | 2,209.77 μs | 2,182.51 μs | 2,246.30 μs | 2,223.80 μs | 2,235.96 μs |  2,426 KB |

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

\* _All benchmarks for `SystemTextJson` methods are generated with `JsonElementComparison.Semantic` option and array move detection disabled because JsonDiffPatch.Net does not support array move detection._