using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirLine.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace AirLine.Tests
{
    [TestClass]
    public class FlightTest : IDisposable
    {
        public void Dispose()
        {
        City.ClearAll();
        Flight.DeleteAll();
        }

        public FlightTest()
        {
        DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=airline_planner_test;";
        }

        [TestMethod]
       public void Save_SavesFlightToDatabase_FlightList()
       {
       Flight testFlight = new Flight(new DateTime(1991,1,1), "London", "on time");
       testFlight.Save();
       List<Flight> result = Flight.GetAll();
       List<Flight> testList = new List<Flight>{testFlight};
       CollectionAssert.AreEqual(testList, result);
       }

       [TestMethod]
      public void EditStatus_UpdatesFlightStatusInDB_String()
      {
      Flight testFlight = new Flight(new DateTime(1991,1,1), "London", "on time");
      testFlight.Save();
      testFlight.Edit("cancelled");
      List<Flight> newList = Flight.GetAll();
      string result = newList[0].GetStatus();
      Assert.AreEqual("cancelled", result);
      }

      [TestMethod]
      public void GetCities_ReturnsAllFlightCities_CityList()
      {
        Flight testFlight = new Flight(new DateTime(1991,1,1), "London", "on time");
        testFlight.Save();
        City testCity1 = new City("Kiev");
        testCity1.Save();
        City testCity2 = new City("Paris");
        testCity2.Save();
        testFlight.AddCity(testCity1);
        List<City> result = testFlight.GetCities();
        List<City> testList = new List<City> {testCity1};
        CollectionAssert.AreEqual(testList, result);
      }

      [TestMethod]
      public void AddCity_AddsCityToFlight_CityList()
      {
        Flight testFlight = new Flight(new DateTime(1991,1,1), "London", "on time");
        testFlight.Save();
        City testCity = new City("London");
        testCity.Save();
        testFlight.AddCity(testCity);
        List<City> result = testFlight.GetCities();
        List<City> testList = new List<City>{testCity};
        CollectionAssert.AreEqual(testList, result);
      }

      [TestMethod]
      public void OrderFlights_SortsFlightsByCityAndByDepartureTimeDifferentCities_FlightList()
      {
        Flight testFlight1 = new Flight(new DateTime(1991,1,1), "London", "on time");
        testFlight1.Save();
        Flight testFlight2 = new Flight(new DateTime(1991,1,1), "Paris", "on time");
        testFlight2.Save();
        List<Flight> newList = Flight.Sort();
        List<Flight> expectedList = new List<Flight> {testFlight1, testFlight2};
        CollectionAssert.AreEqual(newList, expectedList);
      }

      [TestMethod]
      public void OrderFlights_SortsFlightsByCityAndByDepartureTimeDifferentTime_FlightList()
      {
        Flight testFlight1 = new Flight(new DateTime(1991,1,1), "London", "on time");
        testFlight1.Save();
        Flight testFlight2 = new Flight(new DateTime(2000,1,1), "London", "on time");
        testFlight2.Save();
        List<Flight> newList = Flight.Sort();
        List<Flight> expectedList = new List<Flight> {testFlight1, testFlight2};
        CollectionAssert.AreEqual(newList, expectedList);
      }
}
}
