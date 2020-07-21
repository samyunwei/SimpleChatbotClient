using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SimpleChatdbotClient
{
    class ChatBubbleSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var u = container as FrameworkElement;

            ChatMessage message = item as ChatMessage;

            if (message.IsSend)
                return u.FindResource("chatSend1") as DataTemplate;
            else
                return u.FindResource("chatRecv1") as DataTemplate;
        }
    }
}
