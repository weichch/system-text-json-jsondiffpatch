# Benchmark

## Demo JSON object from jsondiffpatch

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1586 (21H1/May2021Update)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.200
  [Host]     : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  DefaultJob : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|                             Method |      Mean |       Min |       Max |       P95 |       P80 | Allocated |
|----------------------------------- |----------:|----------:|----------:|----------:|----------:|----------:|
|                       Diff_JsonNet | 170.86 μs | 112.81 μs | 250.44 μs | 242.28 μs | 222.06 μs |    132 KB |
|                Diff_SystemTextJson | 161.13 μs | 110.60 μs | 272.51 μs | 245.99 μs | 194.82 μs |     70 KB |
|       Diff_SystemTextJson_Semantic | 188.61 μs | 121.51 μs | 312.77 μs | 243.53 μs | 223.13 μs |     77 KB |
|                   Diff_JsonNet_Rfc | 188.54 μs | 143.31 μs | 305.56 μs | 276.16 μs | 231.28 μs |    150 KB |
|            Diff_SystemTextJson_Rfc | 181.22 μs | 107.66 μs | 287.07 μs | 248.40 μs | 223.91 μs |     92 KB |
|                      Patch_JsonNet | 205.86 μs | 123.47 μs | 319.94 μs | 277.28 μs | 242.98 μs |    162 KB |
|               Patch_SystemTextJson |  78.75 μs |  54.53 μs | 113.21 μs |  98.50 μs |  88.62 μs |     37 KB |
|                 DeepEquals_JsonNet | 122.18 μs |  82.64 μs | 196.77 μs | 169.75 μs | 148.24 μs |     91 KB |
|          DeepEquals_SystemTextJson |  89.19 μs |  55.40 μs | 151.38 μs | 123.07 μs | 116.02 μs |     39 KB |
| DeepEquals_SystemTextJson_Semantic | 119.93 μs |  79.38 μs | 189.04 μs | 173.98 μs | 150.50 μs |     46 KB |
|                  DeepClone_JsonNet |  89.48 μs |  51.36 μs | 136.46 μs | 121.85 μs | 104.55 μs |     70 KB |
|           DeepClone_SystemTextJson |  64.91 μs |  38.96 μs |  97.91 μs |  87.33 μs |  77.57 μs |     45 KB |


## Large JSON object

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1586 (21H1/May2021Update)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.200
  [Host]     : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  DefaultJob : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|                             Method |      Mean |      Min |       Max |       P95 |       P80 | Allocated |
|----------------------------------- |----------:|---------:|----------:|----------:|----------:|----------:|
|                       Diff_JsonNet |  6.825 ms | 4.746 ms | 13.074 ms | 11.310 ms |  9.284 ms |      4 MB |
|                Diff_SystemTextJson |  5.898 ms | 4.996 ms |  7.424 ms |  6.910 ms |  6.329 ms |      3 MB |
|       Diff_SystemTextJson_Semantic |  6.466 ms | 4.683 ms | 10.261 ms |  8.130 ms |  7.464 ms |      4 MB |
|                   Diff_JsonNet_Rfc | 12.654 ms | 8.208 ms | 17.506 ms | 15.528 ms | 14.234 ms |      6 MB |
|            Diff_SystemTextJson_Rfc | 10.018 ms | 6.678 ms | 16.484 ms | 13.998 ms | 11.816 ms |      5 MB |
|                      Patch_JsonNet |  9.128 ms | 5.115 ms | 14.050 ms | 11.422 ms | 10.736 ms |      5 MB |
|               Patch_SystemTextJson |  3.772 ms | 2.576 ms |  5.572 ms |  4.933 ms |  4.421 ms |      2 MB |
|                 DeepEquals_JsonNet |  4.549 ms | 3.280 ms |  6.667 ms |  6.059 ms |  5.529 ms |      2 MB |
|          DeepEquals_SystemTextJson |  3.673 ms | 2.321 ms |  6.153 ms |  5.028 ms |  4.303 ms |      2 MB |
| DeepEquals_SystemTextJson_Semantic |  4.630 ms | 2.824 ms |  7.049 ms |  6.303 ms |  5.708 ms |      2 MB |
|                  DeepClone_JsonNet |  3.294 ms | 2.014 ms |  5.439 ms |  4.518 ms |  4.190 ms |      2 MB |
|           DeepClone_SystemTextJson |  1.761 ms | 1.475 ms |  2.107 ms |  2.000 ms |  1.927 ms |      2 MB |


## `DeepEquals` benchmarks

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1586 (21H1/May2021Update)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.200
  [Host]     : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  DefaultJob : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|                    Method |       Mean |        Min |        Max |        P95 |        P80 | Allocated |
|-------------------------- |-----------:|-----------:|-----------:|-----------:|-----------:|----------:|
|             JsonNet_Array | 1,198.9 ns |   907.5 ns | 1,704.5 ns | 1,557.5 ns | 1,349.9 ns |   1,248 B |
|        JsonNet_ParseArray | 2,879.1 ns | 2,216.4 ns | 3,697.3 ns | 3,361.4 ns | 3,124.2 ns |   5,920 B |
|      SystemTextJson_Array |   857.1 ns |   643.5 ns | 1,312.3 ns | 1,183.3 ns | 1,033.4 ns |     864 B |
| SystemTextJson_ParseArray | 2,141.7 ns | 2,027.8 ns | 2,188.0 ns | 2,184.4 ns | 2,159.7 ns |   1,264 B |

