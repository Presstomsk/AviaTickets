

namespace AviaTickets.Statuses
{
    public class Result
    {
        public bool Success { get; set; } = false;
        public object? Content { get; set; }
    }
    public enum STATUS
    {
        DONE,       
        ERROR
    }
}
