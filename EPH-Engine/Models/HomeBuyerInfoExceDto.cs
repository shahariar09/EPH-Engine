namespace EPH_Engine.Models
{
    public class HomeBuyerInfoExceDto
    {
        public int ID { get; set; }
        public double GrossMonthlyIncome { get; set; }
        public double CreditCardPayment { get; set; }
        public double CarPayment { get; set; }

        public double StudentLoanPayments { get; set; }
        public double AppraisedValue { get; set; }
        public double DownPayment { get; set; }
        public double LoanAmount { get; set; }
        public double MonthlyMortgagePayment { get; set; }
        public double CreditScore { get; set; }
    }
}
