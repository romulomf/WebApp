using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddDbContext<DataContext>(opts => opts.UseSqlServer(builder.Configuration["ConnectionStrings:ProductConnection"]).EnableSensitiveDataLogging(true));

// o método AddControllers adiciona o suporte a controllers de serviço (que servem para APIs REST, por exemplo).
builder.Services.AddControllers()
	// adiciona suporte à respostas em application/json
	.AddNewtonsoftJson()
	// habilita as respostas também em application/xml
	.AddXmlDataContractSerializerFormatters();

// adiciona o suporte a controllers que exibem visualizações em HTML, que podem ser usados com a View Engine.
builder.Services.AddControllersWithViews();

// o cache é uma configuração que é também necessária quando for desejado trabalhar com sessão.
builder.Services.AddDistributedMemoryCache();

builder.Services.AddRateLimiter(opts =>
{
	opts.AddFixedWindowLimiter("fixedWindow", fixOpts =>
	{
		fixOpts.QueueLimit = 0;
		fixOpts.PermitLimit = 1;
		fixOpts.Window = TimeSpan.FromSeconds(15);
	});
});

// adiciona o suporte à páginas razor
builder.Services.AddRazorPages();

// esta configuração, adiciona além das convenções de rotas das páginas razor, um novo padrão de rota que atende uma rota da convenção /Index
builder.Services.Configure<RazorPagesOptions>(opts => opts.Conventions.AddPageRoute("/Index", "/extra/page/{id:long?}"));

// configura que é necessário o uso do cookie para trabalhar com a sessão.
builder.Services.AddSession(opts => opts.Cookie.IsEssential = true);

//builder.Services.Configure<JsonOptions>(opts => opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault);

builder.Services.Configure<MvcNewtonsoftJsonOptions>(opts => opts.SerializerSettings.NullValueHandling = NullValueHandling.Ignore);

builder.Services.Configure<MvcOptions>(opts =>
{
	/*
	 * esta configuração serve para dizer ao ASP.NET Core que ele deve respeitar
	 * o cabeçalho HTTP Accept que o cliente envia na requisição, para determinar
	 * o tipo de conteúdo que ele gostaria de receber. Sem esta configuração, por
	 * padrão, o ASP.NET Core costuma sempre enviar application/json, mesmo que
	 * o cliente especifique um outro tipo de conteúdo preferencialmente, aliado
	 * a um * / * de fallback, tal como Accept: "application/xml,* / *";
	 */
	opts.RespectBrowserAcceptHeader = true;
	/*
	 * esta configuração permite que seja enviada uma resposta HTTP 406 Not Acceptable
	 * quando cliente solicitar um mime type no cabeçalho Accept que não seja suportado
	 * na resposta que o ASP.NET Core envia para a requisição. Caso esta configuração
	 * não seja ativada, isto nunca aconteceria, pois o ASP.NET Core sempre vai tentar
	 * enviar um application/json como fallback.
	 */
	opts.ReturnHttpNotAcceptable = true;
});

builder.Services.AddOutputCache(opts =>
{
	opts.AddPolicy("30sec", policy =>
	{
		policy.Cache();
		policy.Expire(TimeSpan.FromSeconds(30d));
	});
});

builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApp", Version = "v1" }));

var app = builder.Build();

app.UseMiddleware<TestMiddleware>();

app.UseRateLimiter();
app.UseOutputCache();
app.UseStaticFiles();
// registra o middleware que faz o trabalho do controle de sessão.
app.UseSession();
app.MapControllers();
// adiciona o suporte ao roteamento de URLs às páginas razor.
app.MapRazorPages();

app.MapControllerRoute("Default", "{controller=Home}/{action=Index}/{id?}");

app.UseSwagger();
app.UseSwaggerUI(opts => opts.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApp"));

var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedDatabase(context);

app.Run();