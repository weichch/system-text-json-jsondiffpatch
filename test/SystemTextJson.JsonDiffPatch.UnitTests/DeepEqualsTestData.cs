using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SystemTextJson.JsonDiffPatch.UnitTests
{
    [SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
    public static class DeepEqualsTestData
    {
        public static IEnumerable<object?[]> RawTextEqual(Func<string?, object?> jsonizer)
        {
            yield return new[] {jsonizer("true"), jsonizer("true"), true};
            yield return new[] {jsonizer("true"), jsonizer("false"), false};
            yield return new[] {jsonizer("\"2019-11-27\""), jsonizer("\"2019-11-27\""), true};
            yield return new[] {jsonizer("\"2019-11-27\""), jsonizer("\"2019-11-27T00:00:00.000\""), false};
            yield return new[] {jsonizer("\"2019-11-27\""), jsonizer("\"Shaun is a rabbit\""), false};
            yield return new[] {jsonizer("1"), jsonizer("1"), true};
            yield return new[] {jsonizer("1"), jsonizer("2"), false};
            yield return new[] {jsonizer("1.0"), jsonizer("1"), false};
            yield return new[] {jsonizer("1.12e1"), jsonizer("11.2"), false};
            yield return new[] {jsonizer("-1"), jsonizer("-1"), true};
            yield return new[] {jsonizer("-1"), jsonizer("-1.0"), false};
            yield return new[] {jsonizer("-1.1e1"), jsonizer("-11"), false};
            yield return new[]
            {
                jsonizer("\"9d423bba-b9a8-4d19-a39a-b421bed58e02\""),
                jsonizer("\"9d423bba-b9a8-4d19-a39a-b421bed58e02\""),
                true
            };
            yield return new[]
            {
                jsonizer("\"9d423bba-b9a8-4d19-a39a-b421bed58e02\""),
                jsonizer("\"b8baf656-8e97-4694-ae1a-be35e3a86db5\""),
                false
            };
            yield return new[]
            {
                jsonizer("\"9d423bba-b9a8-4d19-a39a-b421bed58e02\""),
                jsonizer("\"9D423BBA-B9A8-4D19-A39A-B421BED58E02\""),
                false
            };
            yield return new[] {jsonizer("\"Shaun is a rabbit\""), jsonizer("\"Shaun is a rabbit\""), true};
            yield return new[] {jsonizer("\"Shaun is a rabbit\""), jsonizer("\"Shawn is a rabbit\""), false};
            yield return new[] {jsonizer("1"), jsonizer("\"Shaun is a rabbit\""), false};
        }

        public static IEnumerable<object?[]> SemanticEqual(Func<string?, object?> jsonizer)
        {
            yield return new[] {jsonizer("true"), jsonizer("true"), true};
            yield return new[] {jsonizer("true"), jsonizer("false"), false};
            yield return new[] {jsonizer("\"2019-11-27\""), jsonizer("\"2019-11-27\""), true};
            yield return new[] {jsonizer("\"2019-11-27\""), jsonizer("\"2019-11-27T00:00:00.000\""), true};
            yield return new[] {jsonizer("\"2019-11-27\""), jsonizer("\"Shaun is a rabbit\""), false};
            yield return new[] {jsonizer("1"), jsonizer("1"), true};
            yield return new[] {jsonizer("1"), jsonizer("2"), false};
            yield return new[] {jsonizer("1.0"), jsonizer("1"), true};
            yield return new[] {jsonizer("1.12e1"), jsonizer("11.2"), true};
            yield return new[] {jsonizer("-1"), jsonizer("-1"), true};
            yield return new[] {jsonizer("-1"), jsonizer("-1.0"), true};
            yield return new[] {jsonizer("-1.1e1"), jsonizer("-11"), true};
            yield return new[]
            {
                jsonizer("\"9d423bba-b9a8-4d19-a39a-b421bed58e02\""),
                jsonizer("\"9d423bba-b9a8-4d19-a39a-b421bed58e02\""),
                true
            };
            yield return new[]
            {
                jsonizer("\"9d423bba-b9a8-4d19-a39a-b421bed58e02\""),
                jsonizer("\"b8baf656-8e97-4694-ae1a-be35e3a86db5\""),
                false
            };
            yield return new[]
            {
                jsonizer("\"9d423bba-b9a8-4d19-a39a-b421bed58e02\""),
                jsonizer("\"9D423BBA-B9A8-4D19-A39A-B421BED58E02\""),
                true
            };
            yield return new[] {jsonizer("\"Shaun is a rabbit\""), jsonizer("\"Shaun is a rabbit\""), true};
            yield return new[] {jsonizer("\"Shaun is a rabbit\""), jsonizer("\"Shawn is a rabbit\""), false};
            yield return new[] {jsonizer("1"), jsonizer("\"Shaun is a rabbit\""), false};
        }
    }
}
