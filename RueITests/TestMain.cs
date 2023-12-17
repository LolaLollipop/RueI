using eMEC;
using System.Diagnostics;

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
    public async Task TestEMEC_Canceling()
    {
        bool wasSuccessful = false;

        UpdateTask task = new();
        task.Start(TimeSpan.FromMilliseconds(250), () => wasSuccessful = true);

        await Task.Delay(100);

        task.End();

        await Task.Delay(500);

        Assert.IsFalse(wasSuccessful);
    }

    [TestMethod]
    public async Task TestEMEC_Updating()
    {
        bool wasSuccessful = false;

        UpdateTask task = new();
        task.Start(TimeSpan.FromMilliseconds(100), () => wasSuccessful = true);

        await Task.Delay(500);

        Assert.IsTrue(wasSuccessful);
    }

    [TestMethod]
    public async Task TestEMEC_Pausing()
    {
        static async Task<long> RunMultiplePauses()
        {
            Stopwatch stopwatch = new();

            UpdateTask task = new();
            task.Start(TimeSpan.FromMilliseconds(500), stopwatch.Stop);

            stopwatch.Start();

            // paused
            await Task.Delay(250);

            task.Pause();

            await Task.Delay(250);
            // unpaused
            task.Resume();

            await Task.Delay(2000);
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }


        Task<long> trialOne = RunMultiplePauses();
        Task<long> trialTwo = RunMultiplePauses();
        Task<long> trialThree = RunMultiplePauses();

        await Task.WhenAll(trialOne, trialTwo, trialThree);

        long elapsed = (await trialOne + await trialTwo + await trialThree) / 3;

        Assert.AreEqual(750, elapsed, 50);
    }
}