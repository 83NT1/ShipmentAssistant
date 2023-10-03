namespace ShipmentAssistant.Models
{
    public class HistoryLog
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string User { get; set; }
        public string Code { get; set; }
        public int UnitsRecived { get; set; }

        public HistoryLog(DateTime date, string user, string code, int unitsRecived)
        {
            Date = date;
            User = user;
            Code = code;
            UnitsRecived = unitsRecived;
        }
    }
}
