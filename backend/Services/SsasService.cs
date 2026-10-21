using System.Data;
using System.Globalization;
using System.Reflection;
using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.Extensions.Options;
using SalesCubeApi.Models;
using SalesCubeApi.Options;

namespace SalesCubeApi.Services;

public sealed class SsasService : ISsasService
{
    private readonly SsasOptions _options;
    private readonly ILogger<SsasService> _logger;

    public SsasService(IOptions<SsasOptions> options, ILogger<SsasService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task<SsasConnectionStatusDto> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                using var connection = CreateConnection();
                connection.Open();

                return new SsasConnectionStatusDto(
                    IsConnected: true,
                    ServerVersion: TryGet(() => connection.ServerVersion),
                    Database: TryGet(() => connection.Database),
                    Message: "Connexion SSAS réussie.",
                    CheckedAtUtc: DateTime.UtcNow
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur de connexion au cube SSAS.");

                return new SsasConnectionStatusDto(
                    IsConnected: false,
                    ServerVersion: null,
                    Database: null,
                    Message: "Connexion SSAS indisponible. Vérifiez la configuration du backend et ses journaux.",
                    CheckedAtUtc: DateTime.UtcNow
                );
            }
        }, cancellationToken);
    }

    public Task<decimal> ExecuteScalarDecimalAsync(string mdx, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var table = ExecuteTableInternal(mdx, cancellationToken);

            foreach (var row in table.Rows)
            {
                foreach (var value in row.Values)
                {
                    if (TryToDecimal(value, out var result))
                    {
                        return result;
                    }
                }
            }

            return 0m;
        }, cancellationToken);
    }

    public Task<string?> ExecuteFirstLabelAsync(string mdx, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var table = ExecuteTableInternal(mdx, cancellationToken);
            var firstRow = table.Rows.FirstOrDefault();

            if (firstRow is null)
            {
                return null;
            }

            foreach (var value in firstRow.Values)
            {
                if (value is null)
                {
                    continue;
                }

                if (!TryToDecimal(value, out _))
                {
                    var label = Convert.ToString(value, CultureInfo.InvariantCulture);
                    if (!string.IsNullOrWhiteSpace(label))
                    {
                        return label;
                    }
                }
            }

            return firstRow.Values.FirstOrDefault()?.ToString();
        }, cancellationToken);
    }

    public Task<MdxTableResponse> ExecuteTableAsync(string mdx, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => ExecuteTableInternal(mdx, cancellationToken), cancellationToken);
    }

    public Task<MdxTableResponse> GetCubesAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() => GetSchemaInternal(AdomdSchemaGuid.Cubes, null, cancellationToken), cancellationToken);
    }

    public Task<MdxTableResponse> GetMeasuresAsync(string? cubeName, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            object?[]? restrictions = string.IsNullOrWhiteSpace(cubeName)
                ? null
                : [null, null, cubeName, null];

            return GetSchemaInternal(AdomdSchemaGuid.Measures, restrictions, cancellationToken);
        }, cancellationToken);
    }

    public Task<MdxTableResponse> GetDimensionsAsync(string? cubeName, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            object?[]? restrictions = string.IsNullOrWhiteSpace(cubeName)
                ? null
                : [null, null, cubeName, null];

            return GetSchemaInternal(AdomdSchemaGuid.Dimensions, restrictions, cancellationToken);
        }, cancellationToken);
    }

    public Task<CubeMetadataExportDto> GetCubeMetadataAsync(string? cubeName, bool includeMembers = false, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => GetCubeMetadataInternal(cubeName, includeMembers, cancellationToken), cancellationToken);
    }

    private MdxTableResponse ExecuteTableInternal(string mdx, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureQueryIsConfigured(mdx);

        using var connection = CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = mdx;
        command.CommandTimeout = _options.CommandTimeoutSeconds;

        using var reader = command.ExecuteReader();
        var columns = GetUniqueColumnNames(reader);
        var rows = new List<Dictionary<string, object?>>();

        while (reader.Read())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < reader.FieldCount; i++)
            {
                row[columns[i]] = NormalizeValue(reader.GetValue(i));
            }

            rows.Add(row);
        }

        return new MdxTableResponse(columns, rows);
    }

    private CubeMetadataExportDto GetCubeMetadataInternal(string? cubeName, bool includeMembers, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var connection = CreateConnection();
        connection.Open();

        var warnings = new List<string>();
        var rowsets = new Dictionary<string, MetadataSchemaTableDto>(StringComparer.OrdinalIgnoreCase);
        var database = TryGet(() => connection.Database);

        object?[]? cubeRestrictions = string.IsNullOrWhiteSpace(cubeName)
            ? null
            : [null, null, cubeName];

        object?[]? cubeRestrictionsWithType = string.IsNullOrWhiteSpace(cubeName)
            ? null
            : [null, null, cubeName, null];

        AddSchemaRowset(connection, rowsets, warnings, "Cubes", "Cubes", cubeRestrictionsWithType, cancellationToken);
        AddSchemaRowset(connection, rowsets, warnings, "Measures", "Measures", cubeRestrictions, cancellationToken);
        AddSchemaRowset(connection, rowsets, warnings, "Dimensions", "Dimensions", cubeRestrictions, cancellationToken);
        AddSchemaRowset(connection, rowsets, warnings, "Hierarchies", "Hierarchies", cubeRestrictions, cancellationToken);
        AddSchemaRowset(connection, rowsets, warnings, "Levels", "Levels", cubeRestrictions, cancellationToken);
        AddSchemaRowset(connection, rowsets, warnings, "Properties", "Properties", cubeRestrictions, cancellationToken);
        AddSchemaRowset(connection, rowsets, warnings, "Kpis", "Kpis", cubeRestrictions, cancellationToken);
        AddSchemaRowset(connection, rowsets, warnings, "Sets", "Sets", cubeRestrictions, cancellationToken);
        AddSchemaRowset(connection, rowsets, warnings, "Actions", "Actions", cubeRestrictions, cancellationToken);
        AddSchemaRowset(connection, rowsets, warnings, "MeasureGroups", "MeasureGroups", cubeRestrictions, cancellationToken);
        AddSchemaRowset(connection, rowsets, warnings, "MeasureGroupDimensions", "MeasureGroupDimensions", cubeRestrictions, cancellationToken);
        AddSchemaRowset(connection, rowsets, warnings, "NamedSets", "NamedSets", cubeRestrictions, cancellationToken);

        if (includeMembers)
        {
            warnings.Add("Members can be very large. Use includeMembers=true only when you really need level/member values.");
            AddSchemaRowset(connection, rowsets, warnings, "Members", "Members", cubeRestrictions, cancellationToken);
        }
        else
        {
            warnings.Add("Members rowset skipped to avoid returning a very large JSON. Call /api/metadata/cube?includeMembers=true to include it.");
        }

        return new CubeMetadataExportDto(
            RequestedCubeName: string.IsNullOrWhiteSpace(cubeName) ? null : cubeName,
            Database: database,
            GeneratedAtUtc: DateTime.UtcNow,
            IncludeMembers: includeMembers,
            Warnings: warnings,
            Rowsets: rowsets
        );
    }

    private void AddSchemaRowset(
        AdomdConnection connection,
        IDictionary<string, MetadataSchemaTableDto> rowsets,
        ICollection<string> warnings,
        string outputName,
        string adomdSchemaGuidName,
        object?[]? restrictions,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var schemaGuid = TryGetAdomdSchemaGuid(adomdSchemaGuidName);
        if (schemaGuid is null)
        {
            var warning = $"ADOMD schema rowset '{adomdSchemaGuidName}' is not available in the installed client library.";
            warnings.Add(warning);
            rowsets[outputName] = new MetadataSchemaTableDto(outputName, [], [], 0, warning);
            return;
        }

        try
        {
            var table = GetSchemaInternal(connection, schemaGuid.Value, restrictions, cancellationToken);
            rowsets[outputName] = new MetadataSchemaTableDto(outputName, table.Columns, table.Rows, table.Rows.Count, null);
        }
        catch (Exception ex)
        {
            var message = $"Failed to read schema rowset '{outputName}': {ex.Message}";
            _logger.LogWarning(ex, "Failed to read SSAS schema rowset {SchemaRowset}.", outputName);
            warnings.Add(message);
            rowsets[outputName] = new MetadataSchemaTableDto(outputName, [], [], 0, message);
        }
    }

    private MdxTableResponse GetSchemaInternal(Guid schemaGuid, object?[]? restrictions, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var connection = CreateConnection();
        connection.Open();

        return GetSchemaInternal(connection, schemaGuid, restrictions, cancellationToken);
    }

    private static MdxTableResponse GetSchemaInternal(AdomdConnection connection, Guid schemaGuid, object?[]? restrictions, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var dataSet = connection.GetSchemaDataSet(schemaGuid, restrictions);
        if (dataSet.Tables.Count == 0)
        {
            return new MdxTableResponse([], []);
        }

        return ConvertDataTable(dataSet.Tables[0]);
    }

    private AdomdConnection CreateConnection()
    {
        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            throw new InvalidOperationException("La chaîne de connexion SSAS est vide. Définissez Ssas__ConnectionString ou un fichier appsettings.Local.json non versionné.");
        }

        return new AdomdConnection(_options.ConnectionString);
    }

    private static MdxTableResponse ConvertDataTable(DataTable table)
    {
        var columns = table.Columns.Cast<DataColumn>()
            .Select(column => column.ColumnName)
            .ToList();

        var rows = new List<Dictionary<string, object?>>();

        foreach (DataRow dataRow in table.Rows)
        {
            var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            foreach (DataColumn column in table.Columns)
            {
                row[column.ColumnName] = NormalizeValue(dataRow[column]);
            }

            rows.Add(row);
        }

        return new MdxTableResponse(columns, rows);
    }

    private static IReadOnlyList<string> GetUniqueColumnNames(AdomdDataReader reader)
    {
        var columns = new List<string>();
        var used = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < reader.FieldCount; i++)
        {
            var rawName = reader.GetName(i);
            var baseName = string.IsNullOrWhiteSpace(rawName) ? $"Column{i + 1}" : rawName.Trim();

            if (!used.TryAdd(baseName, 1))
            {
                used[baseName]++;
                baseName = $"{baseName}_{used[baseName]}";
            }

            columns.Add(baseName);
        }

        return columns;
    }

    private static object? NormalizeValue(object? value)
    {
        if (value is null || value == DBNull.Value)
        {
            return null;
        }

        return value switch
        {
            DateTime date => date,
            decimal number => number,
            double number => Convert.ToDecimal(number, CultureInfo.InvariantCulture),
            float number => Convert.ToDecimal(number, CultureInfo.InvariantCulture),
            int number => number,
            long number => number,
            short number => number,
            byte number => number,
            bool boolean => boolean,
            _ => Convert.ToString(value, CultureInfo.InvariantCulture)
        };
    }

    private static bool TryToDecimal(object? value, out decimal result)
    {
        result = 0m;

        if (value is null || value == DBNull.Value)
        {
            return false;
        }

        try
        {
            switch (value)
            {
                case decimal number:
                    result = number;
                    return true;
                case double number:
                    result = Convert.ToDecimal(number, CultureInfo.InvariantCulture);
                    return true;
                case float number:
                    result = Convert.ToDecimal(number, CultureInfo.InvariantCulture);
                    return true;
                case int number:
                    result = number;
                    return true;
                case long number:
                    result = number;
                    return true;
                case short number:
                    result = number;
                    return true;
                case byte number:
                    result = number;
                    return true;
                case string text:
                    return decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result)
                        || decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out result);
                default:
                    result = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                    return true;
            }
        }
        catch
        {
            return false;
        }
    }

    private static string? TryGet(Func<string?> getter)
    {
        try
        {
            return getter();
        }
        catch
        {
            return null;
        }
    }

    private static Guid? TryGetAdomdSchemaGuid(string name)
    {
        var flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase;

        var property = typeof(AdomdSchemaGuid).GetProperty(name, flags);
        if (property?.GetValue(null) is Guid propertyGuid)
        {
            return propertyGuid;
        }

        var field = typeof(AdomdSchemaGuid).GetField(name, flags);
        if (field?.GetValue(null) is Guid fieldGuid)
        {
            return fieldGuid;
        }

        return null;
    }

    private static void EnsureQueryIsConfigured(string mdx)
    {
        if (string.IsNullOrWhiteSpace(mdx))
        {
            throw new InvalidOperationException("La requête MDX est vide. Vérifie la section MdxQueries dans appsettings.json.");
        }
    }
}
