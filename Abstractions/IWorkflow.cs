using AviaTickets.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AviaTickets.Abstractions
{
    public interface IWorkflow
    {
        string WorkflowType { get; set; }
        public void Start();
    }
}
