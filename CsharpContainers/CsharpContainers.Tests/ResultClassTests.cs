using System;
using Containers;
using Containers.Types;
using NUnit.Framework;

namespace CsharpContainers.Tests
{
    [TestFixture]
    public class ResultClassTests {
        [Test]
        public void can_create_a_success_result_and_read_it_back ()
        {
            var data = "Hello, world";

            var result = Result.Success(data); // can use the `Result` rather than `Result<T>` class to infer result type.

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.IsFailure);
            Assert.AreEqual(data, result.ResultData);
        }
        
        [Test]
        public void can_create_a_failure_case_with_a_specific_exception ()
        {
            var msg = "Example";
            var error = new NotFiniteNumberException(msg);

            var result = Result<string>.Failure(error); // note, we can't infer the type of a failure

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsNull(result.ResultData);
            Assert.AreEqual(result.FailureCause, error);
            Assert.AreEqual(msg, result.FailureCause.Message);
        }
        
        [Test]
        public void can_create_a_failure_case_with_a_generic_exception_from_a_message_string ()
        {
            var msg = "Example";

            var result = Result<string>.Failure(msg); // here we pass just the message, without creating an Exception first

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.ResultData);
            Assert.AreEqual(msg, result.FailureCause.Message);
        }

        [Test]
        public void can_treat_a_result_as_boolean_to_read_success_status ()
        {
            var good = Result.Success("ok");
            var bad = Result<int>.Failure("bad");

            Assert.IsTrue(good);
            Assert.IsFalse(bad);
        }

        [Test]
        public void can_implicitly_cast_a_success_result_to_its_contained_type ()
        {
            var result = Result.Success("value");
            string value = result;

            Assert.AreEqual("value", value);
        }

        [Test]
        public void trying_to_implicitly_cast_a_failure_result_throws_the_underlying_failure_exception ()
        {
            var expected = new Exception("sample error");
            var result = Result<int>.Failure(expected);

            try
            {
                int value = result;
                Console.WriteLine(value);
            }
            catch (Exception actual)
            {
                Assert.AreEqual(expected, actual);
                return;
            }

            Assert.Fail("Did not throw exception");
        }

        [Test]
        public void can_change_the_contained_type_of_a_failure_result_to_help_error_propagation ()
        {
            var expected = new Exception("hello");
            var original = Result<int>.Failure(expected);

            // ReSharper disable SuggestVarOrType_Elsewhere
            Result<string> propagated_1 = original.PropagateFailure<string>();
            Result<float>  propagated_2 = propagated_1.PropagateFailure<float>();
            // ReSharper restore SuggestVarOrType_Elsewhere

            Assert.AreEqual(expected, propagated_2.FailureCause);
        }

        [Test]
        public void can_have_a_result_of_nothing () {
            var success1 = Result<Nothing>.Success(Nothing.Instance);
            var success2 = Result<Nothing>.Success(null);
            var failure = Result<Nothing>.Failure("A failure");

            Assert.That(success1.IsSuccess, Is.True);
            Assert.That(success1.ResultData, Is.EqualTo(Nothing.Instance));

            Assert.That(success2.IsSuccess, Is.True);
            Assert.That(failure.IsFailure, Is.True);
        }
    }
}