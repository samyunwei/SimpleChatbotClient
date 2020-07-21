using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace SimpleChatdbotClient
{
    public class AutoScrollBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SelectionChanged += new SelectionChangedEventHandler(AssociatedObject_SelectionChanged);
        }
        void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox)
            {
                ListBox listbox = (sender as ListBox);
                if (listbox.SelectedItem != null)
                {
                    listbox.Dispatcher.BeginInvoke((Action)delegate
                    {
                        listbox.UpdateLayout();
                        listbox.ScrollIntoView(listbox.SelectedItem);
                    });
                }
            }
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.SelectionChanged -=
                new SelectionChangedEventHandler(AssociatedObject_SelectionChanged);
        }
    }
}
