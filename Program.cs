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

// o m�todo AddControllers adiciona o suporte a controllers de servi�o (que servem para APIs REST, por exemplo).
builder.Services.AddControllers()
	// adiciona suporte � respostas em application/json
	.AddNewtonsoftJson()
	// habilita as respostas tamb�m em application/xml
	.AddXmlDataContractSerializerFormatters();

// adiciona o suporte a controllers que exibem visualiza��es em HTML, que podem ser usados com a View Engine.
builder.Services.AddControllersWithViews();

// o cache � uma configura��o que � tamb�m necess�ria quando for desejado trabalhar com sess�o.
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

// adiciona o suporte � p�ginas razor
builder.Services.AddRazorPages();

// esta configura��o, adiciona al�m das conven��es de rotas das p�ginas razor, um novo padr�o de rota que atende uma rota da conven��o /Index
builder.Services.Configure<RazorPagesOptions>(opts => opts.Conventions.AddPageRoute("/Index", "/extra/page/{id:long?}"));

// configura que � necess�rio o uso do cookie para trabalhar com a sess�o.
builder.Services.AddSession(opts => opts.Cookie.IsEssential = true);

//builder.Services.Configure<JsonOptions>(opts => opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault);

builder.Services.Configure<MvcNewtonsoftJsonOptions>(opts => opts.SerializerSettings.NullValueHandling = NullValueHandling.Ignore);

builder.Services.Configure<MvcOptions>(opts =>
{
	/*
	 * esta configura��o serve para dizer ao ASP.NET Core que ele deve respeitar
	 * o cabe�alho HTTP Accept que o cliente envia na requisi��o, para determinar
	 * o tipo de conte�do que ele gostaria de receber. Sem esta configura��o, por
	 * padr�o, o ASP.NET Core costuma sempre enviar application/json, mesmo que
	 * o cliente especifique um outro tipo de conte�do preferencialmente, aliado
	 * a um * / * de fallback, tal como Accept: "application/xml,* / *";
	 */
	opts.RespectBrowserAcceptHeader = true;
	/*
	 * esta configura��o permite que seja enviada uma resposta HTTP 406 Not Acceptable
	 * quando cliente solicitar um mime type no cabe�alho Accept que n�o seja suportado
	 * na resposta que o ASP.NET Core envia para a requisi��o. Caso esta configura��o
	 * n�o seja ativada, isto nunca aconteceria, pois o ASP.NET Core sempre vai tentar
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
// registra o middleware que faz o trabalho do controle de sess�o.
app.UseSession();
app.MapControllers();
// adiciona o suporte ao roteamento de URLs �s p�ginas razor.
app.MapRazorPages();

app.MapControllerRoute("Default", "{controller=Home}/{action=Index}/{id?}");

app.UseSwagger();
app.UseSwaggerUI(opts => opts.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApp"));

var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedDatabase(context);

app.Run();