using BlazorDownloadFile;
using MarkdownWordpress.ViewModels;
using MudBlazor;
using MudBlazor.Services;
using WordpressToMarkdown;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<IMainViewModel, MainViewModel>();
builder.Services.AddScoped(typeof(WordpressCollector));
builder.Services.AddSingleton<ISuiviDownload, SuiviDownload>();

builder.Services.AddMudServices();
builder.Services.AddMudMarkdownServices();
builder.Services.AddBlazorDownloadFile(ServiceLifetime.Scoped);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
