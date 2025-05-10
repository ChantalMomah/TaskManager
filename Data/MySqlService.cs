using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

public class MySqlService
{
    private readonly string _connectionString;

    public MySqlService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<string> GetAllTasks()
    {
        var tasks = new List<string>();

        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            conn.Open();

            string query = "SELECT TaskName FROM tasks";
            MySqlCommand cmd = new MySqlCommand(query, conn);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    tasks.Add(reader["Title"].ToString());
                }
            }
        }

        return tasks;
    }
}
