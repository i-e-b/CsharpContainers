using System;
using System.Linq;
using Containers;
using Containers.Types;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace CsharpContainers.Tests;

[TestFixture]
public class PartialResultTests
{
    [Test]
    public void can_create_a_success_result_and_read_it_back()
    {
            var result = PartialResult.Success(1);

            Assert.That(result.IsSuccess, Is.True, "Should have been success, but was not");
            Assert.That(result.IsFailure, Is.False, "Should NOT have been failure, but was");
            Assert.That(result.State, Is.EqualTo(PartialResultState.Success), $"Should have been 'complete', but was '{result.State}'");

            Assert.That(result.ResultData, Is.EqualTo(1), "Stored data was incorrect");
        }

    [Test]
    public void can_create_a_warning_result_with_data_and_more_than_one_error_cause()
    {
            var result_1 = PartialResult
                .WithData("data 1")
                .Warning("string warning")
                .Warning(new Exception("exception warning"));

            var result_2 = PartialResult<string>
                .Warning("String warning")
                .WithData("data 2");

            var result_3 = PartialResult<string>
                .Warning(new Exception("exception warning"))
                .WithData("data 3");


            // Assert on multiple causes
            Assert.That(result_1.Causes.Count, Is.EqualTo(2), "Wrong cause count");
            Assert.That(result_2.Causes.Count, Is.EqualTo(1), "Wrong cause count");
            Assert.That(result_3.Causes.Count, Is.EqualTo(1), "Wrong cause count");

            // Assert on the rest of the result
            Assert.That(result_1.State, Is.EqualTo(PartialResultState.Warning), "Wrong state");
            Assert.That(result_2.State, Is.EqualTo(PartialResultState.Warning), "Wrong state");
            Assert.That(result_3.State, Is.EqualTo(PartialResultState.Warning), "Wrong state");

            Assert.That(result_1.IsFailure, Is.True, "Failure state wrong");
            Assert.That(result_2.IsFailure, Is.True, "Failure state wrong");
            Assert.That(result_3.IsFailure, Is.True, "Failure state wrong");

            Assert.That(result_1.IsSuccess, Is.False, "Success state wrong");
            Assert.That(result_2.IsSuccess, Is.False, "Success state wrong");
            Assert.That(result_3.IsSuccess, Is.False, "Success state wrong");

            Assert.That(result_1.ResultData, Is.EqualTo("data 1"));
            Assert.That(result_2.ResultData, Is.EqualTo("data 2"));
            Assert.That(result_3.ResultData, Is.EqualTo("data 3"));
        }

    [Test]
    public void adding_an_error_and_a_warning_results_in_an_error()
    {
            var result_1 =
                PartialResult<int>
                    .Warning("warn")
                    .Failure("err")
                    .Failure(new Exception("ex"));

            var result_2 =
                PartialResult<int>
                    .Failure("fail")
                    .Warning("warn");

            var result_3 =
                PartialResult<int>
                    .Failure(new Exception("fail"))
                    .Warning("warn");

            Assert.That(result_1.State, Is.EqualTo(PartialResultState.Failed));
            Assert.That(result_2.State, Is.EqualTo(PartialResultState.Failed));
            Assert.That(result_3.State, Is.EqualTo(PartialResultState.Failed));

            Assert.That(string.Join(",", result_1.CauseMessages), Is.EqualTo("warn,err,ex"));
            Assert.That(string.Join(",", result_2.CauseMessages), Is.EqualTo("fail,warn"));
            Assert.That(string.Join(",", result_3.CauseMessages), Is.EqualTo("fail,warn"));
        }

    [Test]
    public void adding_success_and_failure_results_in_failure()
    {
            var result_1 = PartialResult<int>.Success(1).Failure("err");
            var result_2 = PartialResult.Success(2).Failure("fail");
            var result_3 = PartialResult<int>.Failure("fail").Success(1);
            
            Assert.That(result_1.State, Is.EqualTo(PartialResultState.Failed));
            Assert.That(result_2.State, Is.EqualTo(PartialResultState.Failed));
            Assert.That(result_3.State, Is.EqualTo(PartialResultState.Failed));
        }

    [Test]
    public void success_of_nothing_has_a_shortcut()
    {
            var result = PartialResult.Success();
            Assert.That(result.State, Is.EqualTo(PartialResultState.Success));
            Assert.That(result.ResultData, Is.EqualTo(Nothing.Instance));
        }
        
    [Test]
    public void warning_of_nothing_has_a_shortcut()
    {
            var result_1 = PartialResult.Warning("warn");
            var result_2 = PartialResult.Warning(new Exception("warn"));
            
            Assert.That(result_1.State, Is.EqualTo(PartialResultState.Warning));
            Assert.That(result_2.State, Is.EqualTo(PartialResultState.Warning));
            
            Assert.That(string.Join(",", result_1.CauseMessages), Is.EqualTo("warn"));
            Assert.That(string.Join(",", result_2.CauseMessages), Is.EqualTo("warn"));
            
            Assert.That(result_1.ResultData, Is.EqualTo(Nothing.Instance));
            Assert.That(result_2.ResultData, Is.EqualTo(Nothing.Instance));
        }
        
    [Test]
    public void failure_of_nothing_has_a_shortcut()
    {
            var result_1 = PartialResult.Failure("err");
            var result_2 = PartialResult.Failure(new Exception("err"));
            
            Assert.That(result_1.State, Is.EqualTo(PartialResultState.Failed));
            Assert.That(result_2.State, Is.EqualTo(PartialResultState.Failed));
            
            Assert.That(string.Join(",", result_1.CauseMessages), Is.EqualTo("err"));
            Assert.That(string.Join(",", result_2.CauseMessages), Is.EqualTo("err"));
            
            Assert.That(result_1.ResultData, Is.EqualTo(Nothing.Instance));
            Assert.That(result_2.ResultData, Is.EqualTo(Nothing.Instance));
        }
        
    [Test]
    public void can_change_the_contained_type_of_a_failure_result_to_help_error_propagation ()
    {
            var expected = new Exception("hello");
            var original = PartialResult<int>.Failure(expected);

            // ReSharper disable SuggestVarOrType_Elsewhere
            PartialResult<string> propagated_1 = original.PropagateFailure<string>();
            PartialResult<float>  propagated_2 = propagated_1.PropagateFailure<float>();
            // ReSharper restore SuggestVarOrType_Elsewhere

            Assert.That(propagated_2.Causes.Single(), Is.EqualTo(expected));
        }

}