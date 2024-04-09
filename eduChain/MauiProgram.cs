using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using UraniumUI;
using UraniumUI.Icons.FontAwesome;
using Firebase;
using Firebase.Auth;
using Firebase.Auth.Providers;
using eduChain.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using eduChain.Models;
using Supabase;
using eduChain.Views.ContentPages;
using epj.RouteGenerator;
using Plugin.Maui.Audio;

namespace eduChain
{
	[AutoRoutes("Page")]

	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiCommunityToolkit()
				.UseMauiApp<App>()
				.UseUraniumUI()
				.UseUraniumUIMaterial()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
					fonts.AddMaterialIconFonts(); // 👈 Add this line
				});
				
	
#if DEBUG
			builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<ISupabaseClientFactory, SupabaseClientFactory>();
        	builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Supabase"));
			builder.Services.AddSingleton<ISupabaseConnection>(new DatabaseConnection("User Id=postgres.wcbvpqecetfhnfphtmae;Password=notthatexcellent3224;Server=aws-0-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;"));
			builder.Services.AddSingleton(AudioManager.Current);
			builder.Services.AddTransient<InitializingPage>();
			builder.Services.AddFilePicker();
			builder.ConfigureFilePicker(100);
			var app = builder.Build();
			return app;
		}
	}

}