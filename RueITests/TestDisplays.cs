using RueI;
using RueI.Displays;
using RueI.Elements;
using System.Diagnostics;

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
        IElemReference<SetElement> elemRef = DisplayCore.GetReference<SetElement>();
        IElemReference<SetElement> elemRefTwo = DisplayCore.GetReference<SetElement>();

        Assert.AreEqual(elemRef, elemRef);
        Assert.AreNotEqual(elemRef, elemRefTwo);

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

    [TestMethod]
    public async Task TestScheduling()
    {
        Stopwatch stopwatchOne = new();
        Stopwatch stopwatchTwo = new();
        Stopwatch stopwatchThree = new();

        MockDisplayCore core = new();

        core.Scheduler.Schedule(TimeSpan.FromMilliseconds(200), stopwatchOne.Stop);
        core.Scheduler.Schedule(TimeSpan.FromMilliseconds(300), stopwatchTwo.Stop);
        core.Scheduler.Schedule(TimeSpan.FromMilliseconds(400), stopwatchThree.Stop);

        stopwatchOne.Start();
        stopwatchTwo.Start();
        stopwatchThree.Start();

        await Task.Delay(600);

        Assert.AreEqual(300, (stopwatchOne.ElapsedMilliseconds + stopwatchTwo.ElapsedMilliseconds + stopwatchThree.ElapsedMilliseconds) / 3, 50);
    }
}