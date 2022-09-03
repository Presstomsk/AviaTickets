using AviaTickets.Processes.Abstractions;


namespace AviaTickets.Processes
{
    public class CitiesDatabaseUpdateWorkflow : ICitiesDatabaseUpdateWorkflow
    {
        public CitiesDatabaseUpdateWorkflow(string workflowType)
        {
            WorkflowType = workflowType;
        }

        public string WorkflowType { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public (bool, object?) Start()
        {
            throw new System.NotImplementedException();
        }
    }
}
