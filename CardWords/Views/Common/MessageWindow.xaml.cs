using CardWords.Extensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CardWords.Views.Common
{
    /// <summary>
    /// Логика взаимодействия для MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        public MessageWindow(string[] messages)
        {
            InitializeComponent();

            var style = Resources["DefaultWhiteText"] as Style;

            if(messages != null)
            {
                for (int i = 0; i < messages.Length; i++)
                {
                    AddMessage(messages[i], style);
                }
            }
        }

        private void AddMessage(string message, Style style)
        {
            if(message.IsNullOrEmptyOrWhiteSpace())
            {
                return;
            }

            var block = new TextBlock();

            block.Text = message;
            block.Style = style;

            SP_Message.Children.Add(block);
        }

        private void Heap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
