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
            //Get the HttpClient that was DI in the Program.cs
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
                .Select(_ => new DepartmentsExpensesOverFundingResponseModel()
                {
                    DeptId = _.DeptId,
                    DeptName = _.DeptName
                })
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
            var cleanData = NYCData.Where(_ => _.FundsUsed != 0).ToList();

            //Join on DeptId.  That gets you the combinations.
            //While I'm at it, calculate the percentDiff.
            return cleanData.Join(cleanData,
                early => early.DeptId,
                late => late.DeptId,
                (early, late) => new
                {
                    Early = early,
                    Late = late,
                    percentExpenseIncrease = (int)Math.Round(((((Decimal)late.FundsUsed - (Decimal)early.FundsUsed) / (Decimal)early.FundsUsed) * 100), 0)
                })

                //Filter by matches with the right year gap.
                .Where(_ => _.Late.FiscalYear - _.Early.FiscalYear == numberOfYearsFilter)

                //Filter by matches with the right percentage filter.
                .Where(_ => _.percentExpenseIncrease == percentIncreaseFilter)

                //Specify what values to keep.
                .Select(_ => new DepartmentsExpensesIncreasedResponseModel()
                {
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

        /// <summary>
        /// 3. return departments whose expenses are a user specified percentage below their funding year over year.
        /// </summary>
        /// <param name="belowFundingPercentageFilter"></param>
        /// <returns></returns>
        public List<DepartmentsExpensesBelowFundingResponseModel> DepartmentsExpensesBelowFunding(int belowFundingPercentageFilter)
        {
            //First, join the results to itself to get the set that is numberOfYearsFilter apart.  Filter out DivideByZero errors.
            var cleanData = NYCData.Where(_ => _.FundsAvailable != 0).ToList();

            //Join on DeptId.  That gets you the combinations.
            //While I'm at it, calculate the percentDiff.
            return cleanData.Join(cleanData,
                early => early.DeptId,
                late => late.DeptId,
                (early, late) => new
                {
                    Early = early,
                    Late = late,
                    FundingDecreasePercentage = (int)Math.Round(((((Decimal)early.FundsAvailable - (Decimal)late.FundsAvailable) / (Decimal)early.FundsAvailable) * 100), 0)
                })

                //Filter by matches with the right year gap.
                .Where(_ => _.Late.FiscalYear - _.Early.FiscalYear == 1)

                //Filter by matches with the right percentage filter.
                .Where(_ => _.FundingDecreasePercentage == belowFundingPercentageFilter)

                //Specify what values to keep.
                .Select(_ => new DepartmentsExpensesBelowFundingResponseModel()
                {
                    DeptId = _.Early.DeptId,
                    DeptName = _.Early.DeptName,
                    EarlyFiscalYear = _.Early.FiscalYear,
                    EarlyFundsAvailable = _.Early.FundsUsed,
                    LastFiscalYear = _.Late.FiscalYear,
                    LateFundsAvailable = _.Late.FundsUsed,
                    FundingDecreasePercentage = _.FundingDecreasePercentage
                })
                .ToList();
        }


        //Private Methods
        /// <summary>
        /// Gets the data from the cloud and puts it in local memory as structured data.
        /// </summary>
        private void RefreshData()
        {
            //Mark that you got that data.
            _gotData = true;

            //Get the data as a string from the cloud and cast it to dirty data so I can work with it.
            var dirtyData = JsonConvert.DeserializeObject<DirtyDataModel>(_ClientMockBin.GetStringAsync(_EndpointUri).Result);

            //For each dirty object in the list, map it to a clean model.
            foreach (var item in dirtyData.data)
            {
                NYCData.Add(new NYCDataNodeModel()
                {
                    FiscalYear = GetIntFromObject(item[9]),
                    DeptId = GetIntFromObject(item[10]),
                    DeptName = item[11].ToString(),
                    FundsAvailable = GetIntFromObject(item[12]),
                    FundsUsed = GetIntFromObject(item[13]),
                    Remarks = item[14]?.ToString()
                });
            }
        }

        /// <summary>
        /// Takes in an object expected to be converted to an int, and does the conversion.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private static int GetIntFromObject(object v)
        {
            var r = 0;

            if (v != null && !int.TryParse(v.ToString(), out r))
            {
                //Value can't become an int.  Maybe do something IRL.
            }

            return r;
        }
    }
}
