﻿using System;
using System.Text.Json.JsonDiffPatch;
using System.Text.Json.JsonDiffPatch.Diffs.Formatters;
using System.Text.Json.Nodes;
using Xunit;

namespace SystemTextJson.JsonDiffPatch.UnitTests.FormatterTests
{
    public class JsonPatchDeltaFormatterTests
    {
        // Example test cases: https://datatracker.ietf.org/doc/html/rfc6902#appendix-A
        
        [Fact]
        public void IdenticalObjects_EmptyPatch()
        {
            var left = JsonNode.Parse("{\"foo\":\"bar\"}");
            var right = JsonNode.Parse("{\"foo\":\"bar\"}");

            var diff = left.Diff(right, new JsonPatchDeltaFormatter());

            Assert.Equal("[]", diff!.ToJsonString());
        }
        
        [Fact]
        public void Add_ObjectMember()
        {
            var left = JsonNode.Parse("{\"foo\":\"bar\"}");
            var right = JsonNode.Parse("{\"baz\":\"qux\",\"foo\":\"bar\"}");

            var diff = left.Diff(right, new JsonPatchDeltaFormatter());

            Assert.Equal("[{\"op\":\"add\",\"path\":\"/baz\",\"value\":\"qux\"}]", diff!.ToJsonString());
        }
        
        [Fact]
        public void Add_ArrayElement()
        {
            var left = JsonNode.Parse("{\"foo\":[\"bar\",\"baz\"]}");
            var right = JsonNode.Parse("{\"foo\":[\"bar\",\"qux\",\"baz\"]}");

            var diff = left.Diff(right, new JsonPatchDeltaFormatter());

            Assert.Equal("[{\"op\":\"add\",\"path\":\"/foo/1\",\"value\":\"qux\"}]", diff!.ToJsonString());
        }
        
        [Fact]
        public void Add_NestedObjectMember()
        {
            var left = JsonNode.Parse("{\"foo\":\"bar\"}");
            var right = JsonNode.Parse("{\"foo\":\"bar\",\"child\":{\"grandchild\":{}}}");

            var diff = left.Diff(right, new JsonPatchDeltaFormatter());

            Assert.Equal("[{\"op\":\"add\",\"path\":\"/child\",\"value\":{\"grandchild\":{}}}]", diff!.ToJsonString());
        }
        
        [Fact]
        public void Add_NestedArrayElement()
        {
            var left = JsonNode.Parse("{\"foo\":[\"bar\"]}");
            var right = JsonNode.Parse("{\"foo\":[\"bar\",[\"abc\",\"def\"]]}");

            var diff = left.Diff(right, new JsonPatchDeltaFormatter());

            Assert.Equal("[{\"op\":\"add\",\"path\":\"/foo/1\",\"value\":[\"abc\",\"def\"]}]", diff!.ToJsonString());
        }
        
        [Fact]
        public void Remove_ObjectMember()
        {
            var left = JsonNode.Parse("{\"baz\":\"qux\",\"foo\":\"bar\"}");
            var right = JsonNode.Parse("{\"foo\":\"bar\"}");

            var diff = left.Diff(right, new JsonPatchDeltaFormatter());

            Assert.Equal("[{\"op\":\"remove\",\"path\":\"/baz\"}]", diff!.ToJsonString());
        }
        
        [Fact]
        public void Remove_ArrayElement()
        {
            var left = JsonNode.Parse("{\"foo\":[\"bar\",\"qux\",\"baz\"]}");
            var right = JsonNode.Parse("{\"foo\":[\"bar\",\"baz\"]}");

            var diff = left.Diff(right, new JsonPatchDeltaFormatter());

            Assert.Equal("[{\"op\":\"remove\",\"path\":\"/foo/1\"}]", diff!.ToJsonString());
        }

        [Fact]
        public void Remove_OperationsOrder()
        {
            var left = JsonNode.Parse("[1,2]");
            var right = JsonNode.Parse("[]");

            var diff = left.Diff(right, new JsonPatchDeltaFormatter());

            Assert.Equal(
                "[{\"op\":\"remove\",\"path\":\"/1\"},{\"op\":\"remove\",\"path\":\"/0\"}]",
                diff!.ToJsonString());
        }

        [Fact]
        public void Replace_String()
        {
            var left = JsonNode.Parse("{\"baz\":\"qux\",\"foo\":\"bar\"}");
            var right = JsonNode.Parse("{\"baz\":\"boo\",\"foo\":\"bar\"}");

            var diff = left.Diff(right, new JsonPatchDeltaFormatter());

            Assert.Equal("[{\"op\":\"replace\",\"path\":\"/baz\",\"value\":\"boo\"}]", diff!.ToJsonString());
        }

        [Fact]
        public void Replace_OperationsOrder()
        {
            var left = JsonNode.Parse("[1,2]");
            var right = JsonNode.Parse("[3,4]");

            var diff = left.Diff(right, new JsonPatchDeltaFormatter());

            Assert.Equal(
                "[{\"op\":\"remove\",\"path\":\"/1\"},{\"op\":\"remove\",\"path\":\"/0\"},{\"op\":\"add\",\"path\":\"/0\",\"value\":3},{\"op\":\"add\",\"path\":\"/1\",\"value\":4}]",
                diff!.ToJsonString());
        }

        [Fact]
        public void Move_ArrayElement()
        {
            var left = JsonNode.Parse("{\"foo\":[\"all\",\"grass\",\"cows\",\"eat\"]}");
            var right = JsonNode.Parse("{\"foo\":[\"all\",\"cows\",\"eat\",\"grass\"]}");

            var diff = left.Diff(right, new JsonPatchDeltaFormatter());

            Assert.Equal(
                "[{\"op\":\"remove\",\"path\":\"/foo/1\"},{\"op\":\"add\",\"path\":\"/foo/3\",\"value\":\"grass\"}]",
                diff!.ToJsonString());
        }

        [Fact]
        public void Text_ThrowsNotSupportedException()
        {
            var left = JsonNode.Parse("{\"foo\":\"bar\"}");
            var right = JsonNode.Parse("{\"foo\":\"boo\"}");

            Assert.Throws<NotSupportedException>(() => left.Diff(right,
                new JsonPatchDeltaFormatter(),
                new JsonDiffOptions
                {
                    TextDiffMinLength = 1
                }));
        }
        
        [Fact]
        public void JsonPointer_EscapeSpecialChar()
        {
            var left = JsonNode.Parse("{\"data\":{\"/\":{\"~1\":1}}}");
            var right = JsonNode.Parse("{\"data\":{\"/\":{\"~1\":2}}}");

            var diff = left.Diff(right, new JsonPatchDeltaFormatter());

            Assert.Equal("[{\"op\":\"replace\",\"path\":\"/data/~1/~01\",\"value\":2}]", diff!.ToJsonString());
        }
    }
}
