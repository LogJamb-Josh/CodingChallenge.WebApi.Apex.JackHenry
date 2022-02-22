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
        //   1. return departments whose expenses meet or exceed their funding
        public List<DepartmentsExpensesOverFundingResponseModel> DepartmentsExpensesOverFunding()
        {
            return NYCData
                .Where(_ => _.FundsUsed >= _.FundsAvailable)
                .Select(_ => new DepartmentsExpensesOverFundingResponseModel() { DeptId = _.DeptId, DeptName = _.DeptName })
                .ToList();
        }

        //   2. return deparments whose expenses have increased over time by user specified percentage (int) and # of years (int)
        public List<DepartmentsExpensesIncreasedResponseModel> DepartmentsExpensesIncreased(int percentIncreaseFilter, int numberOfYearsFilter)
        {


            percentIncreaseFilter = 10;
            numberOfYearsFilter = 7;

            var L = NYCData.ToList();
            var R = NYCData.ToList();

            //Do a join on DeptId.  That gets you the combinations.
            //Filter by matches with the right year gap.
            //Filter by matches with the right percentage filter.
            var query = L.Join(R,
                ll => ll.DeptId,
                rr => rr.DeptId,
                (ll, rr) => new { L = ll, R = rr })
                .Where(_ => _.L.FiscalYear - _.R.FiscalYear == numberOfYearsFilter)
                .Where(_ => _.L.FundsUsed > 0)
                .Where(_ => _.R.FundsUsed > 0)
                //.Where(_ => (int)(((_.R.FundsUsed - _.L.FundsUsed) / _.L.FundsUsed) * 100) == percentIncreaseFilter);
                .Select(_ => new DepartmentsExpensesIncreasedResponseModel() { DeptId = _.L.DeptId, DeptName = _.L.DeptName, EarlyFiscalYear = _.L.FiscalYear, EarlyFundsUsed = _.L.FundsUsed, LastFiscalYear = _.R.FiscalYear, LateFundsUsed = _.R.FundsUsed, YearsDifferent= _.L.FiscalYear - _.R.FiscalYear, PercentDifferent = (((_.R.FundsUsed - _.L.FundsUsed) / _.L.FundsUsed) * 100) });

            //var one = query.ToList();

            //return null;
            return query.ToList();

        }

        //   3. return departments whose expenses are a user specified percentage below their funding year over year.
        public Task DepartmentsExpensesBelowFunding()
        {
            RefreshData();
            return Task.CompletedTask;
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
