using RueI.Extensions;
using RueI.Extensions.HintBuilding;
using System.Drawing;

namespace RueITest;

[TestClass]
public class TestExtensions
{
    [TestMethod]
    public void TestMax()
    {
        int maxOne = Math.Max(5, 3);
        int maxTwo = 5.Max(3);
        Assert.AreEqual(maxOne, maxTwo);
    }
}
///"\"hello world - - - again\""