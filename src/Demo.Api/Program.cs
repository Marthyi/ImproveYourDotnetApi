using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenTelemetry.Trace;

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

//builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();// https://referbruv.com/blog/posts/integrating-aspnet-core-api-versions-with-swagger-ui
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();


// Add Open Telemetry
builder.Services.AddOpenTelemetryTracing((builder) => builder
            .AddConsoleExporter()
           //.AddAspNetCoreInstrumentation()
           //.AddHttpClientInstrumentation()
           //.AddZipkinExporter(zipkinOptions =>
           //{
           //    zipkinOptions.Endpoint = new Uri(Configuration.GetConnectionString("zipkin"));
           //})
           );

builder.Services.AddScoped<Transformer>();

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
//app.UseAuthorization();




//app.MapControllers();

app.UseRouting();

//app.UseEndpoints(endpoints =>
//{
// //   endpoints.MapControllers();
// endpoints.MapControllerRoute("name","api/toto/{**path}",)
//    endpoints.MapDynamicControllerRoute<Transformer>("api/v{version:apiVersion}/{**path}");
//});


//if (app.Environment.IsDevelopment())
//{
//    app.MapGet("/debug/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
//        string.Join("\n", endpointSources.SelectMany(source => source.Endpoints)));
//}


app.Run();


public class Transformer : DynamicRouteValueTransformer
{

    public Transformer()
    {
     
    }

    public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
    {        
        if(values != null)
        {
            if (values.ContainsKey("action")) values["action"] = "GetTasks";
            else values.Add("action", "GetTasks");

            if (values.ContainsKey("controller")) values["controller"] = "TaskControllerV";
            else values.Add("controller", "TaskControllerV");

            //if (values.ContainsKey("apiVersion")) values["apiVersion"] = 2;
            //else values.Add("apiVersion", 2);

            //if (values.ContainsKey("version")) values["version"] = 2;
            //else values.Add("version", 2);

            if (values.ContainsKey("{ version: apiVersion}")) values["{ version: apiVersion}"] = 2;
            else values.Add("{ version: apiVersion}", 3);

            //{ version: apiVersion}

            return values;
        }


        return new RouteValueDictionary()
            {
                { "action", "GetTasks" },
                { "controller", "TaskControllerV" },
                { "apiVersion", "2" },
            };
    }
}