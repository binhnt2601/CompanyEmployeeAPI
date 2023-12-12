using AspNetCoreRateLimit;
using CompanyEmployees;
using CompanyEmployees.ActionFilters;
using CompanyEmployees.Extensions;
using CompanyEmployees.Utilities;
using Contracts;
using Entities;
using Entities.DTO;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.DataShaping;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging(options =>
{
    options.AddConsole();
    options.AddDebug();
});

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
});
builder.Services.AddDbContext<RepositoryContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), 
        b => b.MigrationsAssembly("CompanyEmployees"));
});
builder.Services.AddAuthentication();
builder.Services.ConfigureJWT(builder.Configuration);

builder.Services.ConfigureIdentity();

builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.Configure<IISOptions>(options => {});
builder.Services.AddScoped(typeof(ValidationFilterAttribute));
builder.Services.AddScoped(typeof(ValidateCompanyExistsAttribute));
builder.Services.AddScoped(typeof(ValidateEmployeeExistsAttribute));
builder.Services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();
builder.Services.AddScoped(typeof(ValidateMediaTypeAttribute));
builder.Services.AddScoped<EmployeeLinks>();
builder.Services.AddScoped<IAuthenticationManager, AuthenticationManager>();

builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
    options.ReturnHttpNotAcceptable = true;
    options.CacheProfiles.Add("DefaultCacheProfile", new CacheProfile
    {
        Duration = 120
    }) ;
}).AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters()
.AddMvcOptions(cfg =>
cfg.OutputFormatters.Add(new CsvOutputFormatter()));
builder.Services.AddResponseCaching();
builder.Services.AddHttpCacheHeaders(
    (expirationOpt) =>
    {
        expirationOpt.MaxAge = 65;
        expirationOpt.CacheLocation = CacheLocation.Private;
    },
    (validationOpt) =>
    {
        validationOpt.MustRevalidate = true;
    });
builder.Services.ConfigureSwagger();
builder.Services.AddCustomMediaTypes();
builder.Services.ConfigureVersioning();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}
app.UseSwagger();
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Code Maze API v1");
    s.SwaggerEndpoint("/swagger/v2/swagger.json", "Code Maze API v2");
});
app.ConfigureExceptionHandler();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("CorsPolicy");
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});
app.UseHttpLogging();
app.UseResponseCaching();
app.UseHttpCacheHeaders();
app.UseIpRateLimiting();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
