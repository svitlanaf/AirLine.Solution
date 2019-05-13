using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirLine.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace AirLine.Tests
{
    [TestClass]
    public class CityTest : IDisposable
    {
        public void Dispose()
        {
        City.ClearAll();
        Flight.DeleteAll();
        }

        public CityTest()
        {
        DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=airline_planner_test;";
        }

        [TestMethod]
       public void Save_SavesCityToDatabase_CityList()
       {
       City testCity = new City("New York");
       testCity.Save();
       List<City> result = City.GetAll();
       List<City> testList = new List<City>{testCity};
       CollectionAssert.AreEqual(testList, result);
       }

       [TestMethod]
        public void Find_ReturnsCityInDatabase_City()
        {
        City testCity = new City("New York");
        testCity.Save();
        City foundCity = City.Find(testCity.GetId());
        Assert.AreEqual(testCity, foundCity);
        }
     }
   }
