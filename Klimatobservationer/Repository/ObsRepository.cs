using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Npgsql;
using Klimatobservationer.Category;
namespace Klimatobservationer.Repository
{
    class ObsRepository
    {
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
                    command.Parameters.AddWithValue("firstname", observer.firstname);
                    command.Parameters.AddWithValue("lastname", observer.lastname);
                    command.ExecuteScalar();
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
                                firstname = (string)reader["firstname"],
                                lastname = (string)reader["lastname"]
                            };
                        }
                    }
                }
                return observer;
            }
        }
        public static IEnumerable<Observer> GetObservers()
        {
            string stmt = "select firstname, lastname from observer order by lastname";
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
                                firstname = (string)reader["firstname"],
                                lastname = (string)reader["lastname"]
                            };
                            observers.Add(observer);
                        }
                    }
                }
                return observers;
            }

        }

        #endregion
        #region UPDATE
        #endregion
        #region DELETE
        public static void DeleteObserver(int id)
        {
            string stmt = "DELETE FROM Observer WHERE id=@id";
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
            #endregion
        }
    }
}