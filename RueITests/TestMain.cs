using eMEC;
using RueI;

namespace RueITest;

[TestClass]
public class TestGeneral
{
    [TestMethod]
    public void TestMain()
    {
        RueI.Main.EnsureInit();
    }

    [TestMethod]
    public async Task TestEMEC()
    {
        bool wasSuccessful = false;

        UpdateTask task = new();
        task.Start(TimeSpan.FromMilliseconds(100), () => wasSuccessful = true);

        await Task.Delay(500);

        Assert.IsTrue(wasSuccessful);
    }
}