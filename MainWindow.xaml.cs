using AviaTickets.ViewModel;
using System.Windows;


namespace AviaTickets
{    
    public partial class MainWindow : Window
    {       


        public MainWindow()
        {
            InitializeComponent();

            DataContext = new AviaTicketsViewModel(this);

        }


    }

}
