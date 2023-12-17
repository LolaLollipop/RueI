using RueI.Extensions;

namespace RueITest;

[TestClass]
public class TestExtensions
{
    [TestMethod]
    public void TestIComparable()
    {
        int seven = 7;

        Assert.AreEqual(10, seven.Max(10));
        Assert.AreEqual(7, seven.Max(5));

        Assert.AreEqual(7, seven.MaxIf(true, 49));
        Assert.AreEqual(49, seven.MaxIf(false, 49));
    }


    [TestMethod]
    public void TestICollection()
    {
        List<int> list = new();

        list.Add(1, 2, 3);

        Assert.AreEqual(3, list.Count);
    }

    [TestMethod]
    public void TestUniversal()
    {
        List<string> list = new();

        "hello".AddTo(list);
        "world".AddTo(list);

        Assert.AreEqual(2, list.Count);
    }

    [TestMethod]
    public void TestUnion()
    {
        
    }
}
///"\"hello world - - - again\""