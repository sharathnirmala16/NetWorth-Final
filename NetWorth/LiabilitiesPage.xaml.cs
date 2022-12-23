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
    public sealed partial class LiabilitiesPage : Page
    {
        public ObservableCollection<Liability> liabilities { get; set; }
        public MainPage mainPage { get; set; }
        public LiabilitiesPage()
        {
            mainPage = new MainPage();

            this.InitializeComponent();

            InitializeForm();
            LoadData();
        }

        private void InitializeForm()
        {
            liabilities = new ObservableCollection<Liability>();
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
                    tempDict.Add("table name", "Liabilities");
                    string requestJSON = JsonConvert.SerializeObject(tempDict);

                    var response = await client.PostAsync(
                            mainPage.baseURL + "/table",
                            new StringContent(requestJSON, Encoding.UTF8, "application/json")
                        );
                    string lbJSON = response.Content.ReadAsStringAsync().Result;
                    liabilities = JsonConvert.DeserializeObject<ObservableCollection<Liability>>(lbJSON, dateTimeConverter);
                    LiabilitiesGrid.ItemsSource = liabilities;
                    LiabilitiesGrid.UpdateLayout();
                }
            }
            catch (Exception Ex) { mainPage.ShowMessage(Ex.ToString()); }
        }

        private async void NewLoanButton_Click(object sender, RoutedEventArgs e)
        {
            Liability lb = new Liability()
            {
                LoanID = mainPage.RandomAlphaNumeric(),
                LoanName = "Blank Loan",
                PrincipleAmount = 100000,
                Interest = 10,
                OpenDate = DateTime.Now.Date,
                AmountRemaining = 500000,
                CloseDate = DateTime.Now.Date,
            };

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string dataJSON = JsonConvert.SerializeObject(lb);
                    var response = await client.PutAsync(
                            mainPage.baseURL + "/insertloan",
                            new StringContent(dataJSON, Encoding.UTF8, "application/json")
                        );
                    string respJSON = response.Content.ReadAsStringAsync().Result;
                    int res = JsonConvert.DeserializeObject<Dictionary<string, int>>(respJSON)["response"];
                    if (res == -1) throw new Exception("Failed to store to database.");
                    else
                    {
                        liabilities.Add(lb);
                        LiabilitiesGrid.UpdateLayout();
                    }
                }
            }
            catch (Exception ex) { mainPage.ShowMessage(ex.ToString()); }
        }

        private async void RemoveLoanButton_Click(object sender, RoutedEventArgs e)
        {
            int selRowIndex = LiabilitiesGrid.SelectedIndex;
            if (selRowIndex >= 0)
            {
                Liability lb = (Liability)LiabilitiesGrid.SelectedItems[0];
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string dataJSON = JsonConvert.SerializeObject(lb);
                        var response = await client.PutAsync(
                                mainPage.baseURL + "/deleteloan",
                                new StringContent(dataJSON, Encoding.UTF8, "application/json")
                            );
                        string respJSON = response.Content.ReadAsStringAsync().Result;
                        int res = JsonConvert.DeserializeObject<Dictionary<string, int>>(respJSON)["response"];
                        if (res == -1) throw new Exception("Failed to update the database.");
                        else
                        {
                            liabilities.RemoveAt(selRowIndex);
                            LiabilitiesGrid.UpdateLayout();
                        }
                    }
                }
                catch (Exception ex) { mainPage.ShowMessage(ex.ToString()); }
            }
            else mainPage.ShowMessage("Please select a transaction to delete.");
        }

        private void LiabilitiesGrid_AutoGeneratingColumn(object sender, AutoGeneratingColumnArgs e)
        {
            if (e.Column.HeaderText == "LoanID")
            {
                e.Column.AllowEditing = false;
                e.Column.AllowResizing = true;
                e.Column.HeaderText = "Loan ID";
            }
            if (e.Column.HeaderText == "LoanName")
            {
                e.Column.AllowEditing = true;
                e.Column.AllowResizing = true;
                e.Column.AllowSorting = true;
                e.Column.AllowFiltering = true;
                e.Column.HeaderText = "Loan Name";
            }
            if (e.Column.HeaderText == "PrincipleAmount")
            {
                e.Column.AllowEditing = true;
                e.Column.AllowResizing = true;
                e.Column.AllowSorting = true;
                e.Column.AllowFiltering = true;
                e.Column.HeaderText = "PrincipleAmount";
            }
            if (e.Column.HeaderText == "Interest")
            {
                e.Column.AllowEditing = true;
                e.Column.AllowResizing = true;
                e.Column.AllowSorting = true;
                e.Column.AllowFiltering = true;
                e.Column.HeaderText = "Interest";
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
            if (e.Column.HeaderText == "AmountRemaining")
            {
                e.Column.AllowEditing = true;
                e.Column.AllowResizing = true;
                e.Column.AllowSorting = true;
                e.Column.AllowFiltering = true;
                e.Column.HeaderText = "Amount Remaining";
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
        }

        private async void LiabilitiesGrid_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            int selRowIndex = LiabilitiesGrid.SelectedIndex;
            if (selRowIndex >= 0)
            {
                Liability lb = (Liability)LiabilitiesGrid.SelectedItems[0];
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string dataJSON = JsonConvert.SerializeObject(lb);
                        var response = await client.PutAsync(
                                mainPage.baseURL + "/insertloan",
                                new StringContent(dataJSON, Encoding.UTF8, "application/json")
                            );
                        string respJSON = response.Content.ReadAsStringAsync().Result;
                        int res = JsonConvert.DeserializeObject<Dictionary<string, int>>(respJSON)["response"];
                        if (res == -1) throw new Exception("Failed to update the database.");
                        else
                        {
                            liabilities[selRowIndex] = lb;
                            LiabilitiesGrid.UpdateLayout();
                        }
                    }
                }
                catch (Exception ex) { mainPage.ShowMessage(ex.ToString()); }
            }
        }

        private void LiabilitiesGrid_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

        }

        private void LiabilitiesGrid_CurrentCellValueChanged(object sender, CurrentCellValueChangedEventArgs e)
        {

        }
    }
}
