namespace SalesCubeApi.Models;

public sealed record SsasConnectionStatusDto(
    bool IsConnected,
    string? ServerVersion,
    string? Database,
    string Message,
    DateTime CheckedAtUtc
);
