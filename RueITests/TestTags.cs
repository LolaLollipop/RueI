using RueI.Parsing.Tags;

namespace RueITest
{
    [TestClass]
    public class TestTags
    {
        [TestMethod]
        public void TestUtility()
        {
            string? shouldExist = TagHelpers.ExtractFromQuotations("hello world");
            string? shouldExist2 = TagHelpers.ExtractFromQuotations("\"hello world - - - again\"");

            string? shouldNull = TagHelpers.ExtractFromQuotations("\"hello  !!!         \nworld");
            string? shouldNull2 = TagHelpers.ExtractFromQuotations("hello world again\"");

            Assert.IsNotNull(shouldExist);
            Assert.IsNotNull(shouldExist2);

            Assert.IsNotNull(shouldNull); 
            Assert.IsNull(shouldNull2);
        }
    }
}