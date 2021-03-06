using System.Text.RegularExpressions;
using OnRail.Extensions;
using OnRail.ResultDetails.Errors;

namespace OnRailTest.Extensions;

public static class StringExtensionsTest {
    private const string SampleEmail = "test@test.com";
    private const string InvalidEmail = "test.com";
    private static readonly Regex EmailRegex = new(@"^[\w-\.]+@([\w-]+\.)+[\w-]+");

    [Fact]
    public static void MustMatchRegex_MatchRegex_SuccessResult() {
        var result = SampleEmail.MustMatchRegex(EmailRegex);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public static void MustMatchRegex_NoMatchRegexWithoutErrorDetail_ReturnDefaultErrorDetail() {
        var result = InvalidEmail.MustMatchRegex(EmailRegex);

        Assert.False(result.IsSuccess);
        Assert.IsType<ArgumentError>(result.Detail);
    }
    
    [Fact]
    public static void MustMatchRegex_NoMatchRegexWithErrorDetail_ReturnErrorDetail() {
        var result = InvalidEmail.MustMatchRegex(EmailRegex, new BadRequestError());

        Assert.False(result.IsSuccess);
        Assert.IsType<BadRequestError>(result.Detail);
    }
}