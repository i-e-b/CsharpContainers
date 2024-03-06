using System;
using Containers;
using NUnit.Framework;

namespace CsharpContainers.Tests;

[TestFixture]
public class AgeDictionaryTests
{

    [Test]
    public void age_dictionary_example()
    {
        var counts = new AgeDictionary<int, int>(maxAge: TimeSpan.FromHours(1), maxCount: 8172);

        for (int i = 0; i < 5; i++)
        {
            if (counts.TryGetValue(5, out var lastCount))
            {
                Console.WriteLine(lastCount);
            }
            counts.TryAdd(5, lastCount+1);
        }
    }
}