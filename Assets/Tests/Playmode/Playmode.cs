using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Playmode
{
    [Test]
    public void PlaymodeSimplePasses()
    {
        Assert.IsTrue(true);
    }

    [UnityTest]
    public IEnumerator PlaymodeWithEnumeratorPasses()
    {
        yield return null;
        Assert.IsTrue(true);
    }
}