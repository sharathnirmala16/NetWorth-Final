using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Syncfusion.UI.Xaml.Grid;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Networth
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SpendingPage : Page
    {
        public ObservableCollection<Transaction> transactions { get; set; }
        public MainPage mainPage { get; set; }
        public SpendingPage()
        {
            mainPage = new MainPage();

            this.InitializeComponent();

            InitializeForm();
            LoadData();
        }

        private void InitializeForm()
        {
            transactions = new ObservableCollection<Transaction>();
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
                    tempDict.Add("table name", "Spending");
                    string requestJSON = JsonConvert.SerializeObject(tempDict);

                    var response = await client.PostAsync(
                            mainPage.baseURL + "/table",
                            new StringContent(requestJSON, Encoding.UTF8, "application/json")
                        );
                    string trJSON = response.Content.ReadAsStringAsync().Result;
                    transactions = JsonConvert.DeserializeObject<ObservableCollection<Transaction>>(trJSON, dateTimeConverter);
                    TransactionsGrid.ItemsSource = transactions;
                    TransactionsGrid.UpdateLayout();
                }
            }
            catch (Exception Ex) { mainPage.ShowMessage(Ex.ToString()); }
        }

        private void TransactionsGrid_AutoGeneratingColumn(object sender, AutoGeneratingColumnArgs e)
        {
            if (e.Column.HeaderText == "TransactionID")
            {
                e.Column.AllowEditing = false;
                e.Column.AllowResizing = true;
                e.Column.HeaderText = "Transaction ID";
            }
            if (e.Column.HeaderText == "Credit")
            {
                GridComboBoxColumn gcbc = new GridComboBoxColumn();
                gcbc.MappingName = "Credit";
                gcbc.HeaderText = "Credit";
                gcbc.AllowEditing = true;
                gcbc.AllowResizing = true;
                gcbc.AllowFiltering = true;
                gcbc.AllowSorting = true;
                gcbc.ItemsSource = new List<string>() { "Credit", "Debit" };
                e.Column = gcbc;
            }
            if (e.Column.HeaderText == "Amount")
            {
                e.Column.AllowEditing = true;
                e.Column.AllowResizing = true;
                e.Column.AllowSorting = true;
                e.Column.AllowFiltering = true;
                e.Column.HeaderText = "Amount";
            }
            if (e.Column.HeaderText == "Purpose")
            {
                e.Column.AllowEditing = true;
                e.Column.AllowResizing = true;
                e.Column.AllowSorting = true;
                e.Column.AllowFiltering = true;
                e.Column.HeaderText = "Purpose";
            }
            if (e.Column.HeaderText == "Date")
            {
                GridDateTimeColumn gdtc = new GridDateTimeColumn();
                gdtc.MappingName = "Date";
                gdtc.HeaderText = "Date";
                gdtc.AllowEditing = true;
                gdtc.AllowResizing = true;
                gdtc.AllowFiltering = true;
                gdtc.AllowSorting = true;
                gdtc.FormatString = "yyyy-MM-dd";
                e.Column = gdtc;
            }
        }

        private async void TransactionsGrid_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            int selRowIndex = TransactionsGrid.SelectedIndex;
            if (selRowIndex >= 0)
            {
                Transaction tr = (Transaction)TransactionsGrid.SelectedItems[0];
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string dataJSON = JsonConvert.SerializeObject(tr);
                        var response = await client.PutAsync(
                                mainPage.baseURL + "/inserttransaction",
                                new StringContent(dataJSON, Encoding.UTF8, "application/json")
                            );
                        string respJSON = response.Content.ReadAsStringAsync().Result;
                        int res = JsonConvert.DeserializeObject<Dictionary<string, int>>(respJSON)["response"];
                        if (res == -1) throw new Exception("Failed to update the database.");
                        else
                        {
                            transactions[selRowIndex] = tr;
                            TransactionsGrid.UpdateLayout();
                        }
                    }
                }
                catch (Exception ex) { mainPage.ShowMessage(ex.ToString()); }
            }
        }

        private void TransactionsGrid_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

        }

        private void TransactionsGrid_CurrentCellValueChanged(object sender, CurrentCellValueChangedEventArgs e)
        {

        }

        private async void NewTransactionButton_Click(object sender, RoutedEventArgs e)
        {
            Transaction tr = new Transaction()
            {
                TransactionID = mainPage.RandomAlphaNumeric(),
                Credit = "Credit",
                Amount = 0.0,
                Purpose = "",
                Date = DateTime.Now.Date,
            };

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string dataJSON = JsonConvert.SerializeObject(tr);
                    var response = await client.PutAsync(
                            mainPage.baseURL + "/inserttransaction",
                            new StringContent(dataJSON, Encoding.UTF8, "application/json")
                        );
                    string respJSON = response.Content.ReadAsStringAsync().Result;
                    int res = JsonConvert.DeserializeObject<Dictionary<string, int>>(respJSON)["response"];
                    if (res == -1) throw new Exception("Failed to store to database.");
                    else
                    {
                        transactions.Add(tr);
                        TransactionsGrid.UpdateLayout();
                    }
                }
            }
            catch (Exception ex) { mainPage.ShowMessage(ex.ToString()); }
        }

        private async void RemoveTransactionButton_Click(object sender, RoutedEventArgs e)
        {
            int selRowIndex = TransactionsGrid.SelectedIndex;
            if (selRowIndex >= 0)
            {
                Transaction tr = (Transaction)TransactionsGrid.SelectedItems[0];
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string dataJSON = JsonConvert.SerializeObject(tr);
                        var response = await client.PutAsync(
                                mainPage.baseURL + "/deletetransaction",
                                new StringContent(dataJSON, Encoding.UTF8, "application/json")
                            );
                        string respJSON = response.Content.ReadAsStringAsync().Result;
                        int res = JsonConvert.DeserializeObject<Dictionary<string, int>>(respJSON)["response"];
                        if (res == -1) throw new Exception("Failed to update the database.");
                        else
                        {
                            transactions.RemoveAt(selRowIndex);
                            TransactionsGrid.UpdateLayout();
                        }
                    }
                }
                catch (Exception ex) { mainPage.ShowMessage(ex.ToString()); }
            }
            else mainPage.ShowMessage("Please select a transaction to delete.");
        }
    }
}
