using RueI.Parsing;
using RueI.Parsing.Enums;
using RueI.Parsing.Records;
using RueI.Parsing.Tags;

namespace RueITest;

[RichTextTag]
public class NotTag
{
}

[RichTextTag]
public class RealTag : NoParamsTag
{
    public override string[] Names { get; } = { "hello" };

    public override bool HandleTag(ParserContext context)
    {
        context.ResultBuilder.Append("hello from RealTag!");
        return true;
    }
}

[RichTextTag]
public class RealTagTwo : NoParamsTag
{
    public override string[] Names { get; } = { "world" };

    public override bool HandleTag(ParserContext context)
    {
        context.ResultBuilder.Append("world from RealTagTwo!");
        return true;
    }
}

[TestClass]
public class TestTags
{
    [TestMethod]
    public void TestParserContext()
    {
        ParserContext context = new();
        context.AddEndingTag<RealTag>();

        context.AddEndingTag<RealTagTwo>();
        context.RemoveEndingTag<RealTagTwo>();

        context.ApplyClosingTags();

        Assert.AreEqual("hello from RealTag!", context.ResultBuilder.ToString());
    }

    [TestMethod]
    [DataRow("A VERY VERY VERY VERY A VERY VERY VERY VERY A VERY VERY VERY VERY A VERY VERY VERY VERY A VERY VERY VERY VERY A VERY VERY VERY VERY A VERY VERY VERY VERY A VERY VERY VERY VERY A VERY VERY VERY VERY A VERY VERY VERY VERY A VERY VERY VERY VERY LONG WORLD", 121.995f)]
    [DataRow("A VERY VERY VERY VERY A VERY VERY VERY VERY A VERY VERY VERY VERY A VERY VERY VERY VERY A VERY VERY VERY VERY A VERY VERY VERY VERY A VERY VERY VERY VERY A VERY VERY VERY VERY A VERY VERY VERY VERY A VERY", 121.995f)]
    [DataRow("A VERY VERY\n\n\nVERY VERY", 121.995f)]
    [DataRow("<line-height=300%>A VERY VERY\nVERY VERY", 121.995f)]
    [DataRow("<line-height<", 0f)]
    public void TestParser(string text, float expectedOffset)
    {
        (_, float offset) = Parser.DefaultParser.Parse(text);
        Assert.AreEqual(expectedOffset, offset, 0.1f);
    }


    [TestMethod]
    [Description("Tests the measurement info parsing")]
    public void CreateMeasurementInfo_ShouldSucceed()
    {
        Assert.IsTrue(MeasurementInfo.TryParse("50.5px", out MeasurementInfo info));
        Assert.AreEqual(50.5, info.Value, 0.001);
        Assert.AreEqual(MeasurementUnit.Pixels, info.Style);
    }

    [TestMethod]
    public void TestTagBuilding()
    {
        ParserBuilder builder = new();

        builder.AddFromAssembly(typeof(TestTags).Assembly);
        Parser parser = builder.Build();

        Assert.AreEqual(2, parser.Tags.Count);
        Assert.IsTrue(parser.Tags.Any(x => x.Contains(SharedTag<RealTag>.Singleton)));
    }

    [TestMethod]
    [DataRow("\"hello  !!!         \nworld")]
    [DataRow("hello world again\"")]
    public void TestQuoteProcessing_ShouldNull(string input)
    {
        string? shouldNull = TagHelpers.ExtractFromQuotations(input);

        Assert.IsNull(shouldNull); 
    }

    [TestMethod]
    [DataRow("\"hello world - - - again\"")]
    [DataRow("hello \n\n\nworld")]
    [DataRow("")]
    public void TestQuoteProcessing_ShouldExist(string input)
    {
        string? shouldExist = TagHelpers.ExtractFromQuotations(input);

        Assert.IsNotNull(shouldExist);
    }
}
///"\"hello world - - - again\""