using System.Text.Json.Serialization;
using SalesCubeApi.Middlewares;
using SalesCubeApi.Options;
using SalesCubeApi.Repositories;
using SalesCubeApi.Services;
using SalesCubeApi.Services.Analytics;

var builder = WebApplication.CreateBuilder(args);

// Optional machine-specific configuration; keep appsettings.Local.json out of source control.
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.Configure<SsasOptions>(builder.Configuration.GetSection(SsasOptions.SectionName));
builder.Services.Configure<MdxQueriesOptions>(builder.Configuration.GetSection(MdxQueriesOptions.SectionName));

builder.Services.AddScoped<ISsasService, SsasService>();
builder.Services.AddScoped<IAnalyticsRepository, SsasAnalyticsRepository>();
builder.Services.AddScoped<IAnalyticsQueryService, AnalyticsQueryService>();
builder.Services.AddScoped<IDashboardAnalyticsService, DashboardAnalyticsService>();
builder.Services.AddScoped<IKpiAnalyticsService, KpiAnalyticsService>();
builder.Services.AddScoped<ISalesAnalyticsService, SalesAnalyticsService>();
builder.Services.AddScoped<IProductAnalyticsService, ProductAnalyticsService>();
builder.Services.AddScoped<ICustomerAnalyticsService, CustomerAnalyticsService>();
builder.Services.AddScoped<IEmployeeAnalyticsService, EmployeeAnalyticsService>();
builder.Services.AddScoped<IGeographyAnalyticsService, GeographyAnalyticsService>();
builder.Services.AddScoped<IDimensionAnalyticsService, DimensionAnalyticsService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                    ?? ["http://localhost:4200"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// app.UseHttpsRedirection(); // Disabled for local HTTP profile to avoid Swagger "Failed to fetch" when HTTPS port is not configured.
app.UseCors("AngularPolicy");
app.MapControllers();

app.Run();
