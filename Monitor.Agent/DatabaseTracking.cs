using Monitor.Shared;
using RestSharp;
using System.Data.SqlClient;

namespace Monitor.Agent
{
    internal class DatabaseTracking
    {
        const string DatabaseDetailsQuery = @"
SELECT
 @@servername AS 'Server Name' -- The database server's machine name
,@@servicename AS 'Instance Name' -- e.g.: MSSQLSERVER
,DB_NAME() AS 'Database Name'
,HOST_NAME() AS 'Host Name' -- The database client's machine name
";
        const string UsersQuery = @"
select name as username
from sys.database_principals
where type not in ('A', 'G', 'R', 'X')
      and sid is not null
      and name != 'guest'
order by username
";

        private readonly string _connection;
        private readonly string _mothership;

        private DatabaseState _localState;

        public DatabaseTracking(string connection, string mothership)
        {
            _connection = connection;
            _mothership = mothership;
        }

        public void Track()
        {
            Console.WriteLine("Agent started and monitoring.");

            while (true)
            {
                var state = GetDatabaseState();
                if (_localState == null || !_localState.Equals(state))
                {
                    //notify state to mothership
                    _localState = state;
                    NotifyStateChanged();
                }

                Thread.Sleep(3000);
            }
        }

        private void NotifyStateChanged()
        {
            var baseUrl = new Uri(_mothership);
            var client = new RestClient(baseUrl);
            var request = new RestRequest("api/agent", Method.Put)
                .AddJsonBody(_localState);

            var response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                Console.WriteLine(response.ErrorMessage);
            }
        }

        private DatabaseState GetDatabaseState()
        {
            using var connection = new SqlConnection(_connection);
            connection.Open();

            var details = GetDatabaseDetails(connection);
            details.Users = GetUsers(connection);

            return details;
        }

        private DatabaseState GetDatabaseDetails(SqlConnection connection)
        {
            using var command = new SqlCommand(DatabaseDetailsQuery, connection);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new DatabaseState
                {
                    Host = reader.GetString(3),
                    Instance = reader.GetString(1),
                    Database = reader.GetString(2)
                };
            }
            else
            {
                throw new Exception("Could not read database details");
            }
        }

        private List<string> GetUsers(SqlConnection connection)
        {
            var users = new List<string>();

            using var command = new SqlCommand(UsersQuery, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                users.Add(reader.GetString(0));
            }

            return users;
        }
    }
}
