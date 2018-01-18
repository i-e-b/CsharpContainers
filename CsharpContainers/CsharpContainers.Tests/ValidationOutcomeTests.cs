using System;
using Containers;
using NUnit.Framework;

namespace CsharpContainers.Tests
{
    [TestFixture]
    public class ValidationOutcomeTests
    {
        [Test]
        public void can_create_a_validation_failure_and_read_it_back()
        {
            var error_msg = "Hello, world";

            var result = ValidationOutcome.Fail(error_msg); // can use the `ValidationOutcome` rather than `ValidationOutcome<T>` class to infer type of failures

            Assert.IsTrue(result.HasError);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(error_msg, result.ValidationErrors);
        }
        
        [Test]
        public void can_create_a_validation_pass_and_read_it_back ()
        {
            var result = ValidationOutcome<string>.Pass(); // note, we can't infer the type of a pass
            
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void can_treat_a_result_as_boolean_to_read_success_status ()
        {
            var good = ValidationOutcome<int>.Pass();
            var bad = ValidationOutcome.Fail("reason");

            Assert.IsTrue(good);
            Assert.IsFalse(bad);
        }

        [Test]
        public void can_implicitly_cast_a_success_result_to_its_contained_type ()
        {
            var result = ValidationOutcome.Fail("value");
            string value = result;

            Assert.AreEqual("value", value);
        }

        [Test]
        public void trying_to_implicitly_cast_a_passed_result_throws_an_exception ()
        {
            var result = ValidationOutcome<int>.Pass();

            try
            {
                int value = result;
                Console.WriteLine(value);
            }
            catch (InvalidOperationException actual)
            {
                Assert.AreEqual("Tried to read errors of a successful validation", actual.Message);
                return;
            }

            Assert.Fail("Did not throw exception");
        }
    }
}