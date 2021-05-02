using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TCTestClass : TypedNumericalComparable<char>
{
    public TCTestClass(char c, float i) : base(c, i) { }
}

public class ALTestClass : ALSet<TCTestClass, char> { }

public class ALSetTest
{
    [Test]
    public void HasAtLeastTest1()
    {
        ALTestClass set1 = new ALTestClass();
        set1.SetElements.Add(new TCTestClass('a', 1));
        set1.SetElements.Add(new TCTestClass('b', 3));
        set1.SetElements.Add(new TCTestClass('c', 7));

        ALTestClass set2 = new ALTestClass();
        set2.SetElements.Add(new TCTestClass('a', 0));
        set2.SetElements.Add(new TCTestClass('b', 2));
        set2.SetElements.Add(new TCTestClass('c', 5));

        Assert.AreEqual(set1.HasAtLeast(set2), true);
    }

    [Test]
    public void HasAtLeastTest2()
    {
        ALTestClass set1 = new ALTestClass();
        set1.SetElements.Add(new TCTestClass('a', 1));
        set1.SetElements.Add(new TCTestClass('b', 3));
        set1.SetElements.Add(new TCTestClass('c', 7));

        ALTestClass set3 = new ALTestClass();
        set3.SetElements.Add(new TCTestClass('a', 1));
        set3.SetElements.Add(new TCTestClass('b', 2));
        
        Assert.AreEqual(set1.HasAtLeast(set3), true);
    }

    [Test]
    public void HasAtLeastTest3()
    {
        ALTestClass set1 = new ALTestClass();
        set1.SetElements.Add(new TCTestClass('a', 1));
        set1.SetElements.Add(new TCTestClass('b', 3));
        set1.SetElements.Add(new TCTestClass('c', 7));

        ALTestClass set2 = new ALTestClass();
        set2.SetElements.Add(new TCTestClass('a', 0));
        set2.SetElements.Add(new TCTestClass('b', 2));
        set2.SetElements.Add(new TCTestClass('c', 5));
        
        Assert.AreEqual(set2.HasAtLeast(set1), false);
    }

    [Test]
    public void HasAtLeastTest4()
    {
        ALTestClass set1 = new ALTestClass();
        set1.SetElements.Add(new TCTestClass('a', 1));
        set1.SetElements.Add(new TCTestClass('b', 3));
        set1.SetElements.Add(new TCTestClass('c', 7));

        ALTestClass set3 = new ALTestClass();
        set3.SetElements.Add(new TCTestClass('a', 1));
        set3.SetElements.Add(new TCTestClass('b', 2));
        
        Assert.AreEqual(set3.HasAtLeast(set1), false);
    }

    [Test]
    public void HasAtLeastTest5()
    {
        ALTestClass set2 = new ALTestClass();
        set2.SetElements.Add(new TCTestClass('a', 0));
        set2.SetElements.Add(new TCTestClass('b', 2));
        set2.SetElements.Add(new TCTestClass('c', 5));

        ALTestClass set3 = new ALTestClass();
        set3.SetElements.Add(new TCTestClass('a', 1));
        set3.SetElements.Add(new TCTestClass('b', 2));
        
        Assert.AreEqual(set2.HasAtLeast(set3), false);
    }

    [Test]
    public void HasAtLeastTest6()
    {
        ALTestClass set2 = new ALTestClass();
        set2.SetElements.Add(new TCTestClass('a', 0));
        set2.SetElements.Add(new TCTestClass('b', 2));
        set2.SetElements.Add(new TCTestClass('c', 5));

        ALTestClass set3 = new ALTestClass();
        set3.SetElements.Add(new TCTestClass('a', 1));
        set3.SetElements.Add(new TCTestClass('b', 2));
        
        Assert.AreEqual(set3.HasAtLeast(set2), false);
    }

    [Test]
    public void HasAtLeastTest7()
    {
        ALTestClass set1 = new ALTestClass();
        ALTestClass set2 = new ALTestClass();
        set2.SetElements.Add(new TCTestClass('a', 0));
        set2.SetElements.Add(new TCTestClass('b', 0));

        Assert.AreEqual(set1.HasAtLeast(set2), true);
    }


    [Test]
    public void TestGet()
    {
        ALTestClass set1 = new ALTestClass();
        set1.SetElements.Add(new TCTestClass('a', 1));
        set1.SetElements.Add(new TCTestClass('b', 3));
        set1.SetElements.Add(new TCTestClass('c', 7));

        float i1 = set1.Get('a').Data;
        Assert.AreEqual(i1, 1);
        float i2 = set1.Get('b').Data;
        Assert.AreEqual(i2, 3);
        float i3 = set1.Get('c').Data;
        Assert.AreEqual(i3, 7);
    }
    
}
