using eduChain.Pages;
namespace eduChain;

public partial class App : Application
{
	public App()
	{
		Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzE4NDYyOEAzMjM1MmUzMDJlMzBpRkVRekI4eWhQZi92U1J5dnB1RE9UTmhUVWhsUjBTVGc1akF5cUxSM25RPQ==;MzE4NDYyOUAzMjM1MmUzMDJlMzBIUjRMclc0L0tvRGgrZW05dXJGdEJ4ck1xazYreVBTWGUzeGJJQTdtVi9FPQ==;Mgo+DSMBaFt6QHFqVkNrXVNbdV5dVGpAd0N3RGlcdlR1fUUmHVdTRHRbQl5hT35Vc0BgX3tYeXc=;Mgo+DSMBPh8sVXJxS0d+X1RPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9nSX1RcEVlXHpddnBdRGM=;NRAiBiAaIQQuGjN/V0N+XU9Hc1RDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3pTdUdlWXlbdXdTQmlZVA==;MzE4NDYzM0AzMjM1MmUzMDJlMzBKZHFDd2UvV29SVWRwUUoyVEZVSmNxV3hIOW1jeEhIK01URFI5WDhzRjF3PQ==;Mgo+DSMBMAY9C3t2UFhhQlJBfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hTX5QdENjX3pbc3JQRGVZ;MzE4NDYzNUAzMjM1MmUzMDJlMzBBUkpOWTJGMlJIRndXT29nMlNtOHd3c2FqaVcvVk0yVVhtOVJOdk9RZDl3PQ==;MzE4NDYzNkAzMjM1MmUzMDJlMzBVbDdPb0RqUFltMGNDaHdLMUNFQy83azE3T3c2SUcrKzhCR3JoT2YwbjM0PQ==;MzE4NDYzN0AzMjM1MmUzMDJlMzBKZHFDd2UvV29SVWRwUUoyVEZVSmNxV3hIOW1jeEhIK01URFI5WDhzRjF3PQ==");
		InitializeComponent();
		MainPage = new AppShell();
		LoginView loginPage = new LoginView();
    	MainPage.Navigation.PushAsync(loginPage);
	}
}