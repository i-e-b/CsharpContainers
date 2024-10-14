using System.Linq;
using Containers;
using NUnit.Framework;

namespace CsharpContainers.Tests;

[TestFixture]
public class CircularBufferTests
{
    [Test]
    public void can_create_empty_buffer()
    {
        var subject = new CircularBuffer<int>(1024);
        
        Assert.That(subject.Size, Is.EqualTo(0), "initial size");
        Assert.That(subject.Capacity, Is.EqualTo(1024), "fixed capacity");
        Assert.That(subject.IsEmpty, Is.True, "empty flag");
    }
    
    [Test]
    public void can_create_pre_populated_buffer()
    {
        var subject = new CircularBuffer<char>(1024, "Hello".ToArray());
        
        Assert.That(subject.Size, Is.EqualTo(5), "initial size");
        Assert.That(subject.Capacity, Is.EqualTo(1024), "fixed capacity");
        Assert.That(subject.IsEmpty, Is.False, "empty flag");
    }
}