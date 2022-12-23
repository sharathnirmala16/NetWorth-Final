using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Transactions;
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
    public sealed partial class AssetsAnalysisPage : Page
    {
        public MainPage mainPage { get; set; }

        public AssetsAnalysisPage()
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
                    tempDict.Add("query", mainPage.GetResourcesValues("CityDistributionQuery"));
                    tempDict.Add("X_label", "City");
                    tempDict.Add("Y_label", "Percentage");
                    tempDict.Add("title", "Estate City Distribution");
                    tempDict.Add("subfolder", "AssetsCharts");
                    tempDict.Add("name", "CityDistribution.png");
                    string requestJSON = JsonConvert.SerializeObject(tempDict);

                    var response = await client.PutAsync(
                            mainPage.baseURL + "/pie",
                            new StringContent(requestJSON, Encoding.UTF8, "application/json")
                        );
                    string sdJSON = response.Content.ReadAsStringAsync().Result;
                    string resp = JsonConvert.DeserializeObject<Dictionary<string, string>>(sdJSON)["response"];
                    if (resp != "FAIL") DistributionChart5.Source = new BitmapImage(new Uri(resp));
                    else throw new Exception("Could not create chart based on data.");
                }

                using (HttpClient client = new HttpClient())
                {
                    var tempDict = new Dictionary<string, string>();
                    tempDict.Add("query", mainPage.GetResourcesValues("EstateUnitValueQuery"));
                    tempDict.Add("X_label", "EstateName");
                    tempDict.Add("Y_label", "Price per Unit Area");
                    tempDict.Add("color_label", "Price per Unit Area");
                    tempDict.Add("title", "Estate Unit Area Value");
                    tempDict.Add("subfolder", "AssetsCharts");
                    tempDict.Add("name", "EstatesUnitPrice.png");
                    string requestJSON = JsonConvert.SerializeObject(tempDict);

                    var response = await client.PutAsync(
                            mainPage.baseURL + "/bar",
                            new StringContent(requestJSON, Encoding.UTF8, "application/json")
                        );
                    string sdJSON = response.Content.ReadAsStringAsync().Result;
                    string resp = JsonConvert.DeserializeObject<Dictionary<string, string>>(sdJSON)["response"];
                    if (resp != "FAIL") DistributionChart4.Source = new BitmapImage(new Uri(resp));
                    else throw new Exception("Could not create chart based on data.");
                }

                using (HttpClient client = new HttpClient())
                {
                    var tempDict = new Dictionary<string, string>();
                    tempDict.Add("query", mainPage.GetResourcesValues("EstateDistributionQuery"));
                    tempDict.Add("X_label", "EstateName");
                    tempDict.Add("Y_label", "Percentage");
                    tempDict.Add("title", "Estate Distribution");
                    tempDict.Add("subfolder", "AssetsCharts");
                    tempDict.Add("name", "EstateDistribution.png");
                    string requestJSON = JsonConvert.SerializeObject(tempDict);

                    var response = await client.PutAsync(
                            mainPage.baseURL + "/pie",
                            new StringContent(requestJSON, Encoding.UTF8, "application/json")
                        );
                    string sdJSON = response.Content.ReadAsStringAsync().Result;
                    string resp = JsonConvert.DeserializeObject<Dictionary<string, string>>(sdJSON)["response"];
                    if (resp != "FAIL") DistributionChart6.Source = new BitmapImage(new Uri(resp));
                    else throw new Exception("Could not create chart based on data.");
                }

                using (HttpClient client = new HttpClient())
                {
                    var tempDict = new Dictionary<string, string>();
                    tempDict.Add("query", mainPage.GetResourcesValues("AllAssetsDistributionQuery"));
                    tempDict.Add("X_label", "Category");
                    tempDict.Add("Y_label", "Percentage");
                    tempDict.Add("title", "All Assets Distribution");
                    tempDict.Add("subfolder", "AssetsCharts");
                    tempDict.Add("name", "AllAssetsDistribution.png");
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

                using (HttpClient client = new HttpClient())
                {
                    var tempDict = new Dictionary<string, string>();
                    tempDict.Add("title", "Trackable Assets Value");
                    tempDict.Add("subfolder", "AssetsCharts");
                    tempDict.Add("name", "TrackableAssetsValue.png");
                    string requestJSON = JsonConvert.SerializeObject(tempDict);

                    var response = await client.PutAsync(
                            mainPage.baseURL + "/track",
                            new StringContent(requestJSON, Encoding.UTF8, "application/json")
                        );
                    string sdJSON = response.Content.ReadAsStringAsync().Result;
                    string resp = JsonConvert.DeserializeObject<Dictionary<string, string>>(sdJSON)["response"];
                    if (resp != "FAIL") DistributionChart1.Source = new BitmapImage(new Uri(resp));
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
