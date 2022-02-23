using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Net.Http;
using WebApiApex.Services;
using Xunit;
using Xunit.Abstractions;

namespace WebApiApex.Tests2
{
    public class ServiceTests
        : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _application;
        private readonly ITestOutputHelper output;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ServiceNYCData _serviceNYCData;


        public ServiceTests(WebApplicationFactory<Program> application, ITestOutputHelper output)
        {
            //This application is prep for integration testing.
            _application = application;

            //xUnit needs this for outputting.
            this.output = output;

            //Create the MockBin HttpClient
            var MockBinClient = new HttpClient();
            MockBinClient.BaseAddress = new Uri("https://mockbin.org");

            //Create the HttpClientFactoory
            var tmpHttpClientFactory = new Mock<IHttpClientFactory>();

            //Add the HttpClient to the HttpClientFactory.
            tmpHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(MockBinClient);

            //Save the factory to a field to be used later.
            _httpClientFactory = tmpHttpClientFactory.Object;

            //Initialize the service.
            _serviceNYCData = new ServiceNYCData(_httpClientFactory);
        }

        [Fact]
        public void Test_Caching_Forward()
        {
            DoThirdTest();
            DoSecondTest();
            DoFirstTest();
        }

        [Fact]
        public void Test_Caching_Reverse()
        {
            DoFirstTest();
            DoSecondTest();
            DoThirdTest();
        }

        [Fact]
        private void DoFirstTest()
        {
            //Initial Time
            DateTime time0 = DateTime.Now;

            //Make the first call to the service.
            var data1 = _serviceNYCData.DepartmentsExpensesOverFunding();
            var time1 = DateTime.Now;

            //Output the results
            output.WriteLine($"Count of Data1: {data1.Count.ToString()}");
            output.WriteLine($"Data1 Milliseconds: {(time1 - time0).Milliseconds}");
            output.WriteLine("");

            Assert.True(data1.Count == 553);
        }

        [Fact]
        private void DoSecondTest()
        {
            //Initial Time
            DateTime time0 = DateTime.Now;

            //Make the second call to the service.            
            var data2 = _serviceNYCData.DepartmentsExpensesIncreased(10, 7);
            var time1 = DateTime.Now;

            //Output the results
            output.WriteLine($"Count of Data2: {data2.Count.ToString()}");
            output.WriteLine($"Data2 Milliseconds: {(time1 - time0).Milliseconds}");
            output.WriteLine("");

            Assert.True(data2.Count == 6);
        }

        [Fact]
        private void DoThirdTest()
        {
            //Initial Time
            DateTime time0 = DateTime.Now;

            //Make the third call to the service.
            var data3 = _serviceNYCData.DepartmentsExpensesBelowFunding(7);
            var time3 = DateTime.Now;

            //Output the results
            output.WriteLine($"Count of Data3: {data3.Count.ToString()}");
            output.WriteLine($"Data3 Milliseconds: {(time3 - time0).Milliseconds}");
            output.WriteLine("");

            Assert.True(data3.Count == 14);
        }
    }
}