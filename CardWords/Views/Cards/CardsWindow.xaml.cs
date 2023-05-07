using CardWords.Business.WordAction;
using CardWords.Core.Helpers;
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
using System.Windows.Shapes;

namespace CardWords.Views.Cards
{
    /// <summary>
    /// Логика взаимодействия для CardsWindow.xaml
    /// </summary>
    public partial class CardsWindow : Window
    {
        private List<WordActionData> data;

        public CardsWindow(int wordCount)
        {
            InitializeComponent();

            data = CommandHelper.GetCommand<IGetWordActionDataCommand>().Execute(wordCount);            
        }

        private void SetDataItem(WordActionData item)
        {

        }

        protected override void OnClosed(EventArgs e)
        {
            Owner.Show();

            base.OnClosed(e);
        }
    }
}
