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
using Klimatobservationer.Category;
using Klimatobservationer.Repository;
using Npgsql;
using static Klimatobservationer.Repository.ObsRepository;

namespace Klimatobservationer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

           // ShowObservers();
        }
        public void ShowObservers()
        {
           
           

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var observer = new Observer
            {
                firstname = txtFirstname.Text,
                lastname = txtLastname.Text
            };
            AddObserver(observer);
            txtFirstname.Clear();
            txtLastname.Clear();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var id = int.Parse(txtId.Text);
            try
            {
                DeleteObserver(id);
            }
            catch (PostgresException ex)
            {
                var code = ex.SqlState;
                MessageBox.Show($"{GetObserver(4)} kan inte tas bort för att {ex.Message}");
            }
            txtId.Clear();
            
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var observers = GetObservers();
            listNames.ItemsSource = null;
            listNames.ItemsSource = observers;
          
        }
    }
}
