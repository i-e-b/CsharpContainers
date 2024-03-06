using System;
using System.Linq;
using Containers;
using NUnit.Framework;

namespace CsharpContainers.Tests;

[TestFixture]
public class BiPartiteQueueTests
{
    [Test]
    public void can_create_a_queue_and_reserve_space()
    {
        var subject = new BiPartiteQueue(4096);
        
        Assert.That(subject.Buffer.Length, Is.EqualTo(4096));
        Assert.That(subject.GetBufferSize(), Is.EqualTo(4096));
        Assert.That(subject.GetCommittedSize(), Is.EqualTo(0));
        Assert.That(subject.GetReservationSize(), Is.EqualTo(0));
        
        var ok = subject.GetContiguousBlock(out var start, out var end);
        Assert.That(ok, Is.False, "should be no blocks allocated");
        Assert.That(end - start, Is.EqualTo(0));
    }

    [Test]
    public void can_reserve_and_commit_space()
    {
        var subject = new BiPartiteQueue(2048);
        
        var ok = subject.Reserve(512, out var start, out var end);
        Assert.That(ok, Is.True, "should have space");
        Assert.That(start, Is.LessThan(end), "indexes should be in order");
        Assert.That(end - start, Is.EqualTo(512), "should get correct space");
        
        Assert.That(subject.GetReservationSize(), Is.EqualTo(512), "Should be reserved");
        Assert.That(subject.GetCommittedSize(), Is.EqualTo(0), "Nothing committed yet");

        for (int i = start; i < end; i++) { subject.Buffer[i] = 0xA5; } // write into buffer
        
        subject.Commit(512);
        
        Assert.That(subject.GetReservationSize(), Is.EqualTo(0), "Reservation should be removed");
        Assert.That(subject.GetCommittedSize(), Is.EqualTo(512), "Written data should be committed");
        
        ok = subject.GetContiguousBlock(out start, out end);
        Assert.That(ok, Is.True, "should be able to see committed block");
        Assert.That(end - start, Is.EqualTo(512), "should be correct size");
        
        subject.DeCommitBlock(512);
        Assert.That(subject.GetReservationSize(), Is.EqualTo(0), "Reservation should be empty");
        Assert.That(subject.GetCommittedSize(), Is.EqualTo(0), "Written data should be removed");
        ok = subject.GetContiguousBlock(out start, out end);
        Assert.That(ok, Is.False, "should not have a committed block");
    }

    [Test]
    public void can_store_multiple_blocks_and_clear()
    {
        var subject = new BiPartiteQueue(2048);
        
        var ok = subject.Reserve(512, out var start, out var end);
        Assert.That(ok, Is.True, "should have space");
        for (int i = start; i < end; i++) { subject.Buffer[i] = 0x01; } // write into buffer
        subject.Commit(512);
        
        ok = subject.Reserve(512, out start, out end);
        Assert.That(ok, Is.True, "should have space");
        for (int i = start; i < end; i++) { subject.Buffer[i] = 0x02; } // write into buffer
        subject.Commit(512);
        
        Assert.That(subject.GetReservationSize(), Is.EqualTo(0), "Reservation should be removed");
        Assert.That(subject.GetCommittedSize(), Is.EqualTo(1024), "Written data should be committed");
        
        ok = subject.GetContiguousBlock(out start, out end);
        Assert.That(ok, Is.True, "should be able to see committed block");
        Assert.That(end - start, Is.EqualTo(1024), "should be correct size");
        
        subject.Clear();
        Assert.That(subject.GetReservationSize(), Is.EqualTo(0), "Reservation should be empty");
        Assert.That(subject.GetCommittedSize(), Is.EqualTo(0), "Written data should be removed");
        ok = subject.GetContiguousBlock(out start, out end);
        Assert.That(ok, Is.False, "should not have a committed block");
    }

    [Test]
    public void reservation_fails_if_requested_size_is_greater_than_buffer_size()
    {
        var subject = new BiPartiteQueue(1024);
        
        var ok = subject.Reserve(2048, out var start, out var end);
        Assert.That(ok, Is.False, "should not have space");
        Assert.That(end - start, Is.EqualTo(0), "size should be zero");
        
        Assert.That(subject.GetReservationSize(), Is.EqualTo(0), "Reservation should be empty");
        Assert.That(subject.GetCommittedSize(), Is.EqualTo(0), "Written data should be removed");
    }

    [Test]
    public void reservation_fails_if_space_is_exhausted()
    {
        var subject = new BiPartiteQueue(2000);

        var ok = subject.Reserve(512, out _, out _);
        Assert.That(ok, Is.True);
        subject.Commit(512);
        
        ok = subject.Reserve(512, out _, out _);
        Assert.That(ok, Is.True);
        subject.Commit(512);
        
        ok = subject.Reserve(512, out _, out _);
        Assert.That(ok, Is.True);
        subject.Commit(512);
        
        // Now not enough space left
        ok = subject.Reserve(512, out _, out _);
        Assert.That(ok, Is.False);
    }
    
    [Test]
    public void can_keep_cycling_through_the_buffer_if_blocks_are_de_committed()
    {
        var subject = new BiPartiteQueue(2048);

        for (int cycle = 0; cycle < 20; cycle++) // 10240 bytes in total
        {
            var ok = subject.Reserve(512, out var start, out var end);
            Assert.That(ok, Is.True);
            
            for (int i = start; i < end; i++) { subject.Buffer[i] = (byte)cycle; }
            
            subject.Commit(512);
            
            if (cycle > 2) subject.DeCommitBlock(512);
        }
        
        Console.WriteLine(string.Join(" ", subject.Buffer.Select(b=>b.ToString("X2"))));
    }
}