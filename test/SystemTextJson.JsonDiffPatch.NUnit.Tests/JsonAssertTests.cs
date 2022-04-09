using System.Text.Json.JsonDiffPatch.Nunit;
using System.Text.Json.Nodes;
using NUnit.Framework;

namespace SystemTextJson.JsonDiffPatch.NUnit.Tests
{
    [TestFixture]
    public class JsonAssertTests
    {
        [Test]
        public void AreEqual_String()
        {
            var json1 = "{\"foo\":\"bar\",\"baz\":\"qux\"}";
            var json2 = "{\"baz\":\"qux\",\"foo\":\"bar\"}";

            JsonAssert.AreEqual(json1, json2);
        }

        [Test]
        public void AreEqual_JsonNode()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"baz\":\"qux\",\"foo\":\"bar\"}");

            JsonAssert.AreEqual(json1, json2);
        }

        [Test]
        public void AreEqual_Nulls()
        {
            JsonNode? json1 = null;
            JsonNode? json2 = null;
        
            JsonAssert.AreEqual(json1, json2);
        }
        
        [Test]
        public void AreEqual_FailWithMessage()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"baz\"}");

            var error = Assert.Throws<AssertionException>(
                () => JsonAssert.AreEqual(json1, json2));

            StringAssert.Contains("JsonAssert.AreEqual() failure.", error!.Message);
        }

        [Test]
        public void AreEqual_FailWithDefaultOutput()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"baz\"}");

            var error = Assert.Throws<AssertionException>(
                () => JsonAssert.AreEqual(json1, json2, true));

            StringAssert.Contains("JsonAssert.AreEqual() failure.", error!.Message);
            StringAssert.Contains("Expected:", error.Message);
            StringAssert.Contains("Actual:", error.Message);
            StringAssert.Contains("Delta:", error.Message);
        }

        [Test]
        public void AreEqual_FailWithCustomOutput()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"baz\"}");

            var error = Assert.Throws<AssertionException>(() => JsonAssert.AreEqual(json1,
                json2, _ => "Custom message"));

            StringAssert.Contains("JsonAssert.AreEqual() failure.", error!.Message);
            StringAssert.Contains("Custom message", error.Message);
        }

        [Test]
        public void AreNotEqual_String()
        {
            var json1 = "{\"foo\":\"bar\",\"baz\":\"qux\"}";
            var json2 = "{\"foo\":\"baz\"}";
        
            JsonAssert.AreNotEqual(json1, json2);
        }
        
        [Test]
        public void AreNotEqual_JsonNode()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"baz\"}");
        
            JsonAssert.AreNotEqual(json1, json2);
        }

        [Test]
        public void AreNotEqual_Nulls()
        {
            JsonNode? json1 = null;
            JsonNode? json2 = null;
        
            var error = Assert.Throws<AssertionException>(
                () => JsonAssert.AreNotEqual(json1, json2));
        
            Assert.IsNotNull(error);
        }

        [Test]
        public void AreNotEqual_FailWithMessage()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"baz\":\"qux\",\"foo\":\"bar\"}");

            var error = Assert.Throws<AssertionException>(
                () => JsonAssert.AreNotEqual(json1, json2));

            StringAssert.Contains("JsonAssert.AreNotEqual() failure.", error!.Message);
        }
    }
}