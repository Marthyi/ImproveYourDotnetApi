using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//added  with nuget package "Microsoft.AspNetCore.Mvc.Versioning"
builder.Services.AddApiVersioning(o =>
{
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.ReportApiVersions = true;
    o.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new MediaTypeApiVersionReader("x-api-version")
        );
});

builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();// https://referbruv.com/blog/posts/integrating-aspnet-core-api-versions-with-swagger-ui
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

WebApplication app = builder.Build();
IApiVersionDescriptionProvider apiProvider = app.Services.GetService<IApiVersionDescriptionProvider>();

app.UseMiddleware<ExceptionMiddleware>();
//app.UseHttpLogging();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (ApiVersionDescription description in apiProvider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
    }
});
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
