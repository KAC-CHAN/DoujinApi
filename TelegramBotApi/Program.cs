using System.Reflection;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using TelegramBotApi.Middlewares;
using TelegramBotApi.Models;
using TelegramBotApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<GlobalExeptionHandlerMiddleware>();
builder.Services.AddTransient<ApiKeyAuthMiddleware>();
builder.Services.Configure<TgBotDatabaseSettings>(
	builder.Configuration.GetSection("TelegramBotDatabase")
);

builder.Services.Configure<TelegraphSettings>(
 	builder.Configuration.GetSection("Telegraph")
);

BsonSerializer.RegisterSerializer(new EnumSerializer<Source>(BsonType.String));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1",
		new()
		{
			Title = "TelegramBotApi", Version = "v1",
			Description = "API to scrape doujins from exhentai and store them in a database.",
			Contact = new()
			{
				Name = "trueimmortal",
				Email = "caelestis.deimos@protonmail.com",
				Url = new Uri("https://github.com/trueimmortal")
			}

		});
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	c.IncludeXmlComments(xmlPath);
	c.AddSecurityDefinition("ApiKey", new()
	{
		Name = "X-Api-Key",
		Description = "API Key",
		Scheme = "ApiKeyScheme",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey
	});
	var scheme = new OpenApiSecurityScheme
	{
		Reference = new OpenApiReference
		{
			Type = ReferenceType.SecurityScheme,
			Id = "ApiKey"
		},
		In = ParameterLocation.Header
	};
	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{scheme, new string[] { }}
		});
});


// Collection services
builder.Services.AddSingleton<DoujinService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<LogService>();
builder.Services.AddSingleton<SettingService>();
builder.Services.AddSingleton<StatsService>();
builder.Services.AddSingleton<TelegraphService>();

// Other services

builder.Services.AddSingleton<LoggerService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExeptionHandlerMiddleware>();
app.UseMiddleware<ApiKeyAuthMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();