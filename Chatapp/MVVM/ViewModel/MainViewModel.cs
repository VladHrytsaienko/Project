using Client.MVVM.Core;
using Client.MVVM.Model;
using Client.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace Client.MVVM.ViewModel
{
    class MainViewModel
    {   public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public String Username { get; set; }
        public string Message { get; set; }
        private Server _server;
        public MainViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            Messages= new ObservableCollection<string>();
            _server = new Server();
            _server.connectedEvent += UserConnected;
            _server.msgReceivedEvent += MessageReceived;
            _server.userDisconectEvent += UserDisconected;
            ConnectToServerCommand =new RelayCommand(o=>_server.ConnectToServer(Username),o=> !string.IsNullOrEmpty(Username));
            SendMessageCommand = new RelayCommand(o => _server.SendMessageToServer(Message), o => !string.IsNullOrEmpty(Message));
        }
        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = _server.PacketReader.ReadMessage(),
                UID = _server.PacketReader.ReadMessage(),

            };
            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(()=>Users.Add(user));
            }
        }
        private void MessageReceived() 
        {
            var msg = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(()=>Messages.Add(msg));
        }
        private void UserDisconected() 
        {
            var uid = _server.PacketReader.ReadMessage();
            var user = Users.Where(x=> x.UID== uid).FirstOrDefault(); 
            Application.Current.Dispatcher.Invoke(()=>Users.Remove(user));
        }
    }
}
