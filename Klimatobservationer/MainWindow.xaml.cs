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
        List<Category> finalCategories = new List<Category>();
        List<Measurement> measurements = new List<Measurement>();
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

        private void Add_Observer(object sender, RoutedEventArgs e)
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

        public void RemoveObserver(object sender, RoutedEventArgs e)
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

        private void ShowObserversWithoutObservations(object sender, RoutedEventArgs e)
        {
            var observers = GetDeletebleObservers();
            listNames.ItemsSource = null;
            listNames.ItemsSource = observers;
        }

        private void Show_Observations(object sender, RoutedEventArgs e)
        {
            UpdateObservations();
        }

        private void Delete_Observation(object sender, RoutedEventArgs e)
        {
            if (listObservation.SelectedItem is Observation)
            {
                var observation = (Observation)listObservation.SelectedItem;
                DeleteObservation(observation.Id);
                UpdateObservations();
            }

        }
                

        private void ShowCategories(object sender, RoutedEventArgs e)
        {
            UppdateCategoryList();
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
        private void SubmitObservation(object sender, RoutedEventArgs e)
        {

            var observer = (Observer)listNames.SelectedItem;
            try
            {
                Observation observation = new Observation()
                {
                    Observer_id = observer.Id,
                    Geolocation_id = 7
                };
                AddMeasurement(observation, measurements);
                measurements.Clear();
                finalCategories.Clear();
                listAddWeather.ItemsSource = null;
            }
            catch (PostgresException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Update(object sender, RoutedEventArgs e)
        {
            var measurement = (Measurement)listObservation.SelectedItem;
            double Value = double.Parse(txtUpdateValue.Text);
            measurement.Value = Value;
            try
            {
                UpdateMeasurement(measurement.Id, Value);
            }
            catch(PostgresException ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (listObservation.SelectedItem is Measurement)
            {
                var measurement1 = (Measurement)listObservation.SelectedItem;
                var allMeasurement = GetMeasurement(measurement1.Observation_id);
                listObservation.ItemsSource = null;
                listObservation.ItemsSource = allMeasurement;
            }
        }

        private void Add_Measurement(object sender, RoutedEventArgs e)
        {
            if (finalCategory[0] != null)
            {
                if (txtAnimal.Text.Length > 0)
                {
                    Measurement measurement = new Measurement()
                    {

                        Value = double.Parse(txtAnimal.Text),
                        Category_id = finalCategory[0].Id,
                    };
                    measurements.Add(measurement);
                }
                else
                {
                    MessageBox.Show("Du glömde att fylla i värden");
                }
            }
            finalCategories.Add(finalCategory[0]);
            finalCategory[0] = null;
            listAddWeather.ItemsSource = null;
            listAddWeather.ItemsSource = finalCategories;
            UppdateCategoryList();
        }
        public void UppdateCategoryList()
        {
            var allCategories = GetCategorys();
            List<Category> categories = new List<Category>();
            foreach (var c in allCategories)
            {
                if (c.Id <= 3)
                {
                    categories.Add(c);
                }
            }
            listAddObservation.ItemsSource = null;
            listAddObservation.ItemsSource = categories;
        }

        private void ShowAllObservers(object sender, RoutedEventArgs e)
        {
            UpdateList();
        }

        private void Show_Measurement(object sender, RoutedEventArgs e)
        {
            if (listObservation.SelectedItem is Observation)
            {
                var Observation = (Observation)listObservation.SelectedItem;
                var myMeasurement = GetMeasurement(Observation.Id);
                List<Measurement> measurements = new List<Measurement>();
                foreach (var c in myMeasurement)
                {
                    measurements.Add(c);
                }
                listObservation.ItemsSource = null;
                listObservation.ItemsSource = measurements;
            }
            else
            {
                var Observation = (Measurement)listObservation.SelectedItem;

            }
        }

        private void Delete_Measurment(object sender, RoutedEventArgs e)
        {
            if (listObservation.SelectedItem is Measurement)
            {
                var measurement = (Measurement)listObservation.SelectedItem;
                var id = measurement.Observation_id;
                DeleteMeasurement(measurement.Id);
                var allMeasurement = GetMeasurement(id);
                listObservation.ItemsSource = null;
                listObservation.ItemsSource = allMeasurement; 
            }
        }
    }
}
