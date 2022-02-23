namespace WebApiApex.DTO
{
    public class DepartmentsExpensesIncreasedResponseModel
    {
        public int DeptId { get; set; }
        public string? DeptName { get; set; }
        public int EarlyFundsUsed { get; set; }
        public int LateFundsUsed { get; set; }
        public int EarlyFiscalYear { get; set; }
        public int LastFiscalYear { get; set; }
        public int YearsDifferent { get; set; }
        public int PercentExpenseIncrease { get; set; }

    }
}
