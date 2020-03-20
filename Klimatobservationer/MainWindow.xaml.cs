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
using Klimatobservationer.Classes;
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
        Category[] finalCategory = new Category[2];
        //List<Category> finalCategories = new List<Category>();
        List<Measurement> measurements  = new List<Measurement>();
        public MainWindow()
        {
            InitializeComponent();
            UpdateList();
           
          
        }
        public void UpdateObservations()
        {
            var observer = (Observer)listNames.SelectedItem;
            if (observer != null)
            {
                var observations = ShowObservations(observer.Id);
                listObservation.ItemsSource = null;
                listObservation.ItemsSource = observations;
            }
            else
            {
                MessageBox.Show("Välj en observatör");
            }
        }

        public void UpdateList()
        {
            var observers = GetObservers();
            listNames.ItemsSource = null;
            listNames.ItemsSource = observers;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var observer = new Observer
            {
                Firstname = txtFirstname.Text,
                Lastname = txtLastname.Text
            };
            AddObserver(observer);
            txtFirstname.Clear();
            txtLastname.Clear();
            UpdateList();
        }

        public void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var observer = (Observer)listNames.SelectedItem;
            try
            {
                if (observer != null)
                {
                    DeleteObserver(observer.Id);
                }
            }
            catch (PostgresException ex)
            {
                var code = ex.SqlState;
                MessageBox.Show(ex.Message);
                txtError.Text = $"{GetObserver(observer.Id)} har genomfört klimatobservationer och kan därför inte raderas."; 
            }
            UpdateList();


        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var observers = GetDeletebleObservers();
            listNames.ItemsSource = null;
            listNames.ItemsSource = observers;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            UpdateObservations();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var observation = (Observation)listObservation.SelectedItem;
            DeleteObservation(observation.Id);
            UpdateObservations();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var allCategories = GetCategorys();
            //listAddObservation.ItemsSource = null;
            //listAddObservation.ItemsSource = allCategories;
            List<Category> categories = new List<Category>();
            List<Category> categoriesweather = new List<Category>();
            foreach (var c in allCategories)
            {
                if (c.Id <= 2)
                {
                    categories.Add(c);
                }
                else if (c.Id == 3)
                {
                    categoriesweather.Add(c);
                }
            }
            listAddObservation.ItemsSource = null;
            listAddObservation.ItemsSource = categories;
            listAddWeather.ItemsSource = null;
            listAddWeather.ItemsSource = categoriesweather;

        }

        private void listAddObservation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            var thisCategory = (Category)listAddObservation.SelectedItem;
            if(thisCategory != null)
            {
                List<Category> categories = new List<Category>();
                var allCategories = GetCategorys();
                foreach (var c in allCategories)
                {
                    if (c.BaseId == thisCategory.Id)
                    {
                        categories.Add(c);
                    }
                }
                if(categories.Count != 0)
                {
                    listAddObservation.ItemsSource = null;
                    listAddObservation.ItemsSource = categories;
                }
                else
                {
                    categories.Add(thisCategory);
                    listAddObservation.ItemsSource = null;
                    listAddObservation.ItemsSource = categories;
                    finalCategory[0] = thisCategory;
                }
            }
        }

        private void listAddWeather_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var thisCategory = (Category)listAddWeather.SelectedItem;
            if (thisCategory != null)
            {
                List<Category> categories = new List<Category>();
                var allCategories = GetCategorys();
                foreach (var c in allCategories)
                {
                    if (c.BaseId == thisCategory.Id)
                    {
                        categories.Add(c);
                    }
                }
                if (categories.Count != 0)
                {
                    listAddWeather.ItemsSource = null;
                    listAddWeather.ItemsSource = categories;
                }
                else
                {
                    categories.Add(thisCategory);
                    listAddWeather.ItemsSource = null;
                    listAddWeather.ItemsSource = categories;
                    finalCategory[1] = thisCategory;
                }
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            
            var observer = (Observer)listNames.SelectedItem;
            try
            {
                
                Observation observation = new Observation()
                {
                    Observer_id = observer.Id,
                    Geolocation_id = 7
                };

                int observation_id = AddObservation(observation);
                var category1 = finalCategory[0];
                var category2 = finalCategory[1];
                if (category1 != null)
                {
                    if (txtAnimal.Text.Length > 0)
                    {
                        Measurement measurement = new Measurement()
                        {

                            Value = double.Parse(txtAnimal.Text),
                            //Observation_id = observation_id,
                            Category_id = category1.Id,
                        };
                        measurements.Add(measurement);
                    }
                    else
                    {
                        MessageBox.Show("Du glömde att fylla i värden");
                    }
                }
                if (category2 != null)
                {
                    if (txtSnowdepth.Text.Length > 0)
                    {
                        Measurement measurement1 = new Measurement()
                        {
                            Value = double.Parse(txtSnowdepth.Text),
                            //Observation_id = observation_id,
                            Category_id = category2.Id,
                        };
                        measurements.Add(measurement1);
                    }
                    else
                    {
                        MessageBox.Show("Du glömde att fylla i värden");
                    }
                }
                AddMeasurement(observation, measurements);
                measurements.Clear();
            }
            catch (PostgresException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listObservation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var measurement = (Observation)listObservation.SelectedItems;
            //var myMeasurement = GetMeasurement(measurement.Observer_id);
            //listObservation.ItemsSource = null;
            //listObservation.ItemsSource = myMeasurement;

        }
    }
}
