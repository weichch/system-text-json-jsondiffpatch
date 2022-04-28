# Benchmarks

## Benchmarks with different options

_All benchmarks are generated using the same small JSON object used in the **Newtonsoft Json vs System.Text.Json** section below._

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1645 (21H1/May2021Update)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.200
  [Host]     : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  Job-ILXIOY : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|   Method |     Mean |      Min |      Max |      P80 |      P95 | Allocated |
|--------- |---------:|---------:|---------:|---------:|---------:|----------:|
|  RawText | 112.0 μs | 109.2 μs | 115.2 μs | 112.8 μs | 114.2 μs |     77 KB |
| Semantic | 116.1 μs | 113.8 μs | 121.7 μs | 117.0 μs | 119.3 μs |     76 KB |


## Newtonsoft Json vs System.Text.Json

_All benchmarks for `*_SystemTextJson` methods are generated with `JsonElementComparison.Semantic` option._

### Small JSON object

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1645 (21H1/May2021Update)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.200
  [Host]     : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  Job-HEQUNO : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|                    Method |      Mean |       Min |       Max |       P80 |       P95 | Allocated |
|-------------------------- |----------:|----------:|----------:|----------:|----------:|----------:|
|              Diff_JsonNet | 109.05 μs | 105.82 μs | 119.70 μs | 111.81 μs | 115.36 μs |    132 KB |
|       Diff_SystemTextJson |  88.99 μs |  87.03 μs |  90.95 μs |  89.82 μs |  90.78 μs |     66 KB |
|             Patch_JsonNet | 115.06 μs | 112.71 μs | 126.55 μs | 115.31 μs | 117.95 μs |    162 KB |
|      Patch_SystemTextJson |  41.39 μs |  40.67 μs |  42.67 μs |  41.62 μs |  42.03 μs |     35 KB |
|        DeepEquals_JsonNet |  70.90 μs |  68.80 μs |  74.54 μs |  71.31 μs |  72.68 μs |     91 KB |
| DeepEquals_SystemTextJson |  63.46 μs |  61.45 μs |  66.30 μs |  64.26 μs |  64.70 μs |     39 KB |
|         DeepClone_JsonNet |  49.64 μs |  48.66 μs |  50.77 μs |  50.05 μs |  50.39 μs |     70 KB |
|  DeepClone_SystemTextJson |  34.90 μs |  33.38 μs |  38.59 μs |  36.32 μs |  37.19 μs |     40 KB |


### Large JSON object

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1645 (21H1/May2021Update)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.200
  [Host]     : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  Job-BZNWDS : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|                    Method |     Mean |      Min |      Max |      P80 |      P95 | Allocated |
|-------------------------- |---------:|---------:|---------:|---------:|---------:|----------:|
|              Diff_JsonNet | 5.534 ms | 4.986 ms | 6.186 ms | 5.732 ms | 6.008 ms |      4 MB |
|       Diff_SystemTextJson | 5.026 ms | 4.725 ms | 5.229 ms | 5.124 ms | 5.216 ms |      3 MB |
|             Patch_JsonNet | 5.998 ms | 5.342 ms | 6.634 ms | 6.171 ms | 6.541 ms |      5 MB |
|      Patch_SystemTextJson | 2.603 ms | 2.276 ms | 2.965 ms | 2.720 ms | 2.934 ms |      2 MB |
|        DeepEquals_JsonNet | 2.986 ms | 2.684 ms | 3.512 ms | 3.252 ms | 3.307 ms |      2 MB |
| DeepEquals_SystemTextJson | 2.461 ms | 2.348 ms | 2.644 ms | 2.514 ms | 2.620 ms |      2 MB |
|         DeepClone_JsonNet | 2.029 ms | 1.991 ms | 2.067 ms | 2.047 ms | 2.061 ms |      2 MB |
|  DeepClone_SystemTextJson | 1.522 ms | 1.484 ms | 1.576 ms | 1.538 ms | 1.562 ms |      2 MB |


### `DeepEquals`

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1645 (21H1/May2021Update)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.200
  [Host]     : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  Job-MDGOSR : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|         Method |     Mean |      Min |      Max |      P80 |      P95 | Allocated |
|--------------- |---------:|---------:|---------:|---------:|---------:|----------:|
|        JsonNet | 17.22 μs | 16.87 μs | 17.73 μs | 17.37 μs | 17.48 μs |     23 KB |
| SystemTextJson | 16.76 μs | 16.44 μs | 17.07 μs | 16.87 μs | 16.99 μs |     10 KB |

