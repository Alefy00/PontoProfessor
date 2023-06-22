using Ponto.Pages;

namespace Ponto;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new Login();
	}
}
