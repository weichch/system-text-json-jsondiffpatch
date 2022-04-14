using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace SystemTextJson.JsonDiffPatch.UnitTests.TestData
{
    public class JsonValueTestData
    {
        public static IEnumerable<object[]> ElementObjectSemanticEqual
        {
            get
            {
                yield return new object[] {Json("true"), JsonValue.Create(true), true};
                yield return new object[] {Json("false"), JsonValue.Create(false), true};
                yield return new object[] {Json("true"), JsonValue.Create(false), false};
                yield return new object[] {Json("false"), JsonValue.Create(true), false};
                yield return new object[] {Json("false"), JsonValue.Create(1), false};

                var dt = DateTime.UtcNow;
                yield return new object[] {Json($"\"{dt:O}\""), JsonValue.Create(dt), true};
                yield return new object[] {Json($"\"{dt:O}\""), JsonValue.Create(dt.AddSeconds(1)), false};
                yield return new object[] {Json($"\"{dt:O}\""), JsonValue.Create(1), false};

                var dto = DateTimeOffset.UtcNow;
                yield return new object[] {Json($"\"{dto:O}\""), JsonValue.Create(dto), true};
                yield return new object[] {Json($"\"{dto:O}\""), JsonValue.Create(dto.AddSeconds(1)), false};
                yield return new object[] {Json($"\"{dto:O}\""), JsonValue.Create(1), false};

                var guid = Guid.NewGuid();
                yield return new object[] {Json($"\"{guid:D}\""), JsonValue.Create(guid), true};
                yield return new object[] {Json($"\"{guid:D}\""), JsonValue.Create(Guid.NewGuid()), false};
                yield return new object[] {Json($"\"{guid:D}\""), JsonValue.Create(1), false};

                var bytes = new byte[] {1, 2, 3, 4};
                yield return new object[] {Json($"\"{Convert.ToBase64String(bytes)}\""), JsonValue.Create(bytes)!, true};
                yield return new object[]
                {
                    JsonValue.Create(bytes)!,
                    Json($"\"{Convert.ToBase64String(new byte[] {1, 2, 3, 5})}\""),
                    false
                };
                yield return new object[] {Json($"\"{Convert.ToBase64String(bytes)}\""), JsonValue.Create(1), false};

                yield return new object[] {Json("\"W\""), JsonValue.Create('W'), true};
                yield return new object[] {Json("\"W\""), JsonValue.Create('V'), false};
                yield return new object[] {Json("\"W\""), JsonValue.Create(1), false};

                yield return new object[] {Json("\"Shaun is a rabbit\""), JsonValue.Create("Shaun is a rabbit")!, true};
                yield return new object[] {Json("\"Shaun is a rabbit\""), JsonValue.Create("yes")!, false};
                yield return new object[] {Json("\"Shaun is a rabbit\""), JsonValue.Create(1), false};

                yield return new object[] {Json("1"), JsonValue.Create((byte) 1), true};
                yield return new object[] {Json("1.0"), JsonValue.Create((byte) 1), true};
                yield return new object[] {Json("1.1e1"), JsonValue.Create((byte) 11), true};
                yield return new object[] {Json("1"), JsonValue.Create((byte) 2), false};
                
                yield return new object[] {Json("1"), JsonValue.Create(1m), true};
                yield return new object[] {Json("1.0"), JsonValue.Create(1m), true};
                yield return new object[] {Json("1.000"), JsonValue.Create(1.00m), true};
                yield return new object[] {Json("1.1e1"), JsonValue.Create(11m), true};
                yield return new object[] {Json("1"), JsonValue.Create(2m), false};
                
                yield return new object[] {Json("1"), JsonValue.Create(1.0d), true};
                yield return new object[] {Json("1.0"), JsonValue.Create(1.0d), true};
                yield return new object[] {Json("1.12e1"), JsonValue.Create(11.2d), true};
                yield return new object[] {Json("1"), JsonValue.Create(2.0d), false};
                
                yield return new object[] {Json("1"), JsonValue.Create((short) 1), true};
                yield return new object[] {Json("1.0"), JsonValue.Create((short) 1), true};
                yield return new object[] {Json("1.1e1"), JsonValue.Create((short) 11), true};
                yield return new object[] {Json("1"), JsonValue.Create((short) 2), false};
                
                yield return new object[] {Json("1"), JsonValue.Create(1), true};
                yield return new object[] {Json("1.0"), JsonValue.Create(1), true};
                yield return new object[] {Json("1.1e1"), JsonValue.Create(11), true};
                yield return new object[] {Json("1"), JsonValue.Create(2), false};
                
                yield return new object[] {Json("1"), JsonValue.Create(1L), true};
                yield return new object[] {Json("1.0"), JsonValue.Create(1L), true};
                yield return new object[] {Json("1.1e1"), JsonValue.Create(11L), true};
                yield return new object[] {Json("1"), JsonValue.Create(2L), false};
                
                yield return new object[] {Json("1"), JsonValue.Create((sbyte) 1), true};
                yield return new object[] {Json("1.0"), JsonValue.Create((sbyte) 1), true};
                yield return new object[] {Json("1.1e1"), JsonValue.Create((sbyte) 11), true};
                yield return new object[] {Json("1"), JsonValue.Create((sbyte) 2), false};
                
                yield return new object[] {Json("1"), JsonValue.Create(1.0f), true};
                yield return new object[] {Json("1.0"), JsonValue.Create(1.0f), true};
                yield return new object[] {Json("1.12e1"), JsonValue.Create(11.2f), true};
                yield return new object[] {Json("1"), JsonValue.Create(2.0f), false};
                
                yield return new object[] {Json("1"), JsonValue.Create((ushort) 1), true};
                yield return new object[] {Json("1.0"), JsonValue.Create((ushort) 1), true};
                yield return new object[] {Json("1.1e1"), JsonValue.Create((ushort) 11), true};
                yield return new object[] {Json("1"), JsonValue.Create((ushort) 2), false};
                
                yield return new object[] {Json("1"), JsonValue.Create((uint) 1), true};
                yield return new object[] {Json("1.0"), JsonValue.Create((uint) 1), true};
                yield return new object[] {Json("1.1e1"), JsonValue.Create((uint) 11), true};
                yield return new object[] {Json("1"), JsonValue.Create((uint) 2), false};
                
                yield return new object[] {Json("1"), JsonValue.Create((ulong) 1), true};
                yield return new object[] {Json("1.0"), JsonValue.Create((ulong) 1), true};
                yield return new object[] {Json("1.1e1"), JsonValue.Create((ulong) 11), true};
                yield return new object[] {Json("1"), JsonValue.Create((ulong) 2), false};
                
                yield return new object[] {Json("1"), JsonValue.Create(new object())!, false};
            }
        }
        
        public static IEnumerable<object[]> ObjectSemanticEqual
        {
            get
            {
                yield return new object[] {JsonValue.Create(true), JsonValue.Create(true), true};
                yield return new object[] {JsonValue.Create(false), JsonValue.Create(false), true};
                yield return new object[] {JsonValue.Create(true), JsonValue.Create(false), false};
                yield return new object[] {JsonValue.Create(false), JsonValue.Create(true), false};
                yield return new object[] {JsonValue.Create(false), JsonValue.Create(1), false};

                var dt = DateTime.UtcNow;
                yield return new object[] {JsonValue.Create(dt), JsonValue.Create(dt), true};
                yield return new object[] {JsonValue.Create(dt), JsonValue.Create(dt.AddSeconds(1)), false};
                yield return new object[] {JsonValue.Create(dt), JsonValue.Create(1), false};

                var dto = DateTimeOffset.UtcNow;
                yield return new object[] {JsonValue.Create(dto), JsonValue.Create(dto), true};
                yield return new object[] {JsonValue.Create(dto), JsonValue.Create(dto.AddSeconds(1)), false};
                yield return new object[] {JsonValue.Create(dto), JsonValue.Create(1), false};

                var guid = Guid.NewGuid();
                yield return new object[] {JsonValue.Create(guid), JsonValue.Create(guid), true};
                yield return new object[] {JsonValue.Create(guid), JsonValue.Create(Guid.NewGuid()), false};
                yield return new object[] {JsonValue.Create(guid), JsonValue.Create(1), false};

                var bytes = new byte[] {1, 2, 3, 4};
                yield return new object[] {JsonValue.Create(bytes)!, JsonValue.Create(bytes)!, true};
                yield return new object[]
                {
                    JsonValue.Create(bytes)!,
                    JsonValue.Create(new byte[] {1, 2, 3, 5})!,
                    false
                };
                yield return new object[] {JsonValue.Create(bytes)!, JsonValue.Create(1), false};

                yield return new object[] {JsonValue.Create('W'), JsonValue.Create('W'), true};
                yield return new object[] {JsonValue.Create('W'), JsonValue.Create('V'), false};
                yield return new object[] {JsonValue.Create('W'), JsonValue.Create(1), false};

                yield return new object[]
                {
                    JsonValue.Create("Shaun is a rabbit")!,
                    JsonValue.Create("Shaun is a rabbit")!,
                    true
                };
                yield return new object[] {JsonValue.Create("Shaun is a rabbit")!, JsonValue.Create("yes")!, false};
                yield return new object[] {JsonValue.Create("Shaun is a rabbit")!, JsonValue.Create(1), false};

                yield return new object[] {JsonValue.Create(1), JsonValue.Create((byte) 1), true};
                yield return new object[] {JsonValue.Create(1.0), JsonValue.Create((byte) 1), true};
                yield return new object[] {JsonValue.Create(11), JsonValue.Create((byte) 11), true};
                yield return new object[] {JsonValue.Create(1), JsonValue.Create((byte) 2), false};
                
                yield return new object[] {JsonValue.Create(1), JsonValue.Create(1m), true};
                yield return new object[] {JsonValue.Create(1.0d), JsonValue.Create(1m), true};
                yield return new object[] {JsonValue.Create(1.000m), JsonValue.Create(1.00m), true};
                yield return new object[] {JsonValue.Create(11L), JsonValue.Create(11m), true};
                yield return new object[] {JsonValue.Create(1), JsonValue.Create(2m), false};
                
                yield return new object[] {JsonValue.Create(1), JsonValue.Create(1.0d), true};
                yield return new object[] {JsonValue.Create(1.0d), JsonValue.Create(1.0d), true};
                yield return new object[] {JsonValue.Create(11.2f), JsonValue.Create(11.2d), true};
                yield return new object[] {JsonValue.Create(1), JsonValue.Create(2.0d), false};
                
                yield return new object[] {JsonValue.Create(1), JsonValue.Create((short) 1), true};
                yield return new object[] {JsonValue.Create(1.0d), JsonValue.Create((short) 1), true};
                yield return new object[] {JsonValue.Create(11L), JsonValue.Create((short) 11), true};
                yield return new object[] {JsonValue.Create(1), JsonValue.Create((short) 2), false};
                
                yield return new object[] {JsonValue.Create(1), JsonValue.Create(1), true};
                yield return new object[] {JsonValue.Create(1.0d), JsonValue.Create(1), true};
                yield return new object[] {JsonValue.Create(11L), JsonValue.Create(11), true};
                yield return new object[] {JsonValue.Create(1), JsonValue.Create(2), false};
                
                yield return new object[] {JsonValue.Create(1), JsonValue.Create(1L), true};
                yield return new object[] {JsonValue.Create(1.0d), JsonValue.Create(1L), true};
                yield return new object[] {JsonValue.Create(11L), JsonValue.Create(11L), true};
                yield return new object[] {JsonValue.Create(1), JsonValue.Create(2L), false};
                
                yield return new object[] {JsonValue.Create(1), JsonValue.Create((sbyte) 1), true};
                yield return new object[] {JsonValue.Create(1.0d), JsonValue.Create((sbyte) 1), true};
                yield return new object[] {JsonValue.Create(11L), JsonValue.Create((sbyte) 11), true};
                yield return new object[] {JsonValue.Create(1), JsonValue.Create((sbyte) 2), false};
                
                yield return new object[] {JsonValue.Create(1), JsonValue.Create(1.0f), true};
                yield return new object[] {JsonValue.Create(1.0d), JsonValue.Create(1.0f), true};
                yield return new object[] {JsonValue.Create(11.2f), JsonValue.Create(11.2f), true};
                yield return new object[] {JsonValue.Create(1), JsonValue.Create(2.0f), false};
                
                yield return new object[] {JsonValue.Create(1), JsonValue.Create((ushort) 1), true};
                yield return new object[] {JsonValue.Create(1.0d), JsonValue.Create((ushort) 1), true};
                yield return new object[] {JsonValue.Create(11L), JsonValue.Create((ushort) 11), true};
                yield return new object[] {JsonValue.Create(1), JsonValue.Create((ushort) 2), false};
                
                yield return new object[] {JsonValue.Create(1), JsonValue.Create((uint) 1), true};
                yield return new object[] {JsonValue.Create(1.0d), JsonValue.Create((uint) 1), true};
                yield return new object[] {JsonValue.Create(11L), JsonValue.Create((uint) 11), true};
                yield return new object[] {JsonValue.Create(1), JsonValue.Create((uint) 2), false};
                
                yield return new object[] {JsonValue.Create(1), JsonValue.Create((ulong) 1), true};
                yield return new object[] {JsonValue.Create(1.0d), JsonValue.Create((ulong) 1), true};
                yield return new object[] {JsonValue.Create(11L), JsonValue.Create((ulong) 11), true};
                yield return new object[] {JsonValue.Create(1), JsonValue.Create((ulong) 2), false};
                
                yield return new object[] {JsonValue.Create(1), JsonValue.Create(new object())!, false};
            }
        }
        
        private static JsonValue Json(string jsonValue)
        {
            return JsonNode.Parse($"[{jsonValue}]")!.AsArray().First()!.AsValue();
        }
    }
}