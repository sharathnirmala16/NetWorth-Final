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
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Networth
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NetWorthPage : Page
    {
        public ObservableCollection<NetWorthComponent> netWorthComponents { get; set; }
        public MainPage mainPage { get; set; }
        public NetWorthPage()
        {
            mainPage = new MainPage();

            this.InitializeComponent();

            LoadData();
        }
        public async void LoadData()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var tempDict = new Dictionary<string, string>();
                    tempDict.Add("query", mainPage.GetResourcesValues("NetWorthQuery"));
                    string requestJSON = JsonConvert.SerializeObject(tempDict);

                    var response = await client.PostAsync(
                            mainPage.baseURL + "/customquery",
                            new StringContent(requestJSON, Encoding.UTF8, "application/json")
                        );
                    string faJSON = response.Content.ReadAsStringAsync().Result;
                    netWorthComponents = JsonConvert.DeserializeObject<ObservableCollection<NetWorthComponent>>(faJSON);
                    NetWorthGrid.ItemsSource = netWorthComponents;
                    NetWorthGrid.UpdateLayout();
                }

                double totalWorth = 0, netWorth = 0;
                foreach (NetWorthComponent row in netWorthComponents)
                {
                    totalWorth += row.TotalValue;
                    if (row.Category != "Liabilities") netWorth += row.TotalValue;
                    else netWorth -= row.TotalValue;
                }

                TotalWorthLabel.Text = $"Total Worth = Rs.{totalWorth}";
                NetWorthLabel.Text = $"Net Worth = Rs.{netWorth}";

                using (HttpClient client = new HttpClient())
                {
                    var tempDict = new Dictionary<string, string>();
                    tempDict.Add("query", mainPage.GetResourcesValues("TotalWorthDistributionQuery"));
                    tempDict.Add("X_label", "Category");
                    tempDict.Add("Y_label", "Percentage");
                    tempDict.Add("title", "Total Worth Distribution");
                    tempDict.Add("subfolder", "WorthCharts");
                    tempDict.Add("name", "TotalWorthDistribution.png");
                    string requestJSON = JsonConvert.SerializeObject(tempDict);

                    var response = await client.PutAsync(
                            mainPage.baseURL + "/pie",
                            new StringContent(requestJSON, Encoding.UTF8, "application/json")
                        );
                    string sdJSON = response.Content.ReadAsStringAsync().Result;
                    string resp = JsonConvert.DeserializeObject<Dictionary<string, string>>(sdJSON)["response"];
                    if (resp != "FAIL") DistributionChart1.Source = new BitmapImage(new Uri(resp));
                    else throw new Exception("Could not create chart based on data.");
                }
                
                using (HttpClient client = new HttpClient())
                {
                    var tempDict = new Dictionary<string, string>();
                    tempDict.Add("query", mainPage.GetResourcesValues("NetWorthDistributionQuery"));
                    tempDict.Add("X_label", "Category");
                    tempDict.Add("Y_label", "Percentage");
                    tempDict.Add("title", "Net Worth Distribution");
                    tempDict.Add("subfolder", "WorthCharts");
                    tempDict.Add("name", "NetWorthDistribution.png");
                    string requestJSON = JsonConvert.SerializeObject(tempDict);

                    var response = await client.PutAsync(
                            mainPage.baseURL + "/pie",
                            new StringContent(requestJSON, Encoding.UTF8, "application/json")
                        );
                    string sdJSON = response.Content.ReadAsStringAsync().Result;
                    string resp = JsonConvert.DeserializeObject<Dictionary<string, string>>(sdJSON)["response"];
                    if (resp != "FAIL") DistributionChart2.Source = new BitmapImage(new Uri(resp));
                    else throw new Exception("Could not create chart based on data.");
                }
            }
            catch (Exception Ex) { mainPage.ShowMessage(Ex.ToString()); }
        }
        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void NetWorthGrid_AutoGeneratingColumn(object sender, Syncfusion.UI.Xaml.Grid.AutoGeneratingColumnArgs e)
        {
            if (e.Column.HeaderText == "Category")
            {
                e.Column.AllowEditing = false;
                e.Column.AllowResizing = true;
                e.Column.HeaderText = "Asset ID";
            }
            if (e.Column.HeaderText == "TotalValue")
            {
                e.Column.AllowEditing = false;
                e.Column.AllowResizing = true;
                e.Column.HeaderText = "Total Value";
            }
        }

        private void NetWorthGrid_CurrentCellValueChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellValueChangedEventArgs e)
        {

        }
    }
}
