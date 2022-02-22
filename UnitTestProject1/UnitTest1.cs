using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

//Challenge: write a web api that will call an external(3rd party) service that will return past years accounting information for NYC departments.
//include unit tests, error handling, caching, dependency injection, and best practices.

// the api should have endpoints that:
//   1. return departments whose expenses meet or exceed their funding
//   2. return deparments whose expenses have increased over time by user specified percentage (int) and # of years (int)
//   3. return departments whose expenses are a user specified percentage below their funding year over year.
   
//   Note: for this challenge the json indexes of importance are:
//   9 = fiscal year
//   10 = dept.id
//   11 = dept.name
//   12 = funds available
//   13 = funds used
//   14 = remarks


//  https://mockbin.org/bin/fb525688-91a7-47da-a319-fcfc24a14001


//bwoschnick@apexsystems.com

//edmclaughlin@hotmail.com



namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
