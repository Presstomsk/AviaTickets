using System.Windows;

namespace AviaTickets
{
    public partial class App : Application
    {
        private void App_Startup(object sender, StartupEventArgs e)
        {
            new Program();
        }
    }
}
