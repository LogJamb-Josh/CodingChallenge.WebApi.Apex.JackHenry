namespace WebApiApex.Models
{
    public class NYCDataNodeModel
    {

        /// <summary>
        /// 9 = fiscal year
        /// </summary>
        public int FiscalYear { get; set; }

        /// <summary>
        /// 10 = dept. id
        /// </summary>
        public int DeptId { get; set; }

        /// <summary>
        /// 11 = dept. name
        /// </summary>
        public string? DeptName { get; set; }

        /// <summary>
        /// 12 = funds available
        /// </summary>
        public int FundsAvailable { get; set; }

        /// <summary>
        /// 13 = funds used
        /// </summary>
        public int FundsUsed { get; set; }

        /// <summary>
        /// 14 = remarks
        /// </summary>
        public string? Remarks { get; set; }
    }
}
