using Kontursvet.Domain.Common;

namespace Kontursvet.Api.Common;

public static class ResultExtensions
{
    public static IResult ToHttp<T>(this Result<T> result) =>
        result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(result.Error,
                              statusCode: StatusCodes.Status400BadRequest,
                              title: result.ErrorCode);

    public static IResult ToHttp(this Result result) =>
        result.IsSuccess
            ? Results.NoContent()
            : Results.Problem(result.Error,
                              statusCode: StatusCodes.Status400BadRequest,
                              title: result.ErrorCode);
}