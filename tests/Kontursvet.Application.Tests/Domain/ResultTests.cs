using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Tests.Domain;

/// <summary>
/// Тесты базового типа результата: успех/провал, значение, код ошибки.
/// </summary>
public class ResultTests
{
    [Fact]
    public void Success_SetsIsSuccessAndValue()
    {
        var result = Result<int>.Success(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
        Assert.Null(result.Error);
        Assert.Null(result.ErrorCode);
    }

    [Fact]
    public void Failure_SetsErrorAndCode_AndValueIsDefault()
    {
        var result = Result<string>.Failure("Что-то пошло не так", "SOME_CODE");

        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal("Что-то пошло не так", result.Error);
        Assert.Equal("SOME_CODE", result.ErrorCode);
    }

    [Fact]
    public void Failure_WithoutCode_LeavesErrorCodeNull()
    {
        var result = Result<int>.Failure("Ошибка без кода");

        Assert.False(result.IsSuccess);
        Assert.Equal("Ошибка без кода", result.Error);
        Assert.Null(result.ErrorCode);
    }

    [Fact]
    public void NonGeneric_Success_HasNoError()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.Null(result.Error);
        Assert.Null(result.ErrorCode);
    }

    [Fact]
    public void NonGeneric_Failure_SetsErrorAndCode()
    {
        var result = Result.Failure("Карточка не найдена", "CARD_NOT_FOUND");

        Assert.False(result.IsSuccess);
        Assert.Equal("Карточка не найдена", result.Error);
        Assert.Equal("CARD_NOT_FOUND", result.ErrorCode);
    }
}
