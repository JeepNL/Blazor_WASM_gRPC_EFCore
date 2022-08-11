using Blazor_WASM_gRPC_EFCore.Client;
using Grpc.Net.Client.Web;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// For JSON Controllers / API Back-end
// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// For gRPC
//builder.Services.AddSingleton(services =>
builder.Services.AddScoped(services =>
{
	IConfiguration? configuration = services.GetRequiredService<IConfiguration>();
	string? backendUrl = $"https://{configuration["Settings:BackEndUrl"]}";

	// Create a channel with a GrpcWebHandler that is addressed to the backend server.
	// GrpcWebText is used because server streaming requires it. If server streaming is not used in your app
	// then GrpcWeb is recommended because it produces smaller messages.
	GrpcWebHandler httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
	return GrpcChannel.ForAddress(backendUrl, new GrpcChannelOptions
	{
		HttpHandler = httpHandler
	});
});

await builder.Build().RunAsync();
