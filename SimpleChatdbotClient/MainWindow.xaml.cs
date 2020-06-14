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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var addr = mConfig.IniReadValue("Server", "ADDR").ToString();
            var channel = new Channel(addr, ChannelCredentials.Insecure);
            var client = new SimpleChatBotClientImp(new SimpleChatBotServer.SimpleChatBotServerClient(channel));
            var message = textBox_message.Text;
            var rep = makeResponse("自己", message);
            textBox_history.AppendText(rep);
            textBox_message.Text = ""; 
            var rep_data = client.Chat(message);

            rep = makeResponse("聊天机器人", rep_data);
            textBox_history.AppendText(rep);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            mConfig = new IniFile("./cfg.ini");
        }

        string makeResponse(string speaker,string data)
        {
            return string.Format("{0} ：\n       {1}\n", speaker, data);
            
        }
    }
}
