using System.ComponentModel.DataAnnotations;

namespace SalesCubeApi.Models;

public sealed class MdxQueryRequest
{
    [Required]
    [MinLength(10)]
    public string Query { get; set; } = string.Empty;
}
