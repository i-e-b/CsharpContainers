﻿using System;
using Containers;
using NUnit.Framework;
// ReSharper disable PossibleNullReferenceException

namespace CsharpContainers.Tests;

[TestFixture]
public class DequeueTests
{
    private const double Epsilon = 0.0001;

    [Test]
    public void can_create_empty_vec()
    {
        var v = new Dequeue<double>();

        Assert.That(v.Length(), Is.EqualTo(0), "initial size");
        Assert.That(v.IsEmpty(), Is.True, "empty flag");
    }

    [Test]
    public void can_create_vec_with_single_value()
    {
        var v = Dequeue<double>.FromValue(4.2);

        Assert.That(v.Length(), Is.EqualTo(1), "initial size");
        Assert.That(v.IsEmpty(), Is.False, "empty flag");
        Assert.That(v.Get(0), Is.EqualTo(4.2).Within(Epsilon), "idx 0");
    }

    [Test]
    public void can_create_vec_by_adding_to_start()
    {
        var v = new Dequeue<double>();
        v.AddFirst(1.0);
        v.AddFirst(2.0);
        v.AddFirst(3.0);

        // Conceptually, [3,2,1]

        Assert.That(v.Length(), Is.EqualTo(3), "size");
        Assert.That(v.IsEmpty(), Is.False, "empty flag");

        // Check get by index
        Assert.That(v.Get(0), Is.EqualTo(3.0).Within(Epsilon), "idx 0");
        Assert.That(v.Get(2), Is.EqualTo(1.0).Within(Epsilon), "idx 2");
    }

    [Test]
    public void can_create_vec_by_adding_to_end()
    {
        var v = new Dequeue<double>();
        v.AddLast(1.0);
        v.AddLast(2.0);
        v.AddLast(3.0);

        // Conceptually, [1,2,3]

        Assert.That(v.Length(), Is.EqualTo(3), "size");
        Assert.That(v.IsEmpty(), Is.False, "empty flag");

        // Check get by index
        Assert.That(v.Get(0), Is.EqualTo(1.0).Within(Epsilon), "idx 0");
        Assert.That(v.Get(2), Is.EqualTo(3.0).Within(Epsilon), "idx 2");
    }

    [Test]
    public void can_create_vec_by_adding_to_both_sides()
    {
        var v = new Dequeue<double>();
        v.AddLast(1.0);
        v.AddLast(2.0);
        v.AddFirst(3.0);
        v.AddFirst(4.0);

        // Conceptually, [4,3,1,2]

        Assert.That(v.Length(), Is.EqualTo(4), "size");
        Assert.That(v.IsEmpty(), Is.False, "empty flag");

        // Check get by index
        Assert.That(v.Get(0), Is.EqualTo(4.0).Within(Epsilon), "idx 0");
        Assert.That(v.Get(1), Is.EqualTo(3.0).Within(Epsilon), "idx 1");
        Assert.That(v.Get(2), Is.EqualTo(1.0).Within(Epsilon), "idx 2");
        Assert.That(v.Get(3), Is.EqualTo(2.0).Within(Epsilon), "idx 3");
    }

    [Test]
    public void can_peek_at_vector_ends_without_removing_items()
    {
        var v = new Dequeue<double>();
        v.AddLast(1.0);
        v.AddLast(2.0);
        v.AddFirst(3.0);
        v.AddFirst(4.0);

        // Conceptually, [4,3,1,2]

        Assert.That(v.GetFirst(), Is.EqualTo(4.0).Within(Epsilon), "peek start 1");
        Assert.That(v.GetFirst(), Is.EqualTo(4.0).Within(Epsilon), "peek start 2");
        Assert.That(v.GetLast(), Is.EqualTo(2.0).Within(Epsilon), "peek end 1");
        Assert.That(v.GetLast(), Is.EqualTo(2.0).Within(Epsilon), "peek end 2");

        Assert.That(v.Length(), Is.EqualTo(4), "size");
        Assert.That(v.IsEmpty(), Is.False, "empty flag");

        // Check get by index
        Assert.That(v.Get(0), Is.EqualTo(4.0).Within(Epsilon), "idx 0");
        Assert.That(v.Get(1), Is.EqualTo(3.0).Within(Epsilon), "idx 1");
        Assert.That(v.Get(2), Is.EqualTo(1.0).Within(Epsilon), "idx 2");
        Assert.That(v.Get(3), Is.EqualTo(2.0).Within(Epsilon), "idx 3");
    }

    [Test]
    public void can_use_array_accessor_on_vector()
    {
        var v = new Dequeue<double>();
        v.AddLast(1.0);
        v.AddLast(2.0);
        v.AddFirst(3.0);
        v.AddFirst(4.0);

        // get by index
        Assert.That(v[0], Is.EqualTo(4.0).Within(Epsilon), "idx 0");
        Assert.That(v[1], Is.EqualTo(3.0).Within(Epsilon), "idx 1");
        Assert.That(v[2], Is.EqualTo(1.0).Within(Epsilon), "idx 2");
        Assert.That(v[3], Is.EqualTo(2.0).Within(Epsilon), "idx 3");

        // set by index
        v[0] = 29.98;
        v[1] = 33.51;
        v[2] = 31.35;
        v[3] = 55.66;

        // get by index
        Assert.That(v[0], Is.EqualTo(29.98).Within(Epsilon), "idx 0");
        Assert.That(v[1], Is.EqualTo(33.51).Within(Epsilon), "idx 1");
        Assert.That(v[2], Is.EqualTo(31.35).Within(Epsilon), "idx 2");
        Assert.That(v[3], Is.EqualTo(55.66).Within(Epsilon), "idx 3");
    }

    [Test]
    public void can_create_vec_from_array()
    {
        double[] src = { 0.1, 1.2, 2.3, 3.4, 4.5, 5.6 };
        var v = new Dequeue<double>(src);

        Assert.That(v.Length(), Is.EqualTo(6), "size");
        Assert.That(v.IsEmpty(), Is.False, "empty flag");

        // Check get by index
        Assert.That(v.Get(0), Is.EqualTo(0.1).Within(Epsilon), "idx 0");
        Assert.That(v.Get(5), Is.EqualTo(5.6).Within(Epsilon), "idx 5");
    }

    [Test]
    public void can_restore_array_from_vec()
    {
        double[] src = { 0.1, 1.2, 2.3, 3.4, 4.5, 5.6 };
        var v = new Dequeue<double>(src);

        var result = v.ToArray();

        Assert.That(v.Length(), Is.EqualTo(6), "vector length");
        Assert.That(result.Length, Is.EqualTo(6), "result length");

        for (var i = 0; i < src.Length; i++)
        {
            Assert.That(result[i], Is.EqualTo(src[i]).Within(Epsilon), "index " + i);
        }
    }

    [Test]
    public void can_check_if_indexes_are_in_bounds_of_vec()
    {
        double[] src = { 0.1, 1.2, 2.3, 3.4, 4.5, 5.6 };
        var v = new Dequeue<double>(src);

        Assert.False(v.HasIndex(-1), "too low");
        Assert.True(v.HasIndex(0), "first item");
        Assert.True(v.HasIndex(3), "middle item");
        Assert.True(v.HasIndex(5), "last item");
        Assert.False(v.HasIndex(6), "too high");
        Assert.False(v.HasIndex(6000), "way too high");
    }

    [Test]
    public void can_read_from_vec_giving_a_default_if_out_of_range()
    {
        double[] src = { 0.1, 1.2, 2.3 };
        var v = new Dequeue<double>(src);


        Assert.That(v.Get(-1, 42.0), Is.EqualTo(42.0).Within(Epsilon), "too low");
        Assert.That(v.Get(0, 42.0), Is.EqualTo(0.1).Within(Epsilon), "idx 0");
        Assert.That(v.Get(1, 42.0), Is.EqualTo(1.2).Within(Epsilon), "idx 1");
        Assert.That(v.Get(2, 42.0), Is.EqualTo(2.3).Within(Epsilon), "idx 2");
        Assert.That(v.Get(3, 42.0), Is.EqualTo(42.0).Within(Epsilon), "too high");
        Assert.That(v.Get(32767, 42.0), Is.EqualTo(42.0).Within(Epsilon), "way too high");
    }

    [Test]
    public void can_restore_array_from_vec_after_removing_items()
    {
        double[] src = { 0.1, 1.2, 2.3, 3.4, 4.5, 5.6 };
        var v = new Dequeue<double>(src);

        Assert.That(v.RemoveFirst(), Is.EqualTo(0.1).Within(Epsilon), "removed first");
        Assert.That(v.RemoveLast(), Is.EqualTo(5.6).Within(Epsilon), "removed last");

        var result = v.ToArray();

        Assert.That(v.Length(), Is.EqualTo(4), "vector length");
        Assert.That(result.Length, Is.EqualTo(4), "result length");

        for (var i = 0; i < result.Length; i++)
        {
            Assert.That(result[i], Is.EqualTo(src[i + 1]).Within(Epsilon), "index " + i);
        }
    }

    [Test]
    public void can_restore_array_from_vec_after_adding_items()
    {
        double[] src = { 0.1, 1.2, 2.3, 3.4, 4.5, 5.6 };
        var v = new Dequeue<double>(src);

        v.AddFirst(-1);
        v.AddLast(-2);

        double[] expected = { -1.0, 0.1, 1.2, 2.3, 3.4, 4.5, 5.6, -2.0 };
        var result = v.ToArray();

        Assert.That(v.Length(), Is.EqualTo(8), "vector length");
        Assert.That(result.Length, Is.EqualTo(8), "result length");

        for (var i = 0; i < expected.Length; i++)
        {
            Assert.That(result[i], Is.EqualTo(expected[i]).Within(Epsilon), "index " + i);
        }
    }

    [Test]
    public void can_remove_vector_items_by_index()
    {
        double[] src = { 0.1, 1.2, 2.3, 3.4, 4.5, 5.6 };
        var v = new Dequeue<double>(src);

        v.Delete(1);
        v.Delete(4); // index 5 in src

        double[] expected = { 0.1, 2.3, 3.4, 5.6 };
        var result = v.ToArray();

        Assert.That(v.Length(), Is.EqualTo(4), "vector length");
        Assert.That(result.Length, Is.EqualTo(4), "result length");

        for (var i = 0; i < expected.Length; i++)
        {
            Assert.That(result[i], Is.EqualTo(expected[i]).Within(Epsilon), "index " + i);
        }
    }

    [Test]
    public void can_clear_all_items_from_vector()
    {
        double[] src = { 0.1, 1.2, 2.3, 3.4, 4.5, 5.6 };
        var v = new Dequeue<double>(src);

        Assert.That(v.Length(), Is.EqualTo(6), "vector length before clear");

        v.Clear();
        Assert.That(v.Length(), Is.EqualTo(0), "vector length after clear");

        var result = v.ToArray();
        Assert.That(result.Length, Is.EqualTo(0), "array length after clear");

        // can start adding things again
        v.AddLast(1);
        v.AddLast(2);
        v.AddLast(3);

        double[] expected = { 1, 2, 3 };
        result = v.ToArray();

        Assert.That(v.Length(), Is.EqualTo(3), "vector length");
        Assert.That(result.Length, Is.EqualTo(3), "array length");

        for (var i = 0; i < expected.Length; i++)
        {
            Assert.That(result[i], Is.EqualTo(expected[i]).Within(Epsilon), "index " + i);
        }
    }

    [Test]
    public void can_modify_items_in_vec_in_place_by_index()
    {
        double[] src = { 0.1, 1.2, 2.3, 3.4, 4.5, 123 };
        var v = new Dequeue<double>(src);

        v.Edit(1, x => x + 10.0);
        v.Edit(2, x => x * 10.0);
        v.Set(4, -4.4);
        v.Edit(5, x => x % 10);

        double[] expected = { 0.1, 11.2, 23.0, 3.4, -4.4, 3 };
        var result = v.ToArray();

        Assert.That(v.Length(), Is.EqualTo(6), "vector length");
        Assert.That(result.Length, Is.EqualTo(6), "array length");

        for (var i = 0; i < expected.Length; i++)
        {
            Assert.That(result[i], Is.EqualTo(expected[i]).Within(Epsilon), "index " + i);
        }
    }

    [Test]
    public void vectors_can_scale_beyond_initial_bounds()
    {
        double[] initial = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
        var src = new Dequeue<double>(initial);
        var dst = new Dequeue<double>(8);

        while (src.NotEmpty())
        {
            Assert.False(src.IsEmpty());

            dst.AddFirst(src.RemoveLast());
            dst.AddLast(src.RemoveFirst());
        }

        double[] expected = { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var result = dst.ToArray();

        Assert.That(dst.Length(), Is.EqualTo(20), "dest length");
        Assert.That(result.Length, Is.EqualTo(20), "result length");
        Assert.That(src.Length(), Is.EqualTo(0), "source length");

        for (var i = 0; i < expected.Length; i++)
        {
            Assert.That(result[i], Is.EqualTo(expected[i]).Within(Epsilon), "index " + i);
        }
    }

    [Test]
    public void vectors_can_be_truncated_to_a_given_length()
    {
        double[] src = { 0.1, 1.2, 2.3, 3.4, 4.5, 5.6 };
        var v = new Dequeue<double>(src);

        v.TruncateTo(3);

        double[] expected = { 0.1, 1.2, 2.3 };
        var result = v.ToArray();

        Assert.That(v.Length(), Is.EqualTo(3), "vector length");
        Assert.That(result.Length, Is.EqualTo(3), "result length");

        for (var i = 0; i < expected.Length; i++)
        {
            Assert.That(result[i], Is.EqualTo(expected[i]).Within(Epsilon), "index " + i);
        }
    }

    [Test]
    public void vectors_can_have_leading_zeros_truncated()
    {
        double[] src = { 0.0, 0.0, 0.0000001, 1, 2, 0, 0, 0 };
        var v = new Dequeue<double>(src);

        Assert.That(v.Length(), Is.EqualTo(8), "vector length");
        v.TrimLeading(x => x == 0.0);
        Assert.That(v.Length(), Is.EqualTo(6), "length after");
        v.TrimLeading(x => x == 0.0); // no-op if no leading zeros
        Assert.That(v.Length(), Is.EqualTo(6), "length after");

        Assert.That(v.ToArray(), Is.EqualTo(new[] { 0.0000001, 1, 2, 0, 0, 0 }).Within(Epsilon).AsCollection, "values");
    }

    [Test]
    public void copied_vectors_do_not_share_data()
    {
        double[] src = { 0.1, 1.2, 2.3, 3.4, 4.5, 5.6 };
        var a = new Dequeue<double>(src);
        var b = new Dequeue<double>(a);

        a.Set(1, 1000.2);

        Assert.That(a.Get(1), Is.EqualTo(1000.2).Within(Epsilon), "A1");
        Assert.That(b.Get(1), Is.EqualTo(1.2).Within(Epsilon), "B1");

        b.Reverse();
        Assert.That(a.ToArray(), Is.EqualTo(new[] { 0.1, 1000.2, 2.3, 3.4, 4.5, 5.6 }).Within(Epsilon).AsCollection, "A2");
        Assert.That(b.ToArray(), Is.EqualTo(new[] { 5.6, 4.5, 3.4, 2.3, 1.2, 0.1 }).Within(Epsilon).AsCollection, "B2");
    }

    [Test]
    public void can_speculatively_read_start_and_end_of_vector()
    {
        int[] src = { 1, 2, 3, 4, 5 };
        var v = new Dequeue<int>(src);

        var ok = v.TryGetFirst(out var value);
        Assert.That(ok, Is.True);
        Assert.That(value, Is.EqualTo(1));

        ok = v.TryGetLast(out value);
        Assert.That(ok, Is.True);
        Assert.That(value, Is.EqualTo(5));

        v.RemoveFirst();
        v.RemoveFirst();
        v.RemoveLast();
        v.RemoveLast();

        ok = v.TryGetFirst(out value);
        Assert.That(ok, Is.True);
        Assert.That(value, Is.EqualTo(3));

        ok = v.TryGetLast(out value);
        Assert.That(ok, Is.True);
        Assert.That(value, Is.EqualTo(3));

        v.RemoveLast();

        ok = v.TryGetFirst(out value);
        Assert.That(ok, Is.False);

        ok = v.TryGetLast(out value);
        Assert.That(ok, Is.False);
    }

    [Test]
    public void can_speculatively_remove_items_from_start_and_end_of_vector()
    {
        int[] src = { 1, 2, 3, 4, 5 };
        var v = new Dequeue<int>(src);

        var ok = v.TryGetFirst(out var value);
        Assert.That(ok, Is.True);
        Assert.That(value, Is.EqualTo(1));

        ok = v.TryGetLast(out value);
        Assert.That(ok, Is.True);
        Assert.That(value, Is.EqualTo(5));

        ok = v.TryRemoveFirst(out value);
        Assert.That(ok, Is.True);
        Assert.That(value, Is.EqualTo(1));

        ok = v.TryRemoveFirst(out value);
        Assert.That(ok, Is.True);
        Assert.That(value, Is.EqualTo(2));

        ok = v.TryRemoveLast(out value);
        Assert.That(ok, Is.True);
        Assert.That(value, Is.EqualTo(5));

        ok = v.TryRemoveLast(out value);
        Assert.That(ok, Is.True);
        Assert.That(value, Is.EqualTo(4));

        ok = v.TryGetFirst(out value);
        Assert.That(ok, Is.True);
        Assert.That(value, Is.EqualTo(3));

        ok = v.TryGetLast(out value);
        Assert.That(ok, Is.True);
        Assert.That(value, Is.EqualTo(3));

        v.RemoveLast();

        ok = v.TryRemoveFirst(out _);
        Assert.That(ok, Is.False);

        ok = v.TryRemoveLast(out _);
        Assert.That(ok, Is.False);
    }

    [Test]
    public void can_create_vec_as_subset_of_another()
    {
        double[] src = { 0.1, 1.2, 2.3, 3.4, 4.5, 5.6 };
        var a = new Dequeue<double>(src);
        var b = a.Slice(1, 3);

        Assert.That(a.Length(), Is.EqualTo(6), "a length");
        Assert.That(b.Length(), Is.EqualTo(2), "b length");

        // values not shared
        a.Set(1, 100);
        a.Set(2, 200);

        // Check get by index
        Assert.That(a.ToArray(), Is.EqualTo(new[] { 0.1, 100, 200, 3.4, 4.5, 5.6 }).Within(Epsilon).AsCollection, "A");
        Assert.That(b.ToArray(), Is.EqualTo(new[] { 1.2, 2.3 }).Within(Epsilon).AsCollection, "B");
    }

    [Test]
    public void can_reverse_vector_in_place_after_various_operations_1()
    {
        var v = new Dequeue<double>();

        v.Reverse(); // should be a no-op, cause no errors
        Assert.That(v.Length(), Is.EqualTo(0), "vector length");

        v.AddLast(5); // 5
        v.AddFirst(4); // 4 5

        Console.WriteLine(string.Join(", ", v.ToArray()));
        Assert.That(v.ToArray(), Is.EqualTo(new[] { 4, 5 }).Within(Epsilon).AsCollection, "a");
        v.Reverse();
        Assert.That(v.ToArray(), Is.EqualTo(new[] { 5, 4 }).Within(Epsilon).AsCollection, "b");
        v.Reverse();
        Assert.That(v.ToArray(), Is.EqualTo(new[] { 4, 5 }).Within(Epsilon).AsCollection, "c");

        v.AddLast(6); // 4 5 6
        v.AddFirst(3); // 3 4 5 6
        v.AddLast(7); // 3 4 5 6 7
        v.AddFirst(2); // 2 3 4 5 6 7

        Assert.That(v.ToArray(), Is.EqualTo(new[] { 2, 3, 4, 5, 6, 7 }).Within(Epsilon).AsCollection, "d");
        v.Reverse();
        Assert.That(v.ToArray(), Is.EqualTo(new[] { 7, 6, 5, 4, 3, 2 }).Within(Epsilon).AsCollection, "e");
        v.Reverse();
        Assert.That(v.ToArray(), Is.EqualTo(new[] { 2, 3, 4, 5, 6, 7 }).Within(Epsilon).AsCollection, "f");

        v.AddLast(8); // 2 3 4 5 6 7 8
        v.AddFirst(1); // 1 2 3 4 5 6 7 8
        v.AddLast(9); // 1 2 3 4 5 6 7 8 9
        v.AddFirst(0); // 0 1 2 3 4 5 6 7 8 9

        Assert.That(v.ToArray(), Is.EqualTo(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }).Within(Epsilon).AsCollection, "g");
        v.Reverse();
        Assert.That(v.ToArray(), Is.EqualTo(new[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }).Within(Epsilon).AsCollection, "h");
        v.Reverse();
        Assert.That(v.ToArray(), Is.EqualTo(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }).Within(Epsilon).AsCollection, "i");
    }

    [Test]
    public void can_reverse_vector_in_place_after_various_operations_2()
    {
        var v = new Dequeue<double>();

        v.AddLast(-1);
        v.AddLast(-1);
        v.AddLast(0);
        v.AddLast(1);
        v.AddLast(2);
        v.AddLast(3);
        v.RemoveFirst();
        v.RemoveFirst();

        Assert.That(v.ToArray(), Is.EqualTo(new[] { 0, 1, 2, 3 }).Within(Epsilon).AsCollection, "a");
        v.Reverse();
        Assert.That(v.ToArray(), Is.EqualTo(new[] { 3, 2, 1, 0 }).Within(Epsilon).AsCollection, "b");
        v.Reverse();
        Assert.That(v.ToArray(), Is.EqualTo(new[] { 0, 1, 2, 3 }).Within(Epsilon).AsCollection, "c");
    }
}