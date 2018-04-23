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

namespace MalenPlusPlus
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.ComboBox.Items.Add("1");
            this.ComboBox.Items.Add("5");
            this.ComboBox.Items.Add("10");
            this.ComboBox.Items.Add("15");
            this.ComboBox.Items.Add("20");
            this.ComboBox.Items.Add("25");
            this.ComboBox.Items.Add("30");
            this.ComboBox.Items.Add("50");
            this.ComboBox.SelectedIndex = 2;
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
