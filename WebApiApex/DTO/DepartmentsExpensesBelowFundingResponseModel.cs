namespace WebApiApex.DTO
{
    public class DepartmentsExpensesBelowFundingResponseModel
    {
        public int DeptId { get; set; }
        public string? DeptName { get; set; }
        public int EarlyFiscalYear { get; set; }
        public int EarlyFundsAvailable { get; set; }
        public int LastFiscalYear { get; set; }
        public int LateFundsAvailable { get; set; }
        public int FundingDecreasePercentage { get; set; }

    }
}
