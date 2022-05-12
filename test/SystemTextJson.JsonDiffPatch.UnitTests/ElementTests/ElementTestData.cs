using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace SystemTextJson.JsonDiffPatch.UnitTests.ElementTests
{
    [SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
    public class ElementTestData
    {
        public static IEnumerable<object[]> RawTextEqual
        {
            get
            {
                yield return new object[] {Json("true"), Json("true"), true};
                yield return new object[] {Json("true"), Json("false"), false};
                yield return new object[] {Json("\"2019-11-27\""), Json("\"2019-11-27\""), true};
                yield return new object[] {Json("\"2019-11-27\""), Json("\"2019-11-27T00:00:00.000\""), false};
                yield return new object[] {Json("\"2019-11-27\""), Json("\"Shaun is a rabbit\""), false};
                yield return new object[] {Json("1"), Json("1"), true};
                yield return new object[] {Json("1"), Json("2"), false};
                yield return new object[] {Json("1.0"), Json("1"), false};
                yield return new object[] {Json("1.12e1"), Json("11.2"), false};
                yield return new object[] {Json("-1"), Json("-1"), true};
                yield return new object[] {Json("-1"), Json("-1.0"), false};
                yield return new object[] {Json("-1.1e1"), Json("-11"), false};
                yield return new object[]
                {
                    Json("\"9d423bba-b9a8-4d19-a39a-b421bed58e02\""),
                    Json("\"9d423bba-b9a8-4d19-a39a-b421bed58e02\""),
                    true
                };
                yield return new object[]
                {
                    Json("\"9d423bba-b9a8-4d19-a39a-b421bed58e02\""),
                    Json("\"b8baf656-8e97-4694-ae1a-be35e3a86db5\""),
                    false
                };
                yield return new object[]
                {
                    Json("\"9d423bba-b9a8-4d19-a39a-b421bed58e02\""),
                    Json("\"9D423BBA-B9A8-4D19-A39A-B421BED58E02\""),
                    false
                };
                yield return new object[] {Json("\"Shaun is a rabbit\""), Json("\"Shaun is a rabbit\""), true};
                yield return new object[] {Json("\"Shaun is a rabbit\""), Json("\"Shawn is a rabbit\""), false};
                yield return new object[] {Json("1"), Json("\"Shaun is a rabbit\""), false};
            }
        }
        
        public static IEnumerable<object[]> SemanticEqual
        {
            get
            {
                yield return new object[] {Json("true"), Json("true"), true};
                yield return new object[] {Json("true"), Json("false"), false};
                yield return new object[] {Json("\"2019-11-27\""), Json("\"2019-11-27\""), true};
                yield return new object[] {Json("\"2019-11-27\""), Json("\"2019-11-27T00:00:00.000\""), true};
                yield return new object[] {Json("\"2019-11-27\""), Json("\"Shaun is a rabbit\""), false};
                yield return new object[] {Json("1"), Json("1"), true};
                yield return new object[] {Json("1"), Json("2"), false};
                yield return new object[] {Json("1.0"), Json("1"), true};
                yield return new object[] {Json("1.12e1"), Json("11.2"), true};
                yield return new object[] {Json("-1"), Json("-1"), true};
                yield return new object[] {Json("-1"), Json("-1.0"), true};
                yield return new object[] {Json("-1.1e1"), Json("-11"), true};
                yield return new object[]
                {
                    Json("\"9d423bba-b9a8-4d19-a39a-b421bed58e02\""),
                    Json("\"9d423bba-b9a8-4d19-a39a-b421bed58e02\""),
                    true
                };
                yield return new object[]
                {
                    Json("\"9d423bba-b9a8-4d19-a39a-b421bed58e02\""),
                    Json("\"b8baf656-8e97-4694-ae1a-be35e3a86db5\""),
                    false
                };
                yield return new object[]
                {
                    Json("\"9d423bba-b9a8-4d19-a39a-b421bed58e02\""),
                    Json("\"9D423BBA-B9A8-4D19-A39A-B421BED58E02\""),
                    true
                };
                yield return new object[] {Json("\"Shaun is a rabbit\""), Json("\"Shaun is a rabbit\""), true};
                yield return new object[] {Json("\"Shaun is a rabbit\""), Json("\"Shawn is a rabbit\""), false};
                yield return new object[] {Json("1"), Json("\"Shaun is a rabbit\""), false};
            }
        }

        private static JsonElement Json(string jsonValue)
        {
            return JsonSerializer.Deserialize<JsonElement>($"{jsonValue}");
        }
    }
}
