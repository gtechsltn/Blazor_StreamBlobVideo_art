using Blazor_VideoStreamBlob.Components;
using Blazor_VideoStreamBlob.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

// registerthe BLOBConnect service

builder.Services.AddSingleton<BLOBConnect>();



// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.MapGet("/api/getblob/{blobName}", async (BLOBConnect blobConnect, string blobName) =>
{
     var stream = await blobConnect.GetBlobFileStream(blobName);
     return Results.File(stream, "video/mp4");
});
app.Run();
