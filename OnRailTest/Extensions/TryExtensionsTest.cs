using OnRail;
using OnRail.Extensions;
using OnRail.ResultDetails;
using OnRail.ResultDetails.Errors;

namespace OnRailTest.Extensions;

public class TryExtensionsTest {
    #region TestMethods

    private const int DefaultNumOfTry = 3;
    private const string SuccessStr = "Success";
    private static void SuccessfulAction() { }
    private static void SuccessfulActionWithInput(string input) { }
    private static string SuccessfulFunctionReturnString() => SuccessStr;
    private static string SuccessfulFunctionReturnInput(string input) => input;
    private static Result SuccessfulFunctionReturnResult() => Result.Ok();

    private static Result<string> SuccessfulFunctionReturnResultWithString() =>
        Result<string>.Ok(SuccessStr);

    private static Result<string> SuccessfulFunctionReturnResultWithString(string input) =>
        Result<string>.Ok(input);

    private static void FailAction() => throw new Exception();
    private static void FailActionWithInput(string input) => throw new Exception();
    private static string FailFunctionReturnString() => throw new Exception();
    private static string FailFunctionReturnInput(string input) => throw new Exception();
    private static Result FailFunctionReturnResult() => Result.Fail(new BadRequestError());

    #endregion

    private static void EnsureIsSuccess(ResultBase result) {
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Detail.GetMoreDetailProperties<int>("numOfTry").Single());
        Assert.Equal(DefaultNumOfTry, result.Detail.GetMoreDetailProperties<int>("maxTryRequested").Single());
    }

    #region Try

    [Fact]
    public void Try_SuccessfulAction_ReturnSuccessDetail() {
        var result = TryExtensions.Try(SuccessfulAction, DefaultNumOfTry);

        EnsureIsSuccess(result);
    }

    [Fact]
    public void Try_SuccessfulActionWithInput_ReturnSuccessDetail() {
        var result = TryExtensions.Try(() => SuccessfulActionWithInput("input"), DefaultNumOfTry);

        EnsureIsSuccess(result);
    }

    [Fact]
    public void Try_SuccessfulActionWithInput2_ReturnSuccessDetail() {
        const string input = "input";
        var result = input.Try(SuccessfulActionWithInput, DefaultNumOfTry);

        EnsureIsSuccess(result);
    }

    [Fact]
    public void Try_SuccessfulFunction_ReturnString() {
        var result = TryExtensions.Try(SuccessfulFunctionReturnString, DefaultNumOfTry);

        EnsureIsSuccess(result);
        Assert.Equal(SuccessStr, result.Value);
    }

    [Fact]
    public void Try_SuccessfulFunction_ReturnResultWithString() {
        var result = TryExtensions.Try(SuccessfulFunctionReturnResultWithString, DefaultNumOfTry);

        EnsureIsSuccess(result);
        Assert.Equal(SuccessStr, result.Value);
    }

    [Fact]
    public void Try_SuccessfulFunction_ReturnsResult() {
        var result = TryExtensions.Try(SuccessfulFunctionReturnResult, DefaultNumOfTry);

        EnsureIsSuccess(result);
    }

    [Fact]
    public void Try_SuccessfulFunctionWithInput_ReturnInput() {
        const string input = "input";
        var result = TryExtensions.Try(() => SuccessfulFunctionReturnInput(input), DefaultNumOfTry);

        EnsureIsSuccess(result);
        Assert.Equal(input, result.Value);
    }

    [Fact]
    public void Try_SuccessfulFunctionWithInput_ReturnResultWithString() {
        const string input = "input";
        var result = TryExtensions.Try(() => SuccessfulFunctionReturnResultWithString(input), DefaultNumOfTry);

        EnsureIsSuccess(result);
        Assert.Equal(input, result.Value);
    }

    //----------------------------------------------------------------------------
    private static Result<string> FailFunctionReturnResultWithString() =>
        Result<string>.Fail(new BadRequestError());

    private static Result<string> FailFunctionReturnResultWithString(string input) =>
        Result<string>.Fail(new BadRequestError());

    [Fact]
    public void Try_FailAction_ReturnExceptionError() {
        var result = TryExtensions.Try(FailAction);

        Utility.EnsureIsExceptionError(result, 1);
    }

    [Fact]
    public void Try_FailActionWithRepeat_ReturnExceptionError() {
        var result = TryExtensions.Try(FailAction, DefaultNumOfTry);

        Utility.EnsureIsExceptionError(result, DefaultNumOfTry);
    }

    [Fact]
    public void Try_FailActionWithInput_ReturnExceptionError() {
        var result = TryExtensions.Try(() => FailActionWithInput("input"));

        Assert.False(result.IsSuccess);
        Assert.True(result.Detail is ExceptionError);
    }

    [Fact]
    public void Try_FailActionWithInputRepeat_ReturnExceptionError() {
        var result = TryExtensions.Try(() => FailActionWithInput("input"), DefaultNumOfTry);

        Utility.EnsureIsExceptionError(result, DefaultNumOfTry);
    }

    [Fact]
    public void Try_FailActionWithInput2_ReturnExceptionError() {
        const string input = "input";
        var result = input.Try(FailActionWithInput);

        Assert.False(result.IsSuccess);
        Assert.True(result.Detail is ExceptionError);
    }

    [Fact]
    public void Try_FailActionWithInputRepeat2_ReturnExceptionError() {
        const string input = "input";
        var result = input.Try(FailActionWithInput, DefaultNumOfTry);

        Utility.EnsureIsExceptionError(result, DefaultNumOfTry);
    }

    [Fact]
    public void Try_FailFunction_ReturnExceptionError() {
        var result = TryExtensions.Try(FailFunctionReturnString);

        Assert.False(result.IsSuccess);
        Assert.True(result.Detail is ExceptionError);
    }

    [Fact]
    public void Try_FailFunctionRepeat_ReturnExceptionError() {
        var result = TryExtensions.Try(FailFunctionReturnString, DefaultNumOfTry);

        Utility.EnsureIsExceptionError(result, DefaultNumOfTry);
    }

    [Fact]
    public void Try_FailFunction_ReturnBadRequestError() {
        var result = TryExtensions.Try(FailFunctionReturnResultWithString);

        Assert.False(result.IsSuccess);
        Assert.True(result.Detail is BadRequestError);
    }

    [Fact]
    public void Try_FailFunctionRepeat_ReturnErrorDetail() {
        var result = TryExtensions.Try(FailFunctionReturnResultWithString, DefaultNumOfTry);

        Utility.EnsureIsErrorDetail(result, DefaultNumOfTry);
    }

    [Fact]
    public void Try_FailFunction_ReturnsErrorDetail() {
        var result = TryExtensions.Try(FailFunctionReturnResult);

        Assert.False(result.IsSuccess);
        Assert.True(result.Detail is ErrorDetail);
    }

    [Fact]
    public void Try_FailFunctionReturnResult_ReturnErrorDetail() {
        var result = TryExtensions.Try(FailFunctionReturnResult, DefaultNumOfTry);

        Utility.EnsureIsErrorDetail(result, DefaultNumOfTry);
    }

    [Fact]
    public void Try_FailFunctionWithInput_ReturnExceptionError() {
        const string input = "input";
        var result = TryExtensions.Try(() => FailFunctionReturnInput(input));

        Assert.False(result.IsSuccess);
        Assert.True(result.Detail is ExceptionError);
    }

    [Fact]
    public void Try_FailFunctionReturnResultRepeat_ReturnExceptionError() {
        var result = TryExtensions.Try(() => FailFunctionReturnInput("input"), DefaultNumOfTry);

        Utility.EnsureIsExceptionError(result, DefaultNumOfTry);
    }

    [Fact]
    public void Try_FailFunctionWithInput_ReturnResultWithString() {
        const string input = "input";
        var result = TryExtensions.Try(() => FailFunctionReturnResultWithString(input));

        Assert.False(result.IsSuccess);
        Assert.True(result.Detail is ErrorDetail);
    }

    [Fact]
    public void Try_FailFunctionReturnResultWithString_ReturnErrorDetail() {
        const string input = "input";
        var result = TryExtensions.Try(() => FailFunctionReturnResultWithString(input), DefaultNumOfTry);

        Utility.EnsureIsErrorDetail(result, DefaultNumOfTry);
    }

    #endregion

    #region TryAsync

    [Fact]
    public async Task TryAsync_SuccessFunctionReturnValue_ReturnValue() {
        var result = await TryExtensions.Try(() => Task.FromResult(SuccessStr), DefaultNumOfTry);

        EnsureIsSuccess(result);
        Assert.Equal(SuccessStr, result.Value);
    }

    [Fact]
    public async Task TryAsync_FailFunctionReturnValue_ReturnExceptionError() {
        var result = await TryExtensions.Try(() => Task.FromResult(FailFunctionReturnString()), DefaultNumOfTry);

        Utility.EnsureIsExceptionError(result, DefaultNumOfTry);
    }

    [Fact]
    public async Task TryAsync_SuccessFunctionReturnResultWithValue_ReturnResultWithValue() {
        var result = await TryExtensions.Try(() => Task.FromResult(SuccessfulFunctionReturnResultWithString()),
            DefaultNumOfTry);

        EnsureIsSuccess(result);
        Assert.Equal(SuccessStr, result.Value);
    }

    [Fact]
    public async Task TryAsync_FailFunctionReturnResultWithValue_ReturnExceptionError() {
        var result =
            await TryExtensions.Try(() => Task.FromResult(FailFunctionReturnResultWithString()), DefaultNumOfTry);

        Utility.EnsureIsErrorDetail(result, DefaultNumOfTry);
    }

    [Fact]
    public async Task TryAsync_SuccessFunctionReturnTask_ReturnTask() {
        var result = await TryExtensions.Try(() => Task.Run(SuccessfulAction), DefaultNumOfTry);

        EnsureIsSuccess(result);
    }

    [Fact]
    public async Task TryAsync_FailFunctionReturnTask_ReturnExceptionError() {
        var result = await TryExtensions.Try(() => Task.Run(FailAction), DefaultNumOfTry);

        Utility.EnsureIsExceptionError(result, DefaultNumOfTry);
    }

    [Fact]
    public async Task TryAsync_SuccessFunctionReturnTaskResult_ReturnTaskResult() {
        var result =
            await TryExtensions.Try(() => Task.FromResult(SuccessfulFunctionReturnResult()), DefaultNumOfTry);

        EnsureIsSuccess(result);
    }

    [Fact]
    public async Task TryAsync_FailFunctionReturnTaskResult_ReturnExceptionError() {
        var result = await TryExtensions.Try(() => Task.FromResult(FailFunctionReturnResult()), DefaultNumOfTry);

        Utility.EnsureIsErrorDetail(result, DefaultNumOfTry);
    }

    [Fact]
    public async Task TryAsync_SuccessFunctionReturnTaskAndValue_ReturnTaskResult() {
        var result = await SuccessStr.Try(_ => Task.Run(SuccessfulAction), DefaultNumOfTry);

        EnsureIsSuccess(result);
    }

    [Fact]
    public async Task TryAsync_FailFunctionReturnTaskAndValue_ReturnExceptionError() {
        var result = await SuccessStr.Try(_ => Task.Run(FailAction), DefaultNumOfTry);

        Utility.EnsureIsExceptionError(result, DefaultNumOfTry);
    }

    [Fact]
    public async Task TryAsync_SuccessActionWithInputAndCorrectValue_ReturnTaskResult() {
        var @this = Task.FromResult(SuccessStr);
        var result = await @this.Try(SuccessfulActionWithInput, DefaultNumOfTry);

        EnsureIsSuccess(result);
    }

    [Fact]
    public async Task TryAsync_FailValueCorrectActionWithInput_ReturnExceptionError() {
        var @this = Task.Run(FailFunctionReturnString);
        var result = await @this.Try(SuccessfulActionWithInput, DefaultNumOfTry);

        Utility.EnsureIsExceptionError(result, DefaultNumOfTry);
    }

    [Fact]
    public async Task TryAsync_FailActionWithInputCorrectValue_ReturnExceptionError() {
        var @this = Task.Run(SuccessfulFunctionReturnString);
        var result = await @this.Try(FailActionWithInput, DefaultNumOfTry);

        Utility.EnsureIsExceptionError(result, DefaultNumOfTry);
    }

    [Fact]
    public async Task TryAsync_SuccessActionAndCorrectTaskValue_ReturnTaskResult() {
        var @this = Task.FromResult(SuccessStr);
        var result = await @this.Try(SuccessfulAction, DefaultNumOfTry);

        EnsureIsSuccess(result);
    }

    [Fact]
    public async Task TryAsync_FailTaskValueCorrectAction_ReturnExceptionError() {
        var @this = Task.Run(FailFunctionReturnString);
        var result = await @this.Try(SuccessfulAction, DefaultNumOfTry);

        Utility.EnsureIsExceptionError(result, DefaultNumOfTry);
    }

    [Fact]
    public async Task TryAsync_FailActionCorrectTaskValue_ReturnExceptionError() {
        var @this = Task.Run(SuccessfulFunctionReturnString);
        var result = await @this.Try(FailAction, DefaultNumOfTry);

        Utility.EnsureIsExceptionError(result, DefaultNumOfTry);
    }

    [Fact]
    public async Task TryAsync_SuccessActionAndCorrectTask_ReturnTaskResult() {
        var @this = Task.Run(() => { });
        var result = await @this.Try(SuccessfulAction, DefaultNumOfTry);

        EnsureIsSuccess(result);
    }

    [Fact]
    public async Task TryAsync_FailTaskCorrectAction_ReturnExceptionError() {
        var @this = Task.Run(FailAction);
        var result = await @this.Try(SuccessfulAction, DefaultNumOfTry);

        Utility.EnsureIsExceptionError(result, DefaultNumOfTry);
    }

    [Fact]
    public async Task TryAsync_FailActionCorrectTask_ReturnExceptionError() {
        var @this = Task.Run(() => { });
        var result = await @this.Try(FailAction, DefaultNumOfTry);

        Utility.EnsureIsExceptionError(result, DefaultNumOfTry);
    }

    [Fact]
    public async Task TryAsync_SuccessFunctionAndCorrectTask_ReturnTaskResult() {
        var @this = Task.Run(() => { });
        var result = await @this.Try(SuccessfulFunctionReturnResult, DefaultNumOfTry);

        EnsureIsSuccess(result);
    }

    [Fact]
    public async Task TryAsync_FailTaskCorrectFunction_ReturnErrorDetail() {
        var @this = Task.Run(FailAction);
        var result = await @this.Try(SuccessfulFunctionReturnResult, DefaultNumOfTry);

        Utility.EnsureIsErrorDetail(result, DefaultNumOfTry);
    }

    [Fact]
    public async Task TryAsync_FailFunctionCorrectTask_ReturnErrorDetail() {
        var @this = Task.Run(() => { });
        var result = await @this.Try(FailFunctionReturnResult, DefaultNumOfTry);

        Utility.EnsureIsErrorDetail(result, DefaultNumOfTry);
    }

    [Fact]
    public async Task TryAsync_SuccessFunctionWithResultAndCorrectTask_ReturnTaskResult() {
        var @this = Task.Run(() => { });
        var result = await @this.Try(() => Task.FromResult(SuccessfulFunctionReturnResult()), DefaultNumOfTry);

        EnsureIsSuccess(result);
    }

    [Fact]
    public async Task TryAsync_FailTaskCorrectFunctionWithResult_ReturnErrorDetail() {
        var @this = Task.Run(FailAction);
        var result = await @this.Try(() => Task.FromResult(SuccessfulFunctionReturnResult()), DefaultNumOfTry);

        Utility.EnsureIsErrorDetail(result, DefaultNumOfTry);
    }

    [Fact]
    public async Task TryAsync_FailFunctionWithResultCorrectTask_ReturnErrorDetail() {
        var @this = Task.Run(() => { });
        var result = await @this.Try(() => Task.FromResult(FailFunctionReturnResult()), DefaultNumOfTry);

        Utility.EnsureIsErrorDetail(result, DefaultNumOfTry);
    }

    [Fact]
    public async Task TryAsync_SuccessFunctionWithInputAndCorrectTaskWithValue_ReturnTaskResult() {
        var @this = Task.FromResult(SuccessStr);
        var result = await @this.Try(input => Task.FromResult(SuccessfulFunctionReturnInput(input)),
            DefaultNumOfTry);

        EnsureIsSuccess(result);
    }

    [Fact]
    public async Task TryAsync_FailTaskWithValueAndCorrectFunctionWithInput_ReturnExceptionError() {
        var @this = Task.Run(FailFunctionReturnString);
        var result = await @this.Try(input => Task.FromResult(SuccessfulFunctionReturnInput(input)),
            DefaultNumOfTry);

        Utility.EnsureIsExceptionError(result, DefaultNumOfTry);
    }

    [Fact]
    public async Task TryAsync_FailFunctionWithInputCorrectTaskWithValue_ReturnExceptionError() {
        var @this = Task.FromResult(SuccessStr);
        var result = await @this.Try(input => Task.FromResult(FailFunctionReturnInput(input)), DefaultNumOfTry);

        Utility.EnsureIsExceptionError(result, DefaultNumOfTry);
    }

    [Fact]
    public async Task TryAsync_SuccessFunctionReturnTaskResultAndValue_ReturnTaskResult() {
        var result = await SuccessStr.Try(_ => Task.FromResult(SuccessfulFunctionReturnResult()), DefaultNumOfTry);

        EnsureIsSuccess(result);
    }

    [Fact]
    public async Task TryAsync_FailFunctionReturnTaskResultAndValue_ReturnErrorDetail() {
        var result = await SuccessStr.Try(_ => Task.FromResult(FailFunctionReturnResult()), DefaultNumOfTry);

        Utility.EnsureIsErrorDetail(result, DefaultNumOfTry);
    }

    [Fact]
    public async Task TryAsync_SuccessFunctionWithInputTaskResultAndCorrectTaskValue_ReturnTaskResult() {
        var @this = Task.FromResult(SuccessStr);
        var result =
            await @this.Try<string>(_ => Task.FromResult(SuccessfulFunctionReturnResult()), DefaultNumOfTry);

        EnsureIsSuccess(result);
    }

    [Fact]
    public async Task TryAsync_FailTaskValueCorrectFunctionWithInputTaskResul_ReturnErrorDetail() {
        var @this = Task.Run(FailFunctionReturnString);
        var result =
            await @this.Try<string>(_ => Task.FromResult(SuccessfulFunctionReturnResult()), DefaultNumOfTry);

        Utility.EnsureIsErrorDetail(result, DefaultNumOfTry);
    }

    [Fact]
    public async Task TryAsync_FailFunctionWithInputTaskResulCorrectTaskValue_ReturnErrorDetail() {
        var @this = Task.FromResult(SuccessStr);
        var result = await @this.Try<string>(_ => Task.FromResult(FailFunctionReturnResult()), DefaultNumOfTry);

        Utility.EnsureIsErrorDetail(result, DefaultNumOfTry);
    }

    #endregion
}