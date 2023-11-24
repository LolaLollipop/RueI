using RueI;
using RueI.Displays;

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
    public void TestReferences()
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

    [TestMethod]
    public void TestCombining()
    {
        List<SetElement> elements = new()
        {
            new(500, "hello world")
            {
                ZIndex = 5
            },
            new(200, "goodbye world")
            {
                ZIndex = 10
            }
        };

        ElemCombiner.Combine(elements);
    }

    [TestMethod]
    public void TestScreens()
    {
        MockDisplayCore core = new();

        ScreenDisplay display = new(core, new());
        Screen screen = new(display);

        SetElement element = new(500, "hello world");
        SetElement anotherElement = new(500, "hello next world");
    }
}