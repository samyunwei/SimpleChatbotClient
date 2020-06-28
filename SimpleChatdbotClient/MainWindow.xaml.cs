using System;
using System.Collections.Generic;
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


namespace SimpleChatdbotClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private SimpleChatBotClientImp client;
        private IniFile mConfig;

        public MainWindow()

        {
            InitializeComponent();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addr = mConfig.IniReadValue("Server", "ADDR").ToString();
                var type = mConfig.IniReadValue("Server", "TYPE").ToString();
                if (type.Equals("REST"))
                {
                    var message = textBox_message.Text;
                    var rep = makeResponse("自己", message);
                    textBox_history.AppendText(rep);
                    textBox_message.Text = "";
                    var url_addr = string.Format("http://{0}", addr);
                    var client = new RestClient(url_addr);
                    var request = new RestRequest("receive", DataFormat.Json);
                    request.AddParameter("msg",message);
                    var raw = request.ToString();
                    var response = client.Get(request);
                    var jresult = (JObject) JsonConvert.DeserializeObject(response.Content);
                    rep = makeResponse("聊天机器人", jresult["intent"].ToString());
                    textBox_history.AppendText(rep);
                }
                else
                {
                    var channel = new Channel(addr, ChannelCredentials.Insecure);
                    var client = new SimpleChatBotClientImp(new SimpleChatBotServer.SimpleChatBotServerClient(channel));
                    var message = textBox_message.Text;
                    var rep = makeResponse("自己", message);
                    textBox_history.AppendText(rep);
                    textBox_message.Text = "";
                    var task = client.ChatAsync(message);
                    await task;
                    await channel.ShutdownAsync();
                    rep = makeResponse("聊天机器人", task.Result);
                    textBox_history.AppendText(rep);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "错误");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mConfig = new IniFile("./cfg.ini");
        }

        string makeResponse(string speaker, string data)
        {
            return string.Format("{0} ：\n       {1}\n", speaker, data);
        }
    }
}