# Benchmark

## Demo JSON object from jsondiffpatch

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1586 (21H1/May2021Update)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.200
  [Host]     : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  DefaultJob : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|                             Method |      Mean |       Min |       Max |       P80 |       P95 | Allocated |
|----------------------------------- |----------:|----------:|----------:|----------:|----------:|----------:|
|                       Diff_JsonNet |  87.71 μs |  86.27 μs |  88.53 μs |  88.25 μs |  88.48 μs |    132 KB |
|        Diff_SystemTextJson_Default |  82.01 μs |  80.91 μs |  83.15 μs |  82.51 μs |  83.04 μs |     72 KB |
|   Diff_SystemTextJson_JsonNetMatch |  75.55 μs |  74.45 μs |  76.33 μs |  76.21 μs |  76.32 μs |     66 KB |
|       Diff_SystemTextJson_Semantic |  87.25 μs |  86.39 μs |  87.98 μs |  87.63 μs |  87.85 μs |     74 KB |
|     Diff_SystemTextJson_ByPosition |  69.89 μs |  69.02 μs |  71.26 μs |  70.36 μs |  70.94 μs |     65 KB |
|          Diff_SystemTextJson_ByKey |  90.01 μs |  88.90 μs |  91.22 μs |  90.47 μs |  91.19 μs |     81 KB |
|                   Diff_JsonNet_Rfc | 103.56 μs | 103.06 μs | 104.45 μs | 103.87 μs | 104.19 μs |    150 KB |
|            Diff_SystemTextJson_Rfc |  87.76 μs |  87.25 μs |  88.52 μs |  88.00 μs |  88.46 μs |     88 KB |
|                      Patch_JsonNet |  96.73 μs |  95.62 μs |  98.51 μs |  97.58 μs |  98.17 μs |    162 KB |
|               Patch_SystemTextJson |  35.40 μs |  35.00 μs |  36.21 μs |  35.68 μs |  36.04 μs |     35 KB |
|                 DeepEquals_JsonNet |  59.26 μs |  58.22 μs |  60.59 μs |  60.18 μs |  60.43 μs |     91 KB |
|          DeepEquals_SystemTextJson |  46.49 μs |  45.83 μs |  47.54 μs |  46.78 μs |  47.15 μs |     39 KB |
| DeepEquals_SystemTextJson_Semantic |  59.41 μs |  58.67 μs |  59.94 μs |  59.77 μs |  59.91 μs |     46 KB |
|                  DeepClone_JsonNet |  42.95 μs |  42.35 μs |  43.89 μs |  43.26 μs |  43.81 μs |     70 KB |
|           DeepClone_SystemTextJson |  29.22 μs |  28.78 μs |  29.48 μs |  29.38 μs |  29.47 μs |     40 KB |


## Large JSON object

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1586 (21H1/May2021Update)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.200
  [Host]     : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  DefaultJob : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|                             Method |     Mean |      Min |      Max |      P80 |      P95 | Allocated |
|----------------------------------- |---------:|---------:|---------:|---------:|---------:|----------:|
|                       Diff_JsonNet | 3.829 ms | 3.756 ms | 3.937 ms | 3.884 ms | 3.919 ms |      4 MB |
|        Diff_SystemTextJson_Default | 3.238 ms | 3.198 ms | 3.260 ms | 3.247 ms | 3.253 ms |      4 MB |
|   Diff_SystemTextJson_JsonNetMatch | 3.411 ms | 3.338 ms | 3.524 ms | 3.456 ms | 3.485 ms |      3 MB |
|       Diff_SystemTextJson_Semantic | 3.808 ms | 3.748 ms | 3.868 ms | 3.830 ms | 3.854 ms |      3 MB |
|     Diff_SystemTextJson_ByPosition | 3.317 ms | 3.267 ms | 3.394 ms | 3.342 ms | 3.375 ms |      3 MB |
|          Diff_SystemTextJson_ByKey | 3.008 ms | 2.963 ms | 3.052 ms | 3.035 ms | 3.050 ms |      4 MB |
|                   Diff_JsonNet_Rfc | 5.379 ms | 5.269 ms | 5.496 ms | 5.461 ms | 5.479 ms |      6 MB |
|            Diff_SystemTextJson_Rfc | 4.529 ms | 4.467 ms | 4.579 ms | 4.566 ms | 4.579 ms |      5 MB |
|                      Patch_JsonNet | 4.274 ms | 4.159 ms | 4.436 ms | 4.367 ms | 4.407 ms |      5 MB |
|               Patch_SystemTextJson | 1.851 ms | 1.755 ms | 1.959 ms | 1.883 ms | 1.942 ms |      2 MB |
|                 DeepEquals_JsonNet | 2.239 ms | 2.206 ms | 2.279 ms | 2.253 ms | 2.269 ms |      2 MB |
|          DeepEquals_SystemTextJson | 1.734 ms | 1.687 ms | 1.796 ms | 1.756 ms | 1.775 ms |      2 MB |
| DeepEquals_SystemTextJson_Semantic | 2.444 ms | 2.274 ms | 2.551 ms | 2.523 ms | 2.536 ms |      2 MB |
|                  DeepClone_JsonNet | 2.022 ms | 1.764 ms | 2.660 ms | 2.109 ms | 2.472 ms |      2 MB |
|           DeepClone_SystemTextJson | 1.419 ms | 1.204 ms | 2.067 ms | 1.605 ms | 1.923 ms |      2 MB |


## `DeepEquals` benchmarks

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1586 (21H1/May2021Update)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.200
  [Host]     : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  DefaultJob : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|                    Method |       Mean |        Min |        Max |        P80 |        P95 | Allocated |
|-------------------------- |-----------:|-----------:|-----------:|-----------:|-----------:|----------:|
|                   JsonNet |   660.6 ns |   648.7 ns |   669.1 ns |   663.4 ns |   667.2 ns |   1,248 B |
|        JsonNet_JsonString | 1,609.8 ns | 1,578.0 ns | 1,650.9 ns | 1,628.1 ns | 1,641.2 ns |   5,920 B |
|            SystemTextJson |   554.1 ns |   540.7 ns |   564.5 ns |   559.1 ns |   562.1 ns |     864 B |
| SystemTextJson_JsonString | 1,539.9 ns | 1,501.2 ns | 1,575.1 ns | 1,549.9 ns | 1,570.8 ns |   1,264 B |
