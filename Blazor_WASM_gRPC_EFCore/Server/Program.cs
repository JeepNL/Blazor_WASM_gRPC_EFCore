using Blazor_WASM_gRPC_EFCore.Server.Data;
using Blazor_WASM_gRPC_EFCore.Server.Services;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlite(connectionString!));

builder.Services.AddW3CLogging(options =>
{
	options.LogDirectory = @"Log";
	options.LoggingFields = W3CLoggingFields.All;
	options.RetainedFileCountLimit = 10;
});

builder.Services.AddGrpc();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.

//Seed data if DB is empty
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	SeedData.Initialize(services);
}

if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseW3CLogging();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.MapControllers();

app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true }); // No need for .EnableGrpcWeb() below.
app.UseEndpoints(endpoints =>
{
	endpoints.MapGrpcService<BlogService>().EnableGrpcWeb();
});

app.MapFallbackToFile("index.html");

app.Run();
