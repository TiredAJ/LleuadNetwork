using Common.Utils;

using NUnit.Framework;

namespace Common.Tests.Extensions;

[TestFixture]
public class ExtensionsTest
{
    private string ValA;
    private string ValB;
    private byte[] Expected;
    
    [SetUp]
    protected void Setup() {
        ValA = "cakes";
        ValB = "cheese";
        Expected = [198, 201, 208, 202, 230];
    }
    
    [Test]
    public void AddValues() {
        byte[] Result = ValA.AddValue(ValB);
        
        Assert.That(Result, Is.EqualTo(Expected));
    }

    [Test]
    [TestCase(10000L, "9.8KB")]
    [TestCase(650000L, "634.8KB")]
    [TestCase(5L, "5B")]
    [TestCase(954310000L, "910.1MB")]
    [TestCase(234395010000L, "218.3GB")]
    public void FileSizes(long _Size, string _Expected) {
        string R = _Size.ToFileSize();

        Console.WriteLine(R);
        
        Assert.That(R, Is.EqualTo(_Expected));
    }
}
