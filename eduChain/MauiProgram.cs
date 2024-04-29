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
using eduChain.Views.ContentPages.ProfileViews;
using epj.RouteGenerator;
using Plugin.Maui.Audio;
using Mopups.Hosting;
using eduChain.Models;
using ZXing.Net.Maui.Controls;
using CommunityToolkit.Maui.Storage;
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
				.ConfigureMopups()
				.UseUraniumUI()
				.UseUraniumUIMaterial()
				.UseUraniumUIBlurs()
				.UseBarcodeReader()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
					fonts.AddMaterialIconFonts(); // 👈 Add this line
							fonts.AddFontAwesomeIconFonts(); // 👈 Add this line

				});
				
	
#if DEBUG
			builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<ISupabaseClientFactory, SupabaseClientFactory>();
			builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);
        	builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Supabase"));
			builder.Services.AddSingleton<ISupabaseConnection>(new DatabaseConnection("User Id=postgres.wcbvpqecetfhnfphtmae;Password=notthatexcellent3224;Server=aws-0-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;"));
			builder.Services.AddSingleton(AudioManager.Current);
			builder.Services.AddSingleton<StudentProfileModel>();
			builder.Services.AddTransient<InitializingPage>();
			builder.Services.AddTransient<RegisterPage>();
			builder.Services.AddTransient<StudentProfilePage>();
			builder.Services.AddCommunityToolkitDialogs();
			builder.Services.AddMopupsDialogs();
			builder.Services.AddFilePicker();
			builder.ConfigureFilePicker(100);
			var app = builder.Build();
			return app;
		}
	}

}