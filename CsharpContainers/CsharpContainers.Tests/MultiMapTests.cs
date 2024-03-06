using Containers;
using NUnit.Framework;

namespace CsharpContainers.Tests;

[TestFixture]
public class MultiMapTests
{

    [Test]
    public void multi_map_example()
    {
        var subject = new MultiMap<int, string>();
        
        Assert.That(subject.KeyCount, Is.Zero);
        Assert.That(subject.ValueCount, Is.Zero);
        
        subject.Add(1, "a");
        subject.Add(1, "b");
        subject.Add(1, "c");
        subject.Add(2, "x");
        
        Assert.That(subject.KeyCount, Is.EqualTo(2));
        Assert.That(subject.ValueCount, Is.EqualTo(4));
        
        Assert.That(subject.Remove(1, "b"), Is.True);
        Assert.That(subject.Remove(2, "b"), Is.False);
        
        Assert.That(subject.KeyCount, Is.EqualTo(2));
        Assert.That(subject.ValueCount, Is.EqualTo(3));
        
        Assert.That(subject.AllKeys(), Is.EqualTo(new[]{1,2}).AsCollection);
        Assert.That(subject.ListFor(1), Is.EqualTo(new[]{"a","c"}).AsCollection);
    }

}