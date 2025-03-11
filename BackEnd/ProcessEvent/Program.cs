var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IStoreEventHubService, StoreEventHubService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "Store EventHub Demo Backend";
    config.Title = "Store EventHub Demo Backend v1";
    config.Version = "v1";
});
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: MyAllowSpecificOrigins,
                          policy =>
                          {
                              policy.WithOrigins("http://localhost:4200").AllowAnyHeader();
                          });
    });

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "Store EventHub Demo Backend";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });

    app.UseCors(MyAllowSpecificOrigins);
}

app.MapGet("/", () => "Hello World!");

app.MapPost("/process-event", async (IStoreEventHubService service, EventPost post) =>
{
    await service.ProcessEvent(post);
    Results.Created($"/process-event/{post.ProductNumber}", post);
}).RequireCors(MyAllowSpecificOrigins);

// app.MapGet("/list-events", async (IStoreEventHubService service, string productNumber) => {
//     return Results.Content(await service.ListEvents(productNumber));
// }).RequireCors(MyAllowSpecificOrigins);

app.Run();