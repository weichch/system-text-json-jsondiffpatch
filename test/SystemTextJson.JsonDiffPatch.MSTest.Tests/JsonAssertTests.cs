using System.Text.Json.JsonDiffPatch.MsTest;
using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemTextJson.JsonDiffPatch.MsTest.Tests
{
    [TestClass]
    public class JsonAssertTests
    {
        [TestMethod]
        public void AreEqual_String()
        {
            var json1 = "{\"foo\":\"bar\",\"baz\":\"qux\"}";
            var json2 = "{\"baz\":\"qux\",\"foo\":\"bar\"}";

            JsonAssert.AreEqual(json1, json2);
        }
        
        [TestMethod]
        public void AreEqual_JsonNode()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"baz\":\"qux\",\"foo\":\"bar\"}");

            JsonAssert.AreEqual(json1, json2);
        }

        [TestMethod]
        public void AreEqual_ExtensionMethod()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"baz\":\"qux\",\"foo\":\"bar\"}");

            Assert.That.JsonAreEqual(json1, json2);
        }

        [TestMethod]
        public void AreEqual_Nulls()
        {
            JsonNode? json1 = null;
            JsonNode? json2 = null;

            JsonAssert.AreEqual(json1, json2);
        }

        [TestMethod]
        public void AreEqual_FailWithMessage()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"baz\"}");

            var error = Assert.ThrowsException<AssertFailedException>(
                () => Assert.That.JsonAreEqual(json1, json2));

            StringAssert.Contains(error.Message,
                "JsonAssert.AreEqual() failure: The specified two JSON objects are not equal.");
            StringAssert.Contains(error.Message, "Expected:");
            StringAssert.Contains(error.Message, "Actual:");
            StringAssert.Contains(error.Message, "Diff:");
        }

        [TestMethod]
        public void AreNotEqual_String()
        {
            var json1 = "{\"foo\":\"bar\",\"baz\":\"qux\"}";
            var json2 = "{\"foo\":\"baz\"}";

            JsonAssert.AreNotEqual(json1, json2);
        }
        
        [TestMethod]
        public void AreNotEqual_JsonNode()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"baz\"}");

            JsonAssert.AreNotEqual(json1, json2);
        }

        [TestMethod]
        public void AreNotEqual_ExtensionMethod()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"baz\"}");

            Assert.That.JsonAreNotEqual(json1, json2);
        }

        [TestMethod]
        public void AreNotEqual_Nulls()
        {
            JsonNode? json1 = null;
            JsonNode? json2 = null;

            var error = Assert.ThrowsException<AssertFailedException>(
                () => Assert.That.JsonAreNotEqual(json1, json2));

            Assert.IsNotNull(error);
        }

        [TestMethod]
        public void AreNotEqual_FailWithMessage()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"baz\":\"qux\",\"foo\":\"bar\"}");

            var error = Assert.ThrowsException<AssertFailedException>(
                () => Assert.That.JsonAreNotEqual(json1, json2));

            StringAssert.Contains(error.Message,
                "JsonAssert.AreNotEqual() failure: The specified two JSON objects are equal.");
        }
    }
}