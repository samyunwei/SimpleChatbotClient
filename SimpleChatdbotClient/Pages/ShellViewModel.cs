using System;
using Stylet;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PropertyChanged;
using System.ComponentModel;
using System.Windows.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Grpc.Core;
using Simplechatbot;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using DataFormat = RestSharp.DataFormat;

namespace SimpleChatdbotClient.Pages
{
    public class ShellViewModel : Screen, INotifyPropertyChanged
    {
        private SimpleChatBotClientImp client;
        private IniFile mConfig;
        private ObservableCollection<ChatMessage> m_Messages = new ObservableCollection<ChatMessage>();
        public ObservableCollection<ChatMessage> Messages { get { return m_Messages; } }
        public ShellViewModel()
        {
            mConfig = new IniFile("./cfg.ini");
            Messages.Add(new ChatMessage() { IsSend = false, Message = "Hello" });
        }

        public int SelectedMessage { get; set; }
        public string Message { get; set; }

        public bool CanSendMessage
        {
            get { return !string.IsNullOrEmpty(Message); }
        }
        public async void SendMessage()
        {
            m_Messages.Add(new ChatMessage() { IsSend = true, Message = Message });

            SelectedMessage = m_Messages.Count - 1;
            try
            {
                var addr = mConfig.IniReadValue("Server", "ADDR").ToString();
                var type = mConfig.IniReadValue("Server", "TYPE").ToString();
                if (type.Equals("REST"))
                {
                    var themessage = Message;
                    var url_addr = string.Format("http://{0}", addr);
                    var client = new RestClient(url_addr);
                    var request = new RestRequest("receive", DataFormat.Json);
                    request.AddParameter("msg", themessage);
                    var raw = request.ToString();
                    var response = client.Get(request);
                    var jresult = (JObject)JsonConvert.DeserializeObject(response.Content);
                    var rep = jresult["intent"].ToString();
                    m_Messages.Add(new ChatMessage(){IsSend = false,Message = rep });
                }
                else
                {
                    var channel = new Channel(addr, ChannelCredentials.Insecure);
                    var client = new SimpleChatBotClientImp(new SimpleChatBotServer.SimpleChatBotServerClient(channel));
                    var themessage = Message;
                    var task = client.ChatAsync(themessage);
                    await task;
                    await channel.ShutdownAsync();
                    var rep = task.Result;
                    m_Messages.Add(new ChatMessage(){IsSend = false,Message = rep});
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "错误");
            }

            //CollectionViewSource.GetDefaultView(m_Messages).MoveCurrentTo(m_Messages[m_Messages.Count - 1]);
        }
    }
}
