using SalesCubeApi.Models;
using SalesCubeApi.Services;

namespace SalesCubeApi.Repositories;

public sealed class SsasAnalyticsRepository : IAnalyticsRepository
{
    private readonly ISsasService _ssasService;

    public SsasAnalyticsRepository(ISsasService ssasService)
    {
        _ssasService = ssasService;
    }

    public Task<MdxTableResponse> ExecuteMdxAsync(string mdx, CancellationToken cancellationToken = default)
    {
        return _ssasService.ExecuteTableAsync(mdx, cancellationToken);
    }
}
