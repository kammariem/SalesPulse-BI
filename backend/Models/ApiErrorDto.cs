namespace SalesCubeApi.Models;

public sealed record ApiErrorDto(
    int StatusCode,
    string Error,
    string Message,
    DateTime TimestampUtc
);
