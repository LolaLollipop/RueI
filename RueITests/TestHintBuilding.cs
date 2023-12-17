using RueI.Extensions.HintBuilding;
using System.Drawing;

namespace RueITest;

[TestClass]
public class TestHintBuilding
{
    [TestMethod]
    public void TestConvertColor()
    {
        Color color = Color.FromArgb(66, 135, 245);
        Color transparentColor = Color.FromArgb(50, 66, 135, 245);

        Assert.AreEqual("#4287F5", HintBuilding.ConvertToHex(color));
        Assert.AreEqual("#4287F532", HintBuilding.ConvertToHex(transparentColor));
    }
}
///"\"hello world - - - again\""