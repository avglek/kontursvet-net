using Kontursvet.Domain.Common;

namespace Kontursvet.Api.Common;

public static class ResultExtensions
{
    public static IResult ToHttp<T>(this Result<T> result) =>
        result.IsSuccess
            ? Results.Ok(result.Value)
            : Problem(result.Error!, result.ErrorCode);

    public static IResult ToHttp(this Result result) =>
        result.IsSuccess
            ? Results.NoContent()
            : Problem(result.Error!, result.ErrorCode);

    private static IResult Problem(string error, string? code)
    {
        var status = code switch
        {
            "CARD_NOT_FOUND" => StatusCodes.Status404NotFound,
            "PRODUCT_NOT_FOUND" => StatusCodes.Status404NotFound,
            "PRODUCT_CODE_DUPLICATE" => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };

        return Results.Problem(error, statusCode: status, title: code);
    }
}