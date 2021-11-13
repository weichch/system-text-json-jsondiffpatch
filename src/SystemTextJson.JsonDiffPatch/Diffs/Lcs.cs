using System.Buffers;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace System.Text.Json.Diffs
{
    internal readonly struct Lcs
    {
        private readonly List<LcsEntry>? _entries;

        public Lcs(List<LcsEntry> entries)
        {
            _entries = entries;
        }

        public static Lcs Get(Span<JsonNode?> x,  Span<JsonNode?> y)
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
            var equalityMatrixRented = ArrayPool<bool>.Shared.Rent(matrixLength);
            var equalityMatrix = equalityMatrixRented.AsSpan(0, matrixLength);

            // Initializes the matrix
            matrix.Fill(0);
            equalityMatrix.Fill(false);

            try
            {
                // Construct length matrix represented in a one-dimensional array using DP
                // https://en.wikipedia.org/wiki/Longest_common_subsequence_problem
                // https://www.geeksforgeeks.org/longest-common-subsequence-dp-4/
                //
                // Given a matrix:
                // 
                //   X 0 1 2 3 ... M
                // Y
                // 0
                // 1
                // 2
                // .
                // .
                // N
                // 
                // One-dimensional representation is:
                // [ [Y=0, X=[0 1 2 3 ... M]], [Y=1, X=[0 1 2 3 ... M]], ..., [Y=N, X=[0 1 2 3 ... M]] ]

                for (var i = 1; i < m; i++)
                {
                    for (var j = 1; j < n; j++)
                    {
                        if (x[i - 1].DeepEquals(y[j - 1]))
                        {
                            matrix[j * m + i] = 1 + matrix[(j - 1) * m + (i - 1)];
                            equalityMatrix[j * m + i] = true;
                        }
                        else
                        {
                            matrix[j * m + i] = Math.Max(
                                // above
                                matrix[(j - 1) * m + i],
                                // left
                                matrix[j * m + (i - 1)]);
                        }
                    }
                }

                // Backtrack
                if (matrix[matrixLength - 1] == 0)
                {
                    // No common value
                    return default;
                }

                var entries = new List<LcsEntry>(Math.Min(x.Length, y.Length));
                for (int i = m - 1, j = n - 1; i > 0 && j > 0;)
                {
                    if (equalityMatrix[j * m + i])
                    {
                        // X[i - 1] == Y [j - 1]
                        entries.Insert(0, new(x[i - 1], i - 1, j - 1));
                        i--;
                        j--;
                    }
                    else
                    {
                        var valueAbove = matrix[(j - 1) * m + i];
                        var valueLeft = matrix[j * m + (i - 1)];
                        // This movement MUST BE identical to:
                        // https://github.com/benjamine/jsondiffpatch/blob/a8cde4c666a8a25d09d8f216c7f19397f2e1b569/src/filters/lcs.js#L59
                        if (valueAbove > valueLeft)
                        {
                            // Move to above
                            j--;
                        }
                        else
                        {
                            // Move to left
                            i--;
                        }
                    }
                }

                return new Lcs(entries);
            }
            finally
            {
                ArrayPool<int>.Shared.Return(matrixRented);
                ArrayPool<bool>.Shared.Return(equalityMatrixRented);
            }
        }

        /// <summary>
        /// An entry in <see cref="Lcs"/>.
        /// </summary>
        public readonly struct LcsEntry
        {
            public LcsEntry(JsonNode? value, int indexX, int indexY)
            {
                Value = value;
                IndexX = indexX;
                IndexY = indexY;
            }

            public JsonNode? Value { get; }
            public int IndexX { get; }
            public int IndexY { get; }
        }
    }
}
