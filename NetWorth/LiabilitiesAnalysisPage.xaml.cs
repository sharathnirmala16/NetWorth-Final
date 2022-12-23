using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Networth
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LiabilitiesAnalysisPage : Page
    {
        public MainPage mainPage { get; set; }
        public LiabilitiesAnalysisPage()
        {
            mainPage = new MainPage();

            this.InitializeComponent();

            //InitializeForm();
            LoadData();
        }
        /*
        private void InitializeForm()
        {

        }
        */

        public async void LoadData()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var tempDict = new Dictionary<string, string>();
                    tempDict.Add("query", mainPage.GetResourcesValues("LoanTypeDistributionQuery"));
                    tempDict.Add("X_label", "Category");
                    tempDict.Add("Y_label", "Percentage");
                    tempDict.Add("title", "Loan Type Distribution");
                    tempDict.Add("subfolder", "LiabilitiesCharts");
                    tempDict.Add("name", "LoanTypeDistribution.png");
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
                    tempDict.Add("query", mainPage.GetResourcesValues("LoanBarChartQuery"));
                    tempDict.Add("X_label", "Interest");
                    tempDict.Add("Y_label", "AmountRemaining");
                    tempDict.Add("color_label", "LoanName");
                    tempDict.Add("title", "Amount Remaining vs Interest %");
                    tempDict.Add("subfolder", "LiabilitiesCharts");
                    tempDict.Add("name", "AmountInterestBar.png");
                    string requestJSON = JsonConvert.SerializeObject(tempDict);

                    var response = await client.PutAsync(
                            mainPage.baseURL + "/bar",
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
    }
}
