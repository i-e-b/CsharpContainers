using Containers;
using NUnit.Framework;

namespace CsharpContainers.Tests
{
    [TestFixture]
    public class MapTests {

        [Test]
        public void map_can_default_numeric_types () {
            var subject = new Map<string, int>();

            subject["one"]++;
            subject["two"] += 2;

            Assert.That(subject.Keys.Count, Is.EqualTo(2));
            Assert.That(subject["one"], Is.EqualTo(1));
            Assert.That(subject["two"], Is.EqualTo(2));
            Assert.That(subject["other"], Is.EqualTo(0));
        }

        [Test]
        public void map_can_use_generator_for_numeric_types () {
            var subject = new Map<string, int>(k=>9000);

            subject["one"]++;

            Assert.That(subject["one"], Is.GreaterThan(9000));
            Assert.That(subject["two"], Is.EqualTo(9000));
        }

        [Test]
        public void map_can_use_default_for_ref_type () {
            var subject = new Map<int, string>();

            subject[0] = "hello";

            Assert.That(subject[0], Is.EqualTo("hello"));
            Assert.That(subject[42], Is.Null);
        }

        [Test]
        public void map_can_use_generator_for_ref_type () {
            var subject = new Map<int, string>(k=> k+",000");

            subject[1] = "billions";

            Assert.That(subject[1], Is.EqualTo("billions"));
            Assert.That(subject[2], Is.EqualTo("2,000"));
        }

        [Test]
        public void map_can_generate_maps () {
            var subject = new Map<int, Map<string, string>>(k=>new Map<string,string>());

            subject[4]["twenty"] = "usted";

            Assert.That(subject[1].Keys, Is.Empty);
            Assert.That(subject[4]["twenty"], Is.EqualTo("usted"));
        }
    }
}