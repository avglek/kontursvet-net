using Kontursvet.Api.Common;
using Kontursvet.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kontursvet.Api.Tests.Common;

/// <summary>
/// Тесты маппинга Result → HTTP: статусы успешных ответов, коды ошибок → статусы,
/// содержимое ProblemDetails (title = код ошибки, detail = текст).
/// </summary>
public class ResultExtensionsTests
{
    /// <summary>Достаёт HTTP-статус из ленивого <see cref="IResult"/>.</summary>
    private static int StatusOf(IResult result) =>
        result is IStatusCodeHttpResult { StatusCode: not null } status
            ? status.StatusCode!.Value
            : throw new InvalidOperationException($"{result.GetType().Name} не содержит статуса");

    // ==================== Result<T> ====================

    [Fact]
    public void ToHttp_GenericSuccessValueType_Returns200WithValue()
    {
        var result = Result<long>.Success(42).ToHttp();

        Assert.Equal(StatusCodes.Status200OK, StatusOf(result));
        Assert.Equal(42L, Assert.IsAssignableFrom<IValueHttpResult<long>>(result).Value);
    }

    [Fact]
    public void ToHttp_GenericSuccessReferenceType_Returns200WithValue()
    {
        var result = Result<string>.Success("payload").ToHttp();

        Assert.Equal(StatusCodes.Status200OK, StatusOf(result));
        Assert.Equal("payload", Assert.IsAssignableFrom<IValueHttpResult<string>>(result).Value);
    }

    [Theory]
    [InlineData("CARD_NOT_FOUND")]
    [InlineData("PRODUCT_NOT_FOUND")]
    public void ToHttp_NotFoundCodes_Returns404(string code)
    {
        var result = Result<string>.Failure("не найдено", code).ToHttp();

        Assert.Equal(StatusCodes.Status404NotFound, StatusOf(result));
    }

    [Fact]
    public void ToHttp_CodeDuplicate_Returns409()
    {
        var result = Result<string>.Failure("Код занят", "PRODUCT_CODE_DUPLICATE").ToHttp();

        Assert.Equal(StatusCodes.Status409Conflict, StatusOf(result));
    }

    [Theory]
    [InlineData("CARD_LINK_REQUIRED")]
    [InlineData("AUTH_INVALID")]
    [InlineData("FILE_TOO_LARGE")]
    [InlineData("GALLERY_KEY_MISMATCH")]
    [InlineData("LEAD_PHONE_REQUIRED")]
    public void ToHttp_OtherCodes_Returns400(string code)
    {
        var result = Result<string>.Failure("ошибка", code).ToHttp();

        Assert.Equal(StatusCodes.Status400BadRequest, StatusOf(result));
    }

    [Fact]
    public void ToHttp_NullCode_Returns400()
    {
        var result = Result<string>.Failure("Ошибка без кода", null).ToHttp();

        Assert.Equal(StatusCodes.Status400BadRequest, StatusOf(result));
    }

    // ==================== Result (без значения) ====================

    [Fact]
    public void ToHttp_Success_Returns204()
    {
        var result = Result.Success().ToHttp();

        Assert.Equal(StatusCodes.Status204NoContent, StatusOf(result));
    }

    [Fact]
    public void ToHttp_FailureNotFound_Returns404()
    {
        var result = Result.Failure("Карточка не найдена", "CARD_NOT_FOUND").ToHttp();

        Assert.Equal(StatusCodes.Status404NotFound, StatusOf(result));
    }

    [Fact]
    public void ToHttp_FailureValidation_Returns400_WithProblemDetails()
    {
        var result = Result.Failure("Поле Title обязательно", "CARD_TITLE_REQUIRED").ToHttp();

        Assert.Equal(StatusCodes.Status400BadRequest, StatusOf(result));

        var problem = Assert.IsAssignableFrom<IValueHttpResult<ProblemDetails>>(result).Value!;
        Assert.Equal("CARD_TITLE_REQUIRED", problem.Title);
        Assert.Equal("Поле Title обязательно", problem.Detail);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
    }

    [Fact]
    public void ToHttp_GenericFailure_KeepsErrorMessageInProblem()
    {
        var result = Result<int>.Failure("MAX недоступен", "MAX_SEND_FAILED").ToHttp();

        var problem = Assert.IsAssignableFrom<IValueHttpResult<ProblemDetails>>(result).Value!;
        Assert.Equal("MAX недоступен", problem.Detail);
    }
}
