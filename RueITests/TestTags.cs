using RueI;
using RueI.Parsing;
using RueI.Parsing.Tags;
using System.Linq;

namespace RueITest;

[RichTextTag]
public class NotTag
{
}

[RichTextTag]
public class RealTag : NoParamsTag
{
    public override string[] Names { get; } = { "hello" };

    public override bool HandleTag(ParserContext context) => throw new NotImplementedException();
}

[TestClass]
public class TestTags
{
    [TestMethod]
    public void TestTagBuilding()
    {
        ParserBuilder builder = new();

        builder.AddFromAssembly(typeof(TestTags).Assembly);
        Parser parser = builder.Build();

        Assert.AreEqual(1, parser.Tags.Count);
        Assert.IsTrue(parser.Tags.Values.Any(x => x.Contains(SharedTag<RealTag>.Singleton)));
    }

    [TestMethod]
    [DataRow("\"hello  !!!         \nworld")]
    [DataRow("hello world again\"")]
    public void TestQuoteFailures(string input)
    {
        string? shouldNull = TagHelpers.ExtractFromQuotations(input);

        Assert.IsNull(shouldNull); 
    }

    [TestMethod]
    [DataRow("\"hello world - - - again\"")]
    [DataRow("hello \n\n\nworld")]
    [DataRow("\"y")]
    public void TestQuoteSuccess(string input)
    {
        string? shouldExist = TagHelpers.ExtractFromQuotations(input);

        Assert.IsNotNull(shouldExist);
    }
}
///"\"hello world - - - again\""