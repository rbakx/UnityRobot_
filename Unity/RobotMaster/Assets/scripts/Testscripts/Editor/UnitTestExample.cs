using UnityEngine;
using NUnit.Framework;

[TestFixture]
public class UnitTestExample {
    
    [Test]
    public void ExampleTestPasses()
    {
        Assert.That(true);
    }

    [Test]
    public void ExampleTestFails()
    {
        Assert.That(false);
    }
}
