using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using UraniumUI;
using UraniumUI.Icons.FontAwesome;
using Firebase;
using Firebase.Auth;
using Firebase.Auth.Providers;
using eduChain.ViewModels;

namespace eduChain
{

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
					fonts.AddFontAwesomeIconFonts(); // 👈 Add this line

				});

#if DEBUG
			builder.Logging.AddDebug();
#endif
			builder.Services.AddSingleton<IFirebaseAuthService, FirebaseAuthService>();

			return builder.Build();
		}
	}

}