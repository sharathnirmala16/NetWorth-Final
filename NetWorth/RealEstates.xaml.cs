using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Syncfusion.UI.Xaml.Grid;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Transactions;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Networth
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RealEstates : Page
    {
        public ObservableCollection<RealEstate> realEstates { get; set; }
        public MainPage mainPage { get; set; }

        public RealEstates()
        {
            mainPage = new MainPage();

            this.InitializeComponent();

            InitializeForm();
            LoadData();
        }

        private void InitializeForm()
        {
            realEstates = new ObservableCollection<RealEstate>();
        }

        public async void LoadData()
        {
            try
            {
                string dateTimeFormat = "yyyy-MM-dd";
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = dateTimeFormat };
                using (HttpClient client = new HttpClient())
                {
                    var tempDict = new Dictionary<string, string>();
                    tempDict.Add("table name", "RealEstates");
                    string requestJSON = JsonConvert.SerializeObject(tempDict);

                    var response = await client.PostAsync(
                            mainPage.baseURL + "/table",
                            new StringContent(requestJSON, Encoding.UTF8, "application/json")
                        );
                    string reJSON = response.Content.ReadAsStringAsync().Result;
                    realEstates = JsonConvert.DeserializeObject<ObservableCollection<RealEstate>>(reJSON, dateTimeConverter);
                    RealEstatesGrid.ItemsSource = realEstates;
                    RealEstatesGrid.UpdateLayout();
                }
            }
            catch (Exception Ex) { mainPage.ShowMessage(Ex.ToString()); }
        }

        private void RealEstatesGrid_AutoGeneratingColumn(object sender, AutoGeneratingColumnArgs e)
        {
            if (e.Column.HeaderText == "EstateID")
            {
                e.Column.AllowEditing = false;
                e.Column.AllowResizing = true;
                e.Column.HeaderText = "Estate ID";
            }
            if (e.Column.HeaderText == "EstateName")
            {
                e.Column.AllowEditing = true;
                e.Column.AllowResizing = true;
                e.Column.AllowSorting = true;
                e.Column.AllowFiltering = true;
                e.Column.HeaderText = "Estate Name";
            }
            if (e.Column.HeaderText == "City")
            {
                e.Column.AllowEditing = true;
                e.Column.AllowResizing = true;
                e.Column.AllowSorting = true;
                e.Column.AllowFiltering = true;
                e.Column.HeaderText = "City";
            }
            if (e.Column.HeaderText == "Type")
            {
                GridComboBoxColumn gcbc = new GridComboBoxColumn();
                gcbc.MappingName = "Type";
                gcbc.HeaderText = "Type";
                gcbc.AllowEditing = true;
                gcbc.AllowResizing = true;
                gcbc.AllowFiltering = true;
                gcbc.AllowSorting = true;
                gcbc.ItemsSource = new List<string>() { "Agricultural Land", "Empty Land", "Residential Area", "Industrial Area" };
                e.Column = gcbc;
            }
            if (e.Column.HeaderText == "Size")
            {
                e.Column.AllowEditing = true;
                e.Column.AllowResizing = true;
                e.Column.AllowSorting = true;
                e.Column.AllowFiltering = true;
            }
            if (e.Column.HeaderText == "OpenDate")
            {
                GridDateTimeColumn gdtc = new GridDateTimeColumn();
                gdtc.MappingName = "OpenDate";
                gdtc.HeaderText = "Open Date";
                gdtc.AllowEditing = true;
                gdtc.AllowResizing = true;
                gdtc.AllowFiltering = true;
                gdtc.AllowSorting = true;
                gdtc.FormatString = "yyyy-MM-dd";
                e.Column = gdtc;
            }
            if (e.Column.HeaderText == "OpenPrice")
            {
                e.Column.AllowEditing = true;
                e.Column.AllowResizing = true;
                e.Column.HeaderText = "Open Price";
                e.Column.AllowSorting = true;
                e.Column.AllowFiltering = true;
            }
            if (e.Column.HeaderText == "CloseDate")
            {
                GridDateTimeColumn gdtc = new GridDateTimeColumn();
                gdtc.MappingName = "CloseDate";
                gdtc.HeaderText = "Close Date";
                gdtc.AllowEditing = true;
                gdtc.AllowResizing = true;
                gdtc.AllowFiltering = true;
                gdtc.AllowSorting = true;
                gdtc.FormatString = "yyyy-MM-dd";
                e.Column = gdtc;
            }
            if (e.Column.HeaderText == "ClosePrice")
            {
                e.Column.AllowEditing = true;
                e.Column.AllowResizing = true;
                e.Column.HeaderText = "Close Price";
                e.Column.AllowSorting = true;
                e.Column.AllowFiltering = true;
            }
            if (e.Column.HeaderText == "IsClosed")
            {
                e.Column.AllowEditing = true;
                e.Column.AllowResizing = true;
                e.Column.HeaderText = "Is Closed";
                e.Column.AllowSorting = true;
                e.Column.AllowFiltering = true;
            }
        }

        private async void RealEstatesGrid_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            int selRowIndex = RealEstatesGrid.SelectedIndex;
            if (selRowIndex >= 0)
            {
                RealEstate re = (RealEstate)RealEstatesGrid.SelectedItems[0];
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string dataJSON = JsonConvert.SerializeObject(re);
                        var response = await client.PutAsync(
                                mainPage.baseURL + "/insertestate",
                                new StringContent(dataJSON, Encoding.UTF8, "application/json")
                            );
                        string respJSON = response.Content.ReadAsStringAsync().Result;
                        int res = JsonConvert.DeserializeObject<Dictionary<string, int>>(respJSON)["response"];
                        if (res == -1) throw new Exception("Failed to update the database.");
                        else
                        {
                            realEstates[selRowIndex] = re;
                            RealEstatesGrid.UpdateLayout();
                        }
                    }
                }
                catch (Exception ex) { mainPage.ShowMessage(ex.ToString()); }
            }
        }

        private void RealEstatesGrid_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

        }

        private void RealEstatesGrid_CurrentCellValueChanged(object sender, CurrentCellValueChangedEventArgs e)
        {

        }

        private async void NewEstateButton_Click(object sender, RoutedEventArgs e)
        {
            RealEstate re = new RealEstate()
            {
                EstateID = mainPage.RandomAlphaNumeric(),
                EstateName = "Blank Estate",
                City = "Hyderabad",
                Type = "Residential Area",
                Size = 1000,
                OpenDate = DateTime.Now.Date,
                OpenPrice = 3000000,
                CloseDate = DateTime.Now.Date,
                ClosePrice = 3000000,
                IsClosed = false
            };

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string dataJSON = JsonConvert.SerializeObject(re);
                    var response = await client.PutAsync(
                            mainPage.baseURL + "/insertestate",
                            new StringContent(dataJSON, Encoding.UTF8, "application/json")
                        );
                    string respJSON = response.Content.ReadAsStringAsync().Result;
                    int res = JsonConvert.DeserializeObject<Dictionary<string, int>>(respJSON)["response"];
                    if (res == -1) throw new Exception("Failed to store to database.");
                    else
                    {
                        realEstates.Add(re);
                        RealEstatesGrid.UpdateLayout();
                    }
                }
            }
            catch (Exception ex) { mainPage.ShowMessage(ex.ToString()); }
        }

        private async void RemoveEstateButton_Click(object sender, RoutedEventArgs e)
        {
            int selRowIndex = RealEstatesGrid.SelectedIndex;
            if (selRowIndex >= 0)
            {
                RealEstate re = (RealEstate)RealEstatesGrid.SelectedItems[0];
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string dataJSON = JsonConvert.SerializeObject(re);
                        var response = await client.PutAsync(
                                mainPage.baseURL + "/deleteestate",
                                new StringContent(dataJSON, Encoding.UTF8, "application/json")
                            );
                        string respJSON = response.Content.ReadAsStringAsync().Result;
                        int res = JsonConvert.DeserializeObject<Dictionary<string, int>>(respJSON)["response"];
                        if (res == -1) throw new Exception("Failed to update the database.");
                        else
                        {
                            realEstates.RemoveAt(selRowIndex);
                            RealEstatesGrid.UpdateLayout();
                        }
                    }
                }
                catch (Exception ex) { mainPage.ShowMessage(ex.ToString()); }
            }
            else mainPage.ShowMessage("Please select a transaction to delete.");
        }
    }
}
