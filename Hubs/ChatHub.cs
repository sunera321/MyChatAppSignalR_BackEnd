using chat_app.DataService;
using chat_app.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace chat_app.Hubs
{
    public class ChatHub : Hub
    {
        private readonly SharedDb _sharedDb;

        public ChatHub(SharedDb sharedDb)
        {
            _sharedDb = sharedDb;
        }

        public async Task JoinChat(UserConnection connection)
        {
            await Clients.All
                .SendAsync("ReceiveMessage", "admin", $"{connection.UserName} has joined the chat");
        }

        public async Task JoinSpecificChatRoom(UserConnection connection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);

            _sharedDb.Connection[Context.ConnectionId] = connection;


            await Clients.Group(connection.ChatRoom)
                .SendAsync("ReceiveMessage", "admin", $"{connection.UserName} has joined the chat room {connection.ChatRoom}");

            Console.WriteLine($"ChatRoom: {connection.ChatRoom} UserName: {connection.UserName}");
            //print out all chat rooms
            foreach (var item in _sharedDb.Connection)
            {
                Console.WriteLine($"Key: {item.Key} Value: {item.Value.ChatRoom}");
            }
        }


        public async Task SendMessage(string msg)
        {
            if (_sharedDb.Connection.TryGetValue(Context.ConnectionId, out UserConnection connection))
            {
                Console.WriteLine($"Sending message: {msg} from user: {connection.UserName}");
                await Clients.Group(connection.ChatRoom)
                    .SendAsync("ReceiveSpecificMessage", connection.UserName, msg);
                Console.WriteLine("ReceiveSpecificMessage event sent");
            }
            else
            {
                Console.WriteLine("SendMessage: Connection not found");
            }
        }
    }
}
