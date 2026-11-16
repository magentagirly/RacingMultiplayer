using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Testmode 
{
    [Test]
    public void TestmodeSimplePasses()
    {
        Assert.IsTrue(true);
    }

    [UnityTest]
    public IEnumerator TestmodeWithEnumeratorPasses()
    {
        yield return null;
        Assert.IsTrue(true);
    }
}