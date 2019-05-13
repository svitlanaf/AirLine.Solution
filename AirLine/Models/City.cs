using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace AirLine.Models
{
  public class City
  {
    private string _name;
    private int _id;
    public City(string name, int id = 0)
    {
    	_name = name;
    	_id = id;
    }

    public override int GetHashCode()
      {
      	return this.GetId().GetHashCode();
      }

    public string GetName()
      {
      	return _name;
      }

    public int GetId()
      {
      	return _id;
      }


    public void Dispose()
    {
      City.ClearAll();
      Flight.DeleteAll();
    }


    public override bool Equals(System.Object otherCity)
  {
  	if (!(otherCity is City))
  	{
  		return false;
  	}
  	else
  	{
  		City newCity = (City) otherCity;
  		bool idEquality = this.GetId().Equals(newCity.GetId());
  		bool nameEquality = this.GetName().Equals(newCity.GetName());
  		return (idEquality && nameEquality);
  	}
  }


      public void Save()
  {
  	MySqlConnection conn = DB.Connection();
  	conn.Open();

  	var cmd = conn.CreateCommand() as MySqlCommand;
  	cmd.CommandText = @"INSERT INTO city (name) VALUES (@name);";

  	MySqlParameter name = new MySqlParameter();
  	name.ParameterName = "@name";
  	name.Value = this._name;
  	cmd.Parameters.Add(name);

  	cmd.ExecuteNonQuery();
  	_id = (int) cmd.LastInsertedId;
  	conn.Close();
  	if (conn != null)
    	{
    		conn.Dispose();
    	}

  }

  public static List<City> GetAll()
  {
  	List<City> allCities = new List<City> {
  	};
  	MySqlConnection conn = DB.Connection();
  	conn.Open();
  	var cmd = conn.CreateCommand() as MySqlCommand;
  	cmd.CommandText = @"SELECT * FROM city;";
  	var rdr = cmd.ExecuteReader() as MySqlDataReader;
  	while(rdr.Read())
  	{
  		int CityId = rdr.GetInt32(0);
  		string CityName = rdr.GetString(1);
  		City newCity = new City(CityName, CityId);
  		allCities.Add(newCity);
  	}
  	conn.Close();
  	if (conn != null)
  	{
  		conn.Dispose();
  	}
  	return allCities;
  }


  public static City Find(int id)
{
	MySqlConnection conn = DB.Connection();
	conn.Open();
	var cmd = conn.CreateCommand() as MySqlCommand;
	cmd.CommandText = @"SELECT * FROM city WHERE id = (@searchId);";

	MySqlParameter searchId = new MySqlParameter();
	searchId.ParameterName = "@searchId";
	searchId.Value = id;
	cmd.Parameters.Add(searchId);

	var rdr = cmd.ExecuteReader() as MySqlDataReader;
	int CityId = 0;
	string CityName = "";

	while(rdr.Read())
	{
		CityId = rdr.GetInt32(0);
		CityName = rdr.GetString(1);
	}
	City newCity = new City(CityName, CityId);
	conn.Close();
	if (conn != null)
	{
		conn.Dispose();
	}
	return newCity;
}

public static void DeleteAll()
{
	MySqlConnection conn = DB.Connection();
	conn.Open();
	var cmd = conn.CreateCommand() as MySqlCommand;
	cmd.CommandText = @"DELETE FROM city;";
	cmd.ExecuteNonQuery();
	conn.Close();
	if (conn != null)
	{
		conn.Dispose();
	}
}


public void Delete()
{
	MySqlConnection conn = DB.Connection();
	conn.Open();
	MySqlCommand cmd = new MySqlCommand( "DELETE FROM city WHERE id = @CityId; DELETE FROM city_items WHERE city_id = @CityId;", conn);
	MySqlParameter cityIdParameter = new MySqlParameter();
	cityIdParameter.ParameterName = "@CityId";
	cityIdParameter.Value = this.GetId();
	cmd.Parameters.Add(cityIdParameter);
	cmd.ExecuteNonQuery();

	if (conn != null)
	{
		conn.Close();
	}
}

public List<Flight> GetFlights()
{

	MySqlConnection conn = DB.Connection();
	conn.Open();
	var cmd = conn.CreateCommand() as MySqlCommand;
	cmd.CommandText = @"SELECT flight.* FROM city
            JOIN cities_flights ON (city.id = cities_flights.city_id)
            JOIN flight ON (cities_flights.flight_id = flight.id)
            WHERE city.id = @CityId;";
	MySqlParameter cityIdParameter = new MySqlParameter();
	cityIdParameter.ParameterName = "@CityId";
	cityIdParameter.Value = _id;
	cmd.Parameters.Add(cityIdParameter);
	MySqlDataReader flightQueryRdr = cmd.ExecuteReader() as MySqlDataReader;
	List<Flight> flights = new List<Flight> {
	};

	while(flightQueryRdr.Read())
	{
		int thisFlightId = flightQueryRdr.GetInt32(0);
		DateTime flightDepartureTime = flightQueryRdr.GetDateTime(1);
		string flightDepartureCity = flightQueryRdr.GetString(2);
    string flightStatus = flightQueryRdr.GetString(3);
		Flight newCity = new Flight (flightDepartureTime, flightDepartureCity, flightStatus, thisFlightId);
		flights.Add (newCity);
	}

	conn.Close();
	if (conn != null)
	{
		conn.Dispose();
	}
	return flights;
}

public static void ClearAll()
  {
  	MySqlConnection conn = DB.Connection();
  	conn.Open();
  	var cmd = conn.CreateCommand() as MySqlCommand;
  	cmd.CommandText = @"DELETE FROM city;";
  	cmd.ExecuteNonQuery();

  	conn.Close();
  	if(conn != null)
  	{
  		conn.Dispose();
  	}
  }


public void AddFlight (Flight newFlight)
  {
  	MySqlConnection conn = DB.Connection();
  	conn.Open();
  	var cmd = conn.CreateCommand() as MySqlCommand;
  	cmd.CommandText = @"INSERT INTO cities_flights (city_id, flight_id) VALUES (@CityId, @FlightId);";
  	MySqlParameter city_id = new MySqlParameter();
  	city_id.ParameterName = "@CityId";
  	city_id.Value = _id;
  	cmd.Parameters.Add(city_id);
  	MySqlParameter flight_id = new MySqlParameter();
  	flight_id.ParameterName = "@FlightId";
  	flight_id.Value = newFlight.GetId();
  	cmd.Parameters.Add(flight_id);
  	cmd.ExecuteNonQuery();
  	conn.Close();
  	if(conn != null)
  	{
  		conn.Dispose();
  	}
  }



  }
}
