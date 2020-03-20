using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Npgsql;
using Klimatobservationer.Classes;
namespace Klimatobservationer.Repository
{
    class ObsRepository
    {
        public ObsRepository()
        {
            
        }
        
        private static string connectionString = ConfigurationManager.ConnectionStrings["dbLocal"].ConnectionString;
        #region CREATE
        public static void AddObserver(Observer observer)
        {

            string stmt = "INSERT INTO observer(firstname, lastname) values(@firstname, @lastname)";
            using (var conn = new NpgsqlConnection(connectionString)) 
            {
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    conn.Open();
                    command.Parameters.AddWithValue("firstname", observer.Firstname);
                    command.Parameters.AddWithValue("lastname", observer.Lastname);
                    command.ExecuteScalar();
                }
            }

        }
        public static int AddObservation(Observation observation)
        {
            var Id = 0;
            string stmt = "INSERT INTO observation (observer_id, geolocation_id) values(@observer_id, 7) returning id";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        using (var command = new NpgsqlCommand(stmt, conn))
                        {
                            
                            command.Parameters.AddWithValue("observer_id", observation.Observer_id);
                            Id = (int)command.ExecuteScalar();
                        }

                        trans.Commit();
                    }
                            
                    catch (PostgresException ex)
                    {
                        trans.Rollback();
                        throw;
                    }

                    return Id;
                }
            }

        }
        public static void AddMeasurement(Measurement measurement)
        {
            string stmt = "INSERT INTO measurement (observation_id, category_id, value) values(@observation_id, @category_id, @value)";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        using (var command = new NpgsqlCommand(stmt, conn))
                        {

                            command.Parameters.AddWithValue("observation_id", measurement.Observation_id);
                            command.Parameters.AddWithValue("category_id", measurement.Category_id);
                            command.Parameters.AddWithValue("value", measurement.Value);
                            command.ExecuteScalar();
                        }
                        trans.Commit();
                    }

                    catch (PostgresException ex)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

        }
        #endregion
        #region READ
        public static Observer GetObserver(int id)
        {
            string stmt = "select id, firstname, lastname from observer where id=@id";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                Observer observer = null;
                conn.Open();
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    command.Parameters.AddWithValue("id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            observer = new Observer
                            {
                                Id = (int)reader["id"],
                                Firstname = (string)reader["firstname"],
                                Lastname = (string)reader["lastname"]
                            };
                        }
                    }
                }
                return observer;
            }
        }
        public static IEnumerable<Observer> GetObservers()
        {
            string stmt = "SELECT id, firstname, lastname FROM observer ORDER BY lastname";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                Observer observer = null;
                List<Observer> observers = new List<Observer>();
                conn.Open();
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            observer = new Observer
                            {
                                Id = (int)reader["id"],
                                Firstname = (string)reader["firstname"],
                                Lastname = (string)reader["lastname"]
                            };
                            observers.Add(observer);
                        }
                    }
                }
                return observers;
            }

        }
        public static IEnumerable<Observer> GetDeletebleObservers()
        {
            string stmt = "SELECT observer.id, observer.firstname, observer.lastname " +
                "FROM observer FULL JOIN observation ON observer.id = observation.observer_id " +
                "WHERE observer_id IS NULL";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                Observer observer = null;
                List<Observer> observers = new List<Observer>();
                conn.Open();
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            observer = new Observer
                            {
                                Id = (int)reader["id"],
                                Firstname = (string)reader["firstname"],
                                Lastname = (string)reader["lastname"]
                            };
                            observers.Add(observer);
                        }
                    }
                }
                return observers;
            }
        }
        public static IEnumerable<Observation> ShowObservations(int id)
        {
            string stmt = "SELECT observation.id, observation.date " +
                "FROM observation WHERE observation.observer_id = @id";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                Observation observation = null;
                List<Observation> observations = new List<Observation>();
                conn.Open();
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    command.Parameters.AddWithValue("id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            observation = new Observation
                            {
                                Id = (int)reader["id"],
                                Date = (DateTime)reader["date"]
                            };
                            observations.Add(observation);
                        }
                    }
                }
                return observations;
            }

        }
        public static IEnumerable<Category> GetCategorys()
        {
            string stmt = "SELECT id, basecategory_id, name FROM category";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                Category category = null;
                List<Category> categories = new List<Category>();
                conn.Open();
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            category = new Category
                            {
                                Id = (int)reader["id"],
                                BaseId = (reader["basecategory_id"] as int?).GetValueOrDefault(),
                                Name = (string)reader["name"]
                            };
                            categories.Add(category);
                        }
                    }
                }
                return categories;
            }

        }

        #endregion
        #region UPDATE
        

        #endregion
        #region DELETE
        public static void DeleteObserver(int id)
        {
            string stmt = "DELETE FROM observer WHERE id=@id";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        using (var command = new NpgsqlCommand(stmt, conn))
                        {
                            command.Parameters.AddWithValue("id", id);
                            command.Prepare();
                            command.ExecuteScalar();
                        }
                        trans.Commit();
                    }
                    catch (PostgresException ex)
                    {
                        trans.Rollback();
                        throw;
                    }

                }
            }
            
        }
        public static void DeleteObservation(int id)
        {
            string stmt = "DELETE FROM observation WHERE id=@id";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    command.Parameters.AddWithValue("id", id);
                    command.Prepare();
                    command.ExecuteScalar();
                }
            }
        }
        #endregion
    }
}