using chat_app.Models;
using System.Collections.Concurrent;

namespace chat_app.DataService
{
    public class SharedDb
    {
        private readonly ConcurrentDictionary<string, UserConnection> _connection = new();

        public ConcurrentDictionary<string, UserConnection> Connection => _connection;

        

    }
}
