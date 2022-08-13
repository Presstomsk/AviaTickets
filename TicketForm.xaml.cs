using AviaTickets.ViewModel;
using System.Windows.Controls;


namespace AviaTickets
{
    /// <summary>
    /// Логика взаимодействия для TicketForm.xaml
    /// </summary>
    public partial class TicketForm : UserControl
    {     


        public TicketForm()
        {
            InitializeComponent();
            DataContext = new TicketUserControl();
        }
        

    }
}
