using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace AirLine.Models
{
  public class Flight   // class
  {
  private DateTime _departureTime;
  private string _departureCity;
  private string _status;   // field
  private int _id;

  public Flight (DateTime departureTime, string departureCity, string status, int id=0)     // constructor
  {
  	_departureTime = departureTime;
  	_departureCity = departureCity;
    _status = status;
  	_id = id;
  }

  public void Dispose()
  {
    City.ClearAll();
    Flight.DeleteAll();
  }

  public int GetId()
  {
  	return _id;
  }

  public DateTime GetDepartureTime()
  {
  	return _departureTime;
  }

  public string GetDepartureCity()
  {
  	return _departureCity;
  }

  public string GetStatus ()
  {
    return _status;
  }

  public override int GetHashCode()
    {
      return this.GetId().GetHashCode();
    }


  public static List<Flight> GetAll()
  {
  	List<Flight> allFlights = new List<Flight> {
  	};

  	MySqlConnection conn = DB.Connection();
  	conn.Open();
  	MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
  	cmd.CommandText = @"SELECT * FROM flight;";
  	MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;

  	while (rdr.Read())
  	{
      int thisFlightId = rdr.GetInt32(0);
  		DateTime flightDepartureTime = rdr.GetDateTime(1);
  		string flightDepartureCity = rdr.GetString(2);
      string flightStatus = rdr.GetString(3);
  		Flight newFlight = new Flight (flightDepartureTime, flightDepartureCity, flightStatus, thisFlightId);
  		allFlights.Add(newFlight);
  	}

  	conn.Close();

  	if (conn != null)
  	{
  		conn.Dispose();
  	}

  	return allFlights;

  }

  public static void DeleteAll()
  {
  	MySqlConnection conn = DB.Connection();
  	conn.Open();
  	var cmd = conn.CreateCommand() as MySqlCommand;
  	cmd.CommandText = @"DELETE FROM flight;";
  	cmd.ExecuteNonQuery();

  	conn.Close();
  	if(conn != null)
  	{
  		conn.Dispose();
  	}
  }

  public override bool Equals(System.Object otherFlight)
  {
  	if (!(otherFlight is Flight))
  	{
  		return false;
  	}
  	else
  	{
  		Flight newFlight = (Flight) otherFlight;

  		bool idEquality = this.GetId() == newFlight.GetId();
  		bool flightDepartureTimeEquality = this.GetDepartureTime() == newFlight.GetDepartureTime();
  		bool flightDepartureCityEquality = this.GetDepartureCity() == newFlight.GetDepartureCity();
      bool flightStatusEquality = this.GetStatus() == newFlight.GetStatus();
  		return (idEquality && flightDepartureTimeEquality && flightDepartureCityEquality && flightStatusEquality);
  	}
  }

  public void Save()
  {
  	MySqlConnection conn = DB.Connection();
  	conn.Open();
  	var cmd = conn.CreateCommand() as MySqlCommand;

  	cmd.CommandText = @"INSERT INTO flight (departure_time, departure_city, status) VALUES (@FlightDepartureTime, @FlightDepartureCity, @FlightStatus);";

  	MySqlParameter departureTimeParameter = new MySqlParameter();
  	departureTimeParameter.ParameterName = "@FlightDepartureTime";
  	departureTimeParameter.Value = this._departureTime;
  	cmd.Parameters.Add(departureTimeParameter);

  	MySqlParameter departureCityParameter = new MySqlParameter();
  	departureCityParameter.ParameterName = "@FlightDepartureCity";
  	departureCityParameter.Value = this._departureCity;
  	cmd.Parameters.Add(departureCityParameter);

    MySqlParameter flightStatusParameter = new MySqlParameter();
  	flightStatusParameter.ParameterName = "@FlightStatus";
  	flightStatusParameter.Value = this._status;
  	cmd.Parameters.Add(flightStatusParameter);

  	cmd.ExecuteNonQuery();

  	_id = (int) cmd.LastInsertedId;

  	conn.Close();
  	if (conn != null)
  	{
  		conn.Dispose();
  	}
  }

  public static Flight Find (int id)
{

	MySqlConnection conn = DB.Connection();
	conn.Open();
	var cmd = conn.CreateCommand() as MySqlCommand;
	cmd.CommandText = @"SELECT * FROM flight WHERE id = (@searchId);";
	MySqlParameter idParameter = new MySqlParameter();
	idParameter.ParameterName = "@searchId";
	idParameter.Value = id;
	cmd.Parameters.Add(idParameter);
	var rdr = cmd.ExecuteReader() as MySqlDataReader;
	int flightId=0;

	DateTime flightDepartureTime = new DateTime();
  string flightDepartureCity = "";
  string flightStatus = "";

	while(rdr.Read())
	{
    flightId = rdr.GetInt32(0);
    flightDepartureTime = rdr.GetDateTime(1);
    flightDepartureCity = rdr.GetString(2);
    flightStatus = rdr.GetString(3);
	}
	Flight foundFlight = new Flight (flightDepartureTime,flightDepartureCity, flightStatus, flightId);

	conn.Close();
	if(conn != null)
	{
		conn.Dispose();
	}
	return foundFlight;
}


public List<City> GetCities()
{
	MySqlConnection conn = DB.Connection();
	conn.Open();
	var cmd = conn.CreateCommand() as MySqlCommand;
	cmd.CommandText = @"SELECT city_id FROM cities_flights WHERE flight_id = @flightId;";
	MySqlParameter flightIdParameter = new MySqlParameter();
	flightIdParameter.ParameterName = "@flightId";
	flightIdParameter.Value = _id;
	cmd.Parameters.Add(flightIdParameter);
	var rdr = cmd.ExecuteReader() as MySqlDataReader;
	List<int> cityIds = new List <int> {
	};
	while (rdr.Read())
	{
		int cityId = rdr.GetInt32(0);
		cityIds.Add(cityId);
	}
	rdr.Dispose();
	List<City> cities = new List<City> {
	};
	foreach (int cityId in cityIds)
	{
		var cityQuery = conn.CreateCommand() as MySqlCommand;
		cityQuery.CommandText = @"SELECT * FROM city WHERE id = @CityId;";
		MySqlParameter cityIdParameter = new MySqlParameter();
		cityIdParameter.ParameterName = "@CityId";
		cityIdParameter.Value = cityId;
		cityQuery.Parameters.Add(cityIdParameter);
		var cityQueryRdr = cityQuery.ExecuteReader() as MySqlDataReader;
		while(cityQueryRdr.Read())
		{
			int thisCityId = cityQueryRdr.GetInt32(0);
			string cityName = cityQueryRdr.GetString(1);
			City foundCity = new City(cityName, thisCityId);
			cities.Add(foundCity);
		}
		cityQueryRdr.Dispose();
	}
  conn.Close();
	if(conn != null)
	{
		conn.Dispose();
	}
	return cities;
}

public void AddCity (City newCity)
{
  MySqlConnection conn = DB.Connection();
  conn.Open();
  var cmd = conn.CreateCommand() as MySqlCommand;
  cmd.CommandText = @"INSERT INTO cities_flights (city_id, flight_id) VALUES (@CityId, @FlightId);";
  MySqlParameter city_id = new MySqlParameter();
  city_id.ParameterName = "@CityId";
  city_id.Value = newCity.GetId();
  cmd.Parameters.Add(city_id);
  MySqlParameter flight_id = new MySqlParameter();
  flight_id.ParameterName = "@FlightId";
  flight_id.Value = _id;
  cmd.Parameters.Add(flight_id);
  cmd.ExecuteNonQuery();


  conn.Close();
  if(conn != null)
  {
    conn.Dispose();
  }
}

public void Delete()
{
	MySqlConnection conn = DB.Connection();
	conn.Open();
	var cmd = conn.CreateCommand() as MySqlCommand;
	cmd.CommandText = @"DELETE FROM flights WHERE id = @FlightId; DELETE FROM cities_items WHERE flight_id = @FlightId;";
	MySqlParameter flightIdParameter = new MySqlParameter();
	flightIdParameter.ParameterName = "@ItemId";
	flightIdParameter.Value = this.GetId();
	cmd.Parameters.Add(flightIdParameter);
	cmd.ExecuteNonQuery();
	if(conn != null)
	{
		conn.Close();
	}
}

public void Edit(string newStatus)
  {
  	MySqlConnection conn = DB.Connection();
  	conn.Open();
  	var cmd = conn.CreateCommand() as MySqlCommand;
  	// cmd.CommandText = @"UPDATE flight SET departureTime = @newDepartureTime WHERE id = @searchId;";
  	// cmd.CommandText = @"UPDATE flight SET departureCity = @newDepartureCity WHERE id = @searchId;";
    cmd.CommandText = @"UPDATE flight SET status = @newStatus WHERE id = @searchId;";
  	MySqlParameter searchId = new MySqlParameter();
  	searchId.ParameterName = "@searchId";
  	searchId.Value = _id;
  	cmd.Parameters.Add(searchId);

  	// MySqlParameter departureTime = new MySqlParameter();
  	// departureTime.ParameterName = "@newDepartureTime";
  	// departureTime.Value = newDepartureTime;
  	// cmd.Parameters.Add(departureTime);
    //
  	// MySqlParameter departureCity = new MySqlParameter();
  	// departureCity.ParameterName = "@newDepartureCity";
  	// departureCity.Value = newDepartureCity;
  	// cmd.Parameters.Add(departureCity);
  	// cmd.ExecuteNonQuery();

    MySqlParameter status = new MySqlParameter();
  	status.ParameterName = "@newStatus";
  	status.Value = newStatus;
  	cmd.Parameters.Add(status);
  	cmd.ExecuteNonQuery();

  	_status = newStatus;

  	conn.Close();
  	if (conn != null)
  	{
  		conn.Dispose();
  	}
  }




}
}
