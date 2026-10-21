using SalesCubeApi.Models;

namespace SalesCubeApi.Repositories;

public interface IAnalyticsRepository
{
    Task<MdxTableResponse> ExecuteMdxAsync(string mdx, CancellationToken cancellationToken = default);
}
