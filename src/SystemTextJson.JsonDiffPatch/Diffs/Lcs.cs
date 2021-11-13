using System.Buffers;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace System.Text.Json.Diffs
{
    internal readonly struct Lcs
    {
        private const int Equal = 1;
        private const int DeepEqual = 2;

        private readonly List<LcsEntry>? _entries;

        public Lcs(List<LcsEntry> entries)
        {
            _entries = entries;
        }

        public static Lcs Get(Span<JsonNode?> x,  Span<JsonNode?> y, ArrayItemMatch match)
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
            var matchMatrix = matchMatrixRented.AsSpan(0, matrixLength);

            // Initializes the matrix
            matrix.Fill(0);
            matchMatrix.Fill(0);

            try
            {
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
                        if (match(x[i - 1], i - 1, y[j - 1], j - 1, out var deepEqual))
                        {
                            matrix[i * n + j] = 1 + matrix[(i - 1) * n + (j - 1)];
                            matchMatrix[i * n + j] = deepEqual ? DeepEqual : Equal;
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

                var entries = new List<LcsEntry>(matrix[matrixLength - 1]);
                for (int i = m - 1, j = n - 1; i > 0 && j > 0;)
                {
                    if (matchMatrix[i * n + j]>0)
                    {
                        // X[i - 1] == Y [j - 1]
                        entries.Insert(0, new(x[i - 1], i - 1, j - 1));
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
                            // Move to left
                            j--;
                        }
                    }
                }

                return new Lcs(entries);
            }
            finally
            {
                ArrayPool<int>.Shared.Return(matrixRented);
                ArrayPool<int>.Shared.Return(matchMatrixRented);
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
