﻿using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch.Diffs
{
    internal readonly ref struct Lcs
    {
        private const int Equal = 1;
        private const int DeepEqual = 2;

        private readonly Dictionary<int, LcsEntry>? _lookupByLeftIndex;
        private readonly Dictionary<int, LcsEntry>? _lookupByRightIndex;
        private readonly int[]? _matrixRented;
        private readonly int[]? _matchMatrixRented;
        private readonly JsonValueWrapper[]? _wrapperCacheRented;
        private readonly int _rowSize;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Lcs(List<LcsEntry> indices, int[] matrixRented, int[] matchMatrixRented,
            JsonValueWrapper[]? wrapperCacheRented, int rowSize)
        {
            _lookupByLeftIndex = indices.ToDictionary(entry => entry.LeftIndex);
            _lookupByRightIndex = indices.ToDictionary(entry => entry.RightIndex);
            _matrixRented = matrixRented;
            _matchMatrixRented = matchMatrixRented;
            _wrapperCacheRented = wrapperCacheRented;
            _rowSize = rowSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool FindLeftIndex(int index, out LcsEntry result)
        {
            if (_lookupByLeftIndex is null)
            {
                result = default;
                return false;
            }

            return _lookupByLeftIndex.TryGetValue(index, out result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool FindRightIndex(int index, out LcsEntry result)
        {
            if (_lookupByRightIndex is null)
            {
                result = default;
                return false;
            }

            return _lookupByRightIndex.TryGetValue(index, out result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AreEqual(int indexX, int indexY, out bool deepEqual)
        {
            if (_matchMatrixRented is null)
            {
                deepEqual = false;
                return false;
            }

            var matchResult = _matchMatrixRented[(indexX + 1) * _rowSize + indexY + 1];
            deepEqual = matchResult == DeepEqual;
            return matchResult > 0;
        }

        public void Free()
        {
            if (_matrixRented is not null)
            {
                ArrayPool<int>.Shared.Return(_matrixRented);
            }

            if (_matchMatrixRented is not null)
            {
                ArrayPool<int>.Shared.Return(_matchMatrixRented);
            }

            if (_wrapperCacheRented is not null)
            {
                ArrayPool<JsonValueWrapper>.Shared.Return(_wrapperCacheRented);
            }
        }

        public static Lcs Get(Span<JsonNode?> x, Span<JsonNode?> y, JsonDiffOptions? options)
        {
            if (x.Length == 0 || y.Length == 0)
            {
                // At least one sequence is empty
                return default;
            }

            var m = x.Length + 1;
            var n = y.Length + 1;
            var matrixLength = m * n;
            var matrixRented = ArrayPool<int>.Shared.Rent(matrixLength);
            var matrix = matrixRented.AsSpan(0, matrixLength);
            var matchMatrixRented = ArrayPool<int>.Shared.Rent(matrixLength);
            var matchMatrixSpan = matchMatrixRented.AsSpan(0, matrixLength);
            // For performance reasons, we set materialized values into a cache.
            // We only cache JSON values as they are more efficient to cache than objects and arrays.
            var wrapperCacheRented = ArrayPool<JsonValueWrapper>.Shared.Rent(x.Length + y.Length);
            var wrapperCacheSpan = wrapperCacheRented.AsSpan(0, x.Length + y.Length);
            var comparerOptions = options?.CreateComparerOptions() ?? default;

            matrix.Fill(0);
            matchMatrixSpan.Fill(0);
            wrapperCacheSpan.Fill(default);

            for (var i = 1; i < m; i++)
            {
                if (x[i - 1] is JsonValue jsonValueX)
                {
                    wrapperCacheSpan[i - 1] = new JsonValueWrapper(jsonValueX);
                }
            }

            for (var j = 1; j < n; j++)
            {
                if (y[j - 1] is JsonValue jsonValueY)
                {
                    wrapperCacheSpan[x.Length + j - 1] = new JsonValueWrapper(jsonValueY);
                }
            }

            // Construct length matrix represented in a one-dimensional array using DP
            // https://en.wikipedia.org/wiki/Longest_common_subsequence_problem
            // https://www.geeksforgeeks.org/longest-common-subsequence-dp-4/
            //
            // Given a matrix:
            // 
            //   Y 0 1 2 3 ... N
            // X
            // 0
            // 1
            // 2
            // .
            // .
            // M
            // 
            // One-dimensional representation is:
            // [ [X=0, Y=[0 1 2 3 ... N]], [X=1, X=[0 1 2 3 ... N]], ..., [X=N, X=[0 1 2 3 ... N]] ]
            for (var i = 1; i < m; i++)
            {
                for (var j = 1; j < n; j++)
                {
                    var matchContext = new ArrayItemMatchContext(x[i - 1], i - 1, y[j - 1], j - 1);
                    bool itemMatched;
                    
                    if (x[i - 1] is JsonValue && y[j - 1] is JsonValue)
                    {
                        itemMatched = JsonDiffPatcher.MatchArrayValueItem(ref matchContext,
                            ref wrapperCacheSpan[i - 1],
                            ref wrapperCacheSpan[x.Length + j - 1],
                            options,
                            comparerOptions);
                    }
                    else
                    {
                        itemMatched = JsonDiffPatcher.MatchArrayItem(ref matchContext, options, comparerOptions);
                    }

                    if (itemMatched)
                    {
                        matrix[i * n + j] = 1 + matrix[(i - 1) * n + (j - 1)];
                        matchMatrixSpan[i * n + j] = matchContext.IsDeepEqual ? DeepEqual : Equal;
                    }
                    else
                    {
                        matrix[i * n + j] = Math.Max(
                            // above
                            matrix[(i - 1) * n + j],
                            // left
                            matrix[i * n + (j - 1)]);
                    }
                }
            }

            // Backtrack
            if (matrix[matrixLength - 1] == 0)
            {
                // No common value
                return default;
            }

            var entries = new List<LcsEntry>();
            for (int i = m - 1, j = n - 1; i > 0 && j > 0;)
            {
                if (matchMatrixSpan[i * n + j] > 0)
                {
                    // X[i - 1] == Y [j - 1]
                    entries.Insert(0, new LcsEntry(i - 1, j - 1,
                        matchMatrixSpan[i * n + j] == DeepEqual));
                    i--;
                    j--;
                }
                else
                {
                    var valueAbove = matrix[(i - 1) * n + j];
                    var valueLeft = matrix[i * n + (j - 1)];
                    if (valueAbove > valueLeft)
                    {
                        // Move to above
                        i--;
                    }
                    else
                    {
                        if (valueAbove == valueLeft)
                        {
                            var weightAbove = matchMatrixSpan[(i - 1) * n + j];
                            var weightLeft = matchMatrixSpan[(i - 1) * n + j];
                            if (weightAbove > weightLeft)
                            {
                                // Move to above, e.g. above was deeply equal
                                i--;
                                continue;
                            }
                        }

                        // Move to left
                        j--;
                    }
                }
            }

            return new Lcs(entries, matrixRented, matchMatrixRented, wrapperCacheRented, n);
        }

        internal readonly struct LcsEntry
        {
            public LcsEntry(int leftIndex, int rightIndex, bool deepEqual)
            {
                LeftIndex = leftIndex;
                RightIndex = rightIndex;
                AreDeepEqual = deepEqual;
            }

            public readonly int LeftIndex;
            public readonly int RightIndex;
            public readonly bool AreDeepEqual;
        }
    }
}
