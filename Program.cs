using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddDbContext<DataContext>(opts => opts.UseSqlServer(builder.Configuration["ConnectionStrings:ProductConnection"]).EnableSensitiveDataLogging(true));

builder.Services.AddControllers()
	// adiciona suporte � respostas em application/json
	.AddNewtonsoftJson()
	// habilita as respostas tamb�m em application/xml
	.AddXmlDataContractSerializerFormatters();

builder.Services.AddRateLimiter(opts =>
{
	opts.AddFixedWindowLimiter("fixedWindow", fixOpts =>
	{
		fixOpts.QueueLimit = 0;
		fixOpts.PermitLimit = 1;
		fixOpts.Window = TimeSpan.FromSeconds(15);
	});
});

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
app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.UseSwagger();
app.UseSwaggerUI(opts => opts.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApp"));

var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedDatabase(context);

app.Run();