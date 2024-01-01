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
            new(500, "he\nllo world")
            {
                ZIndex = 5
            },
            new(200, "he\nllo world")
            {
                ZIndex = 5
            },
        };

        List<SetElement> elementsTwo = new()
        {
            new(500, "he\nllo world")
            {
                ZIndex = 5,
                Options = RueI.Elements.Enums.ElementOptions.PreserveSpacing
            },
            new(200, "he\nllo world")
            {
                ZIndex = 5,
                Options = RueI.Elements.Enums.ElementOptions.PreserveSpacing
            },
        };

        Console.WriteLine(ElemCombiner.Combine(elements).Replace("\n", "<br>"));
        Console.WriteLine(ElemCombiner.Combine(elementsTwo).Replace("\n", "<br>"));
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
        const double TIMEONE = 200;
        const double TIMETWO = 300;
        const double TIMETHREE = 400;
        const double AVERAGE = (TIMEONE + TIMETWO + TIMETHREE) / 3;

        const double DELTA = 50;

        Stopwatch stopwatchOne = new();
        Stopwatch stopwatchTwo = new();
        Stopwatch stopwatchThree = new();

        MockDisplayCore core = new();

        core.Scheduler.Schedule(TimeSpan.FromMilliseconds(TIMEONE), stopwatchOne.Stop);
        core.Scheduler.Schedule(TimeSpan.FromMilliseconds(TIMETWO), stopwatchTwo.Stop);
        core.Scheduler.Schedule(TimeSpan.FromMilliseconds(TIMETHREE), stopwatchThree.Stop);

        stopwatchOne.Start();
        stopwatchTwo.Start();
        stopwatchThree.Start();

        await Task.Delay(600);

        Assert.AreEqual(AVERAGE, (stopwatchOne.ElapsedMilliseconds + stopwatchTwo.ElapsedMilliseconds + stopwatchThree.ElapsedMilliseconds) / 3, DELTA);
    }

    [TestMethod]
    public async Task TestScheduling_2()
    {
        const double TIMEONE = 200;
        const double TIMETWO = 1500;
        const double TIMETHREE = 2500;

        const double DELTA = 75;

        Stopwatch stopwatchOne = new();
        Stopwatch stopwatchTwo = new();
        Stopwatch stopwatchThree = new();

        MockDisplayCore core = new();

        core.Scheduler.Schedule(TimeSpan.FromMilliseconds(TIMEONE), stopwatchOne.Stop);
        core.Scheduler.Schedule(TimeSpan.FromMilliseconds(TIMETWO), stopwatchTwo.Stop);
        core.Scheduler.Schedule(TimeSpan.FromMilliseconds(TIMETHREE), stopwatchThree.Stop);

        stopwatchOne.Start();
        stopwatchTwo.Start();
        stopwatchThree.Start();

        await Task.Delay(4000);

        Assert.AreEqual(TIMEONE, stopwatchOne.ElapsedMilliseconds, DELTA);
        Assert.AreEqual(TIMETWO, stopwatchTwo.ElapsedMilliseconds, DELTA);
        Assert.AreEqual(TIMETHREE, stopwatchThree.ElapsedMilliseconds, DELTA);
    }
}