using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel.Resources.Core;
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
using Windows.ApplicationModel.Activation;
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System.Data;
using Windows.Graphics.Display;
using System.Reflection;
using Windows.Media.Capture.Frames;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Networth
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FinancialAssetsPage : Page
    {
        public ObservableCollection<FinancialAsset> financialAssets { get; set; }
        public List<string> categoryList { get; set; }
        public MainPage mainPage { get; set; }

        public FinancialAssetsPage()
        {
            mainPage = new MainPage();

            this.InitializeComponent();

            InitializeForm();
            LoadData();
        }

        private void InitializeForm()
        {
            financialAssets = new ObservableCollection<FinancialAsset>();
            categoryList = new List<string>()
            {
                "Stocks/ETF",
                "Cryptocurrency",
                "Gold",
                "Forex",
                "Mutual Fund"
            };
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
                    tempDict.Add("table name", "FinancialAssets");
                    string requestJSON = JsonConvert.SerializeObject(tempDict);

                    var response = await client.PostAsync(
                            mainPage.baseURL + "/table",
                            new StringContent(requestJSON, Encoding.UTF8, "application/json")
                        );
                    string faJSON = response.Content.ReadAsStringAsync().Result;
                    financialAssets = JsonConvert.DeserializeObject<ObservableCollection<FinancialAsset>>(faJSON, dateTimeConverter);
                    FinancialAssetsGrid.ItemsSource = financialAssets;
                    FinancialAssetsGrid.UpdateLayout();
                }
            }
            catch (Exception Ex) { mainPage.ShowMessage(Ex.ToString()); }
        }

        private void FinancialAssetsGrid_AutoGeneratingColumn(object sender, AutoGeneratingColumnArgs e)
        {
            if (e.Column.HeaderText == "AssetID")
            {
                e.Column.AllowEditing = false;
                e.Column.AllowResizing = true;
                e.Column.HeaderText = "Asset ID";
            }
            if (e.Column.HeaderText == "Category")
            {
                GridComboBoxColumn gcbc = new GridComboBoxColumn();
                gcbc.MappingName = "Category";
                gcbc.HeaderText = "Category";
                gcbc.AllowEditing = true;
                gcbc.AllowResizing = true;
                gcbc.AllowFiltering = true;
                gcbc.AllowSorting = true;
                gcbc.ItemsSource = categoryList;
                e.Column = gcbc;
            }
            if (e.Column.HeaderText == "Name")
            {
                e.Column.AllowEditing = true;
                e.Column.AllowResizing = true;
                e.Column.AllowSorting = true;
                e.Column.AllowFiltering = true;
            }
            if (e.Column.HeaderText == "Shares")
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

        private async void NewAssetButton_Click(object sender, RoutedEventArgs e)
        {
            FinancialAsset fa = new FinancialAsset()
            {
                AssetID = mainPage.RandomAlphaNumeric(),
                Category = "Stocks/ETF",
                Name = "ADANIENT.NS",
                Shares = 0,
                OpenDate = DateTime.Now.Date,
                OpenPrice = 1000,
                CloseDate = DateTime.Now.Date,
                ClosePrice = 1000,
                IsClosed = false,
            };

            try
            {
                using (HttpClient client = new HttpClient())
                { 
                    string dataJSON = JsonConvert.SerializeObject(fa);
                    var response = await client.PutAsync(
                            mainPage.baseURL + "/insertfinancialasset",
                            new StringContent(dataJSON, Encoding.UTF8, "application/json")
                        );
                    string respJSON = response.Content.ReadAsStringAsync().Result;
                    int res = JsonConvert.DeserializeObject<Dictionary<string, int>>(respJSON)["response"];
                    if (res == -1) throw new Exception("Failed to store to database.");
                    else
                    {
                        financialAssets.Add(fa);
                        FinancialAssetsGrid.UpdateLayout();
                    }
                }
            }
            catch (Exception ex) { mainPage.ShowMessage(ex.ToString()); }
        }

        private async void FinancialAssetsGrid_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            int selRowIndex = FinancialAssetsGrid.SelectedIndex;
            if (selRowIndex >= 0)
            {
                FinancialAsset fa = (FinancialAsset)FinancialAssetsGrid.SelectedItems[0];
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string dataJSON = JsonConvert.SerializeObject(fa);
                        var response = await client.PutAsync(
                                mainPage.baseURL + "/insertfinancialasset",
                                new StringContent(dataJSON, Encoding.UTF8, "application/json")
                            );
                        string respJSON = response.Content.ReadAsStringAsync().Result;
                        int res = JsonConvert.DeserializeObject<Dictionary<string, int>>(respJSON)["response"];
                        if (res == -1) throw new Exception("Failed to update the database.");
                        else
                        {
                            financialAssets[selRowIndex] = fa;
                            FinancialAssetsGrid.UpdateLayout();
                        }
                    }
                }
                catch (Exception ex) { mainPage.ShowMessage(ex.ToString()); }
            }
        }

        private void FinancialAssetsGrid_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

        }

        private void FinancialAssetsGrid_CurrentCellValueChanged(object sender, CurrentCellValueChangedEventArgs e)
        {

        }

        private async void RemoveAssetButton_Click(object sender, RoutedEventArgs e)
        {
            int selRowIndex = FinancialAssetsGrid.SelectedIndex;
            if (selRowIndex >= 0)
            {
                FinancialAsset fa = (FinancialAsset)FinancialAssetsGrid.SelectedItems[0];
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string dataJSON = JsonConvert.SerializeObject(fa);
                        var response = await client.PutAsync(
                                mainPage.baseURL + "/deletefinancialasset",
                                new StringContent(dataJSON, Encoding.UTF8, "application/json")
                            );
                        string respJSON = response.Content.ReadAsStringAsync().Result;
                        int res = JsonConvert.DeserializeObject<Dictionary<string, int>>(respJSON)["response"];
                        if (res == -1) throw new Exception("Failed to update the database.");
                        else
                        {
                            financialAssets.RemoveAt(selRowIndex);
                            FinancialAssetsGrid.UpdateLayout();
                        }
                    }
                }
                catch (Exception ex) { mainPage.ShowMessage(ex.ToString()); }
            }
            else mainPage.ShowMessage("Please select an asset to delete.");
        }

        private async void UpdateOpenPricesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var postDict = new Dictionary<string, Dictionary<string, string>>();
                foreach (FinancialAsset row in financialAssets)
                {
                    var val = new Dictionary<string, string>();
                    val.Add("ticker", row.Name);
                    val.Add("date", row.OpenDate.Date.ToString("yyyy-MM-dd"));
                    postDict.Add(row.AssetID, val);
                }
                using (HttpClient client = new HttpClient())
                {
                    string dataJSON = JsonConvert.SerializeObject(postDict);
                    //mainPage.ShowMessage(dataJSON, "Test");
                    var response = await client.PutAsync(
                            mainPage.baseURL + "/values/open",
                            new StringContent(dataJSON, Encoding.UTF8, "application/json")
                        );
                    string respJSON = response.Content.ReadAsStringAsync().Result;
                    var res = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(respJSON)["failed"];

                    if (res.Count > 0)
                    {
                        string msg = "Failed to find prices for assets given below:\n";
                        foreach (string id in res) msg += $"{id}, ";
                        mainPage.ShowMessage(msg.Substring(0, msg.Length - 2), "Unable to find prices");
                    }

                    LoadData();
                }
            }
            catch (Exception ex) { mainPage.ShowMessage(ex.ToString()); }
        }

        private async void UpdateClosePricesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var postDict = new Dictionary<string, Dictionary<string, string>>();
                foreach (FinancialAsset row in financialAssets)
                {
                    var val = new Dictionary<string, string>();
                    val.Add("ticker", row.Name);
                    val.Add("date", row.CloseDate.Date.ToString("yyyy-MM-dd"));
                    postDict.Add(row.AssetID, val);
                }
                using (HttpClient client = new HttpClient())
                {
                    string dataJSON = JsonConvert.SerializeObject(postDict);
                    var response = await client.PutAsync(
                            mainPage.baseURL + "/values/close",
                            new StringContent(dataJSON, Encoding.UTF8, "application/json")
                        );
                    string respJSON = response.Content.ReadAsStringAsync().Result;
                    var res = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(respJSON)["failed"];

                    if (res.Count > 0)
                    {
                        string msg = "Failed to find prices for assets given below:\n";
                        foreach (string id in res) msg += $"{id}, ";
                        mainPage.ShowMessage(msg.Substring(0, msg.Length - 2), "Unable to find prices");
                    }

                    LoadData();
                }
            }
            catch (Exception ex) { mainPage.ShowMessage(ex.ToString()); }
        }
    }
}
