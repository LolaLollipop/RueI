using RueI;
using RueI.Enums;
using RueI.Parsing;
using RueI.Parsing.Tags;
using RueI.Records;

namespace RueITest;

public class MockDisplayCore : DisplayCore
{
#nullable disable
    public MockDisplayCore() : base(null)
#nullable enable
    {
    }
}


[TestClass]
public class TestDisplayCore
{
    [TestMethod]
    public void TestDisplay()
    {
        static SetElement CreateNewElem() => new(100, "hello world");

        MockDisplayCore core = new();
        ElemReference<SetElement> elemRef = new();
        ElemReference<SetElement> elemRefTwo = new();

        SetElement elem = core.GetElementOrNew(elemRef, CreateNewElem);
        core.AddAsReference(elemRefTwo, new(500, "goodbye world"));

        Assert.IsNotNull(core.GetElement(elemRef));
        Assert.IsNotNull(core.GetElement(elemRefTwo));
        Assert.AreNotEqual(core.GetElement(elemRef), core.GetElement(elemRefTwo));
    }
}