using Syncfusion.Licensing;
using eduChain.Views;

namespace eduChain;
public partial class App : Application
{
	public App()
	{
		Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzE4OTMwMEAzMjM1MmUzMDJlMzBXWGVuME9SQUFqTXdGQUhZZmtZMXNrM0ViZjJvVGwxa2VLWCtXdVNhd2ZvPQ==");
		InitializeComponent();
		
		MainPage = new AppShell();


	}
}