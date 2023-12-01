using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;

namespace Assignment_5
{
    public partial class MainWindow : Window
    {
        private List<Building> buildings = new List<Building>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddBuildingButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(BuildingSizeTextBox.Text, out int size) || size < 1000 || size > 50000)
            {
                BuildingSizeTextBox.Background = Brushes.Red;
                MessageBox.Show("Invalid building size. Please enter a positive integer between 1000 and 50000 square feet.");
                return;
            }
            if (!int.TryParse(LightBulbsTextBox.Text, out int lightBulbs) || lightBulbs < 1 || lightBulbs > 20)
            {
                LightBulbsTextBox.Background = Brushes.Red;
                MessageBox.Show("Invalid number of light bulbs. Please enter a value between 1 and 20.");
                return;
            }

            if (!int.TryParse(OutletsTextBox.Text, out int outlets) || outlets < 1 || outlets > 50)
            {
                OutletsTextBox.Background = Brushes.Red;
                MessageBox.Show("Invalid number of outlets. Please enter a value between 1 and 50.");
                return;
            }
            if (!IsNumeric(CreditCardTextBox.Text) || CreditCardTextBox.Text.Length != 16)
            {
                CreditCardTextBox.Background = Brushes.Red;
                MessageBox.Show("Invalid credit card number. Must be a 16-digit numeric string.");
                return;
            }

            BuildingType buildingType = (BuildingType)BuildingTypeComboBox.SelectedIndex;
            string additionalFeatures = GetAdditionalFeatures(buildingType);

            Building building = new Building
            {
                Name = CustomerNameTextBox.Text,
                BuildingType = buildingType,
                Size = size,
                LightBulbs = lightBulbs,
                Outlets = outlets,
                CreditCardNumber = CreditCardTextBox.Text,
                AdditionalFeatures = additionalFeatures
            };

            buildings.Add(building);

            UpdateDataGrid();
            ClearInputFields();
        }

        private bool IsNumeric(string value)
        {
            return value.All(char.IsDigit);
        }

        private void SaveToFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = "building_data.xml";

                using (XmlWriter writer = XmlWriter.Create(filePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Building>));
                    serializer.Serialize(writer, buildings);
                }

                MessageBox.Show("File saved successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while saving file: {ex.Message}");
            }
        }

        private void LoadFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = "building_data.xml";
                if (File.Exists(filePath))
                {
                    using (XmlReader reader = XmlReader.Create(filePath))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<Building>));
                        buildings = (List<Building>)serializer.Deserialize(reader);
                    }

                    UpdateDataGrid();
                    MessageBox.Show("File loaded successfully!");
                }
                else
                {
                    MessageBox.Show("File missing. No building data loaded.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while loading file: {ex.Message}");
            }
        }

        private void UpdateDataGrid()
        {
            BuildingDataGrid.ItemsSource = null;
            BuildingDataGrid.ItemsSource = buildings;
        }

        private void ClearInputFields()
        {
            CustomerNameTextBox.Text = "";
            BuildingTypeComboBox.SelectedIndex = -1;
            BuildingSizeTextBox.Text = "";
            LightBulbsTextBox.Text = "";
            OutletsTextBox.Text = "";
            CreditCardTextBox.Text = "";

            CustomerNameTextBox.Background = Brushes.White;
            BuildingSizeTextBox.Background = Brushes.White;
            LightBulbsTextBox.Background = Brushes.White;
            OutletsTextBox.Background = Brushes.White;
            CreditCardTextBox.Background = Brushes.White;
        }

        private string GetAdditionalFeatures(BuildingType buildingType)
        {
            switch (buildingType)
            {
                case BuildingType.House:
                    return "Fire Alarms";
                case BuildingType.Barn:
                    return "Milking Equipment";
                case BuildingType.Garage:
                    return "Automatic Doors";
                default:
                    return "Additional features";
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchKey = SearchTextBox.Text.Trim();

            if (string.IsNullOrEmpty(searchKey))
            {
                MessageBox.Show("Please enter a search term.");
                UpdateDataGrid();
                return;
            }
            var searchResults = buildings.Where(b => b.Name.Contains(searchKey, StringComparison.OrdinalIgnoreCase)).ToList();
            BuildingDataGrid.ItemsSource = searchResults;
            SearchTextBox.Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BuildingDataGrid.ItemsSource = null;
        }

        private void CustomerNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

    public class Building
    {
        public string Name { get; set; } = "";
        public BuildingType BuildingType { get; set; }
        public int Size { get; set; }
        public int LightBulbs { get; set; }
        public int Outlets { get; set; }
        public string CreditCardNumber { get; set; } = "";
        public string AdditionalFeatures { get; set; } = "";

        public override string ToString()
        {
            return $"Customer Name: {Name}\nBuilding Type: {BuildingType}\nBuilding Size: {Size} sq. ft.\n" +
                   $"Number of Light Bulbs: {LightBulbs}\nNumber of Outlets: {Outlets}\nCredit Card Number: {CreditCardNumber}\n" +
                   $"Additional Features: {AdditionalFeatures}\n";
        }
    }

    public enum BuildingType
    {
        House,
        Barn,
        Garage
    }
}
