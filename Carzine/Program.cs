using Carzine.Auth;
using CarzineCore;
using CarzineCore.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

var allowSpecificOrigins = "_allowSpecificOrigins";

try
{
	var builder = WebApplication.CreateBuilder(args);

	// Add services to the container.

	builder.Services.AddControllersWithViews();

	builder.Services.AddAuthorization();
	builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
		.AddJwtBearer(options =>
		{
			options.TokenValidationParameters = new TokenValidationParameters
			{
				// ���������, ����� �� �������������� �������� ��� ��������� ������
				ValidateIssuer = true,
				// ������, �������������� ��������
				ValidIssuer = AuthOptions.ISSUER,
				// ����� �� �������������� ����������� ������
				ValidateAudience = true,
				// ��������� ����������� ������
				ValidAudience = AuthOptions.AUDIENCE,
				// ����� �� �������������� ����� �������������
				ValidateLifetime = true,
				// ��������� ����� ������������
				IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
				// ��������� ����� ������������
				ValidateIssuerSigningKey = true,
			};
		});

	builder.Services.AddSingleton<IApiDataService, ApiDataService>();
	builder.Services.AddSingleton<IDataTranslatorService, LanguageTranslatorService>();
	builder.Services.AddScoped<IPaymentService, PaymentService>();
	builder.Services.AddTransient<IMailService, MailService>();

	builder.Services.AddSingleton<IDbDataRepository, MySqlDataRepository>();
	builder.Services.AddSingleton<IDbUserService, MySqlDataRepository>();
	builder.Services.AddSingleton<IDbTranslationService, MySqlDataRepository>();
	builder.Services.AddSingleton<IDbActionLogService, MySqlDataRepository>();
	builder.Services.AddSingleton<IOrderRepository, MySqlDataRepository>();

	builder.Services.AddSingleton<IDataCollector, DataCollector>();

	builder.Services.AddCors(options =>
	{
		options.AddPolicy(allowSpecificOrigins, policy => policy.AllowAnyOrigin());
	});

	//builder.Logging.AddConsole();
	builder.Logging.ClearProviders();
	builder.Host.UseNLog();

	var app = builder.Build();

	// Configure the HTTP request pipeline.
	if (!app.Environment.IsDevelopment())
	{
		// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
		app.UseHsts();
	}

	app.UseHttpsRedirection();
	app.UseStaticFiles();
	app.UseRouting();

	app.UseAuthentication();
	app.UseAuthorization();

	app.UseCors(allowSpecificOrigins);

	app.MapControllerRoute(
	name: "default",
	pattern: "{controller}/{action=Index}/{id?}");

	app.MapFallbackToFile("index.html");

	app.Run();
}
catch (Exception ex)
{
	logger.Error(ex, "Stopped program because of exception");
	throw;
}
finally
{
	LogManager.Shutdown();
}
