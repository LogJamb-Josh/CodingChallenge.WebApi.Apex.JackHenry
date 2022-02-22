using Newtonsoft.Json;
using System.Linq;
using WebApiApex.DTO;
using WebApiApex.Models;

namespace WebApiApex.Services
{
    public class ServiceNYCData
    {
        //Fields
        private HttpClient _ClientMockBin;
        private readonly string _EndpointUri = "/bin/fb525688-91a7-47da-a319-fcfc24a14001";
        private readonly List<NYCDataNodeModel> _NYCData = new List<NYCDataNodeModel>();
        private bool _gotData = false;


        //Properties        
        public List<NYCDataNodeModel> NYCData
        {
            get
            {
                if (_gotData == false)
                {
                    RefreshData();
                }
                return _NYCData;
            }
        }


        //Constructor
        public ServiceNYCData(IHttpClientFactory httpClientFactory)
        {
            _ClientMockBin = httpClientFactory.CreateClient("MockBinClient");
        }


        //Public Methods

        /// <summary>
        /// 1. return departments whose expenses meet or exceed their funding
        /// </summary>
        /// <returns></returns>
        public List<DepartmentsExpensesOverFundingResponseModel> DepartmentsExpensesOverFunding()
        {
            return NYCData
                .Where(_ => _.FundsUsed >= _.FundsAvailable)
                .Select(_ => new DepartmentsExpensesOverFundingResponseModel() { DeptId = _.DeptId, DeptName = _.DeptName })
                .ToList();
        }

        /// <summary>
        /// 2. return deparments whose expenses have increased over time by user specified percentage (int) and # of years (int)
        /// </summary>
        /// <param name="percentIncreaseFilter"></param>
        /// <param name="numberOfYearsFilter"></param>
        /// <returns></returns>
        public List<DepartmentsExpensesIncreasedResponseModel> DepartmentsExpensesIncreased(int percentIncreaseFilter, int numberOfYearsFilter)
        {
            //First, join the results to itself to get the set that is numberOfYearsFilter apart.  Filter out DivideByZero errors.
            var early = NYCData.Where(_ => _.FundsUsed != 0).ToList();
            var late = NYCData.Where(_ => _.FundsUsed != 0).ToList();

            //Join on DeptId.  That gets you the combinations.
            //While I'm at it, calculate the percentDiff.
            return early.Join(late,
                early => early.DeptId,
                late => late.DeptId,
                (early, late) => new { 
                    Early = early, 
                    Late = late, 
                    percentExpenseIncrease = (int)Math.Round((Decimal)((Decimal)((late.FundsUsed - early.FundsUsed) / early.FundsUsed) * 100), 0) 
                })

                //Filter by matches with the right year gap.
                .Where(_ => _.Late.FiscalYear - _.Early.FiscalYear == numberOfYearsFilter)

                //Filter by matches with the right percentage filter.
                .Where(_ => _.percentExpenseIncrease == percentIncreaseFilter)

                //Specify what values to keep.
                .Select(_ => new DepartmentsExpensesIncreasedResponseModel() {
                    DeptId = _.Early.DeptId,
                    DeptName = _.Early.DeptName,
                    EarlyFiscalYear = _.Early.FiscalYear,
                    EarlyFundsUsed = _.Early.FundsUsed,
                    LastFiscalYear = _.Late.FiscalYear,
                    LateFundsUsed = _.Late.FundsUsed,
                    YearsDifferent = _.Late.FiscalYear - _.Early.FiscalYear,
                    PercentExpenseIncrease = _.percentExpenseIncrease
                })
                .ToList();
        }

        //   3. return departments whose expenses are a user specified percentage below their funding year over year.
        public List<DepartmentsExpensesBelowFundingResponseModel> DepartmentsExpensesBelowFunding(int belowFundingPercentageFilter)
        {
            //First, join the results to itself to get the set that is numberOfYearsFilter apart.  Filter out DivideByZero errors.
            var early = NYCData.Where(_ => _.FundsAvailable != 0).ToList();
            var late = NYCData.Where(_ => _.FundsAvailable != 0).ToList();

            //Join on DeptId.  That gets you the combinations.
            //While I'm at it, calculate the percentDiff.
            return early.Join(late,
                early => early.DeptId,
                late => late.DeptId,
                (early, late) => new
                {
                    Early = early,
                    Late = late,
                    fundingDecreasePercentage = (int)Math.Round((Decimal)((Decimal)((Decimal)(early.FundsAvailable - late.FundsAvailable) / early.FundsAvailable) * 100), 0)
                })

                //Filter by matches with the right year gap.
                .Where(_ => _.Late.FiscalYear - _.Early.FiscalYear == 1)

                //Filter by matches with the right percentage filter.
                .Where(_ => _.fundingDecreasePercentage == belowFundingPercentageFilter)

                //Specify what values to keep.
                .Select(_ => new DepartmentsExpensesBelowFundingResponseModel()
                {
                    DeptId = _.Early.DeptId,
                    DeptName = _.Early.DeptName,
                    EarlyFiscalYear = _.Early.FiscalYear,
                    EarlyFundsAvailable = _.Early.FundsUsed,
                    LastFiscalYear = _.Late.FiscalYear,
                    LateFundsAvailable = _.Late.FundsUsed,                    
                    FundingDecreasePercentage = _.fundingDecreasePercentage
                })
                .ToList();
        }


        //Private Methods
        private void RefreshData()
        {
            _gotData = true;

            //Get the data as a string from the cloud and cast it to dirty data so I can work with it.
            var dirtyData = JsonConvert.DeserializeObject<DirtyDataModel>(_ClientMockBin.GetStringAsync(_EndpointUri).Result);

            //For each dirty object in the list, map it to a clean model.
            foreach (var item in dirtyData?.data)
            {
                NYCData.Add(new NYCDataNodeModel()
                {
                    FiscalYear = getIntFromObject(item[9]),
                    DeptId = getIntFromObject(item[10]),
                    DeptName = item[11].ToString(),
                    FundsAvailable = getIntFromObject(item[12]),
                    FundsUsed = getIntFromObject(item[13]),
                    Remarks = item[14]?.ToString()
                });
            }

        }

        /// <summary>
        /// Takes in an object expected to be converted to an int, and does the conversion.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private int getIntFromObject(object v)
        {
            var r = 0;

            if (v != null && !int.TryParse(v.ToString(), out r))
            {
                //Value can't become an int.  Maybe do something.
            }

            return r;
        }
    }


}
