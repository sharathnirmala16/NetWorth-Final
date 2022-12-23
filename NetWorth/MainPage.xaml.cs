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
using Syncfusion.UI.Xaml.Controls.Input;
using Windows.UI.Popups;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Resources.Core;
using Microsoft.Data.Sqlite;
using Windows.Storage;
using System.Data.Common;
using Newtonsoft.Json;
using System.Net.Http;
using Windows.Security.Cryptography.Core;
using System.Text;
using Newtonsoft.Json.Converters;
using System.Reflection;
using Windows.Graphics.Display;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Networth
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ResourceContext defaultContextForCurrentView;
        public ResourceMap stringResourcesResourceMap;
        public string dbName;
        public string dbPath;
        public string baseURL;

        public MainPage()
        {
            InitializeForm();
            this.InitializeComponent();
            InitializeDatabase();
        }

        private void InitializeForm()
        {
            //Syncfusion License Key
            defaultContextForCurrentView = ResourceContext.GetForCurrentView();
            stringResourcesResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(GetResourcesValues("SFRL"));

            baseURL = "http://127.0.0.1:8080";
            dbPath = "";
            dbName = "";
        }

        public string GetResourcesValues(string key)
        {
            return stringResourcesResourceMap.GetValue(key, defaultContextForCurrentView).ValueAsString;
        }

        public async void ShowMessage(string message, string title = "Error")
        {
            await new MessageDialog(message, title).ShowAsync();
        }

        public async void InitializeDatabase()
        {
            dbName = "ApplicationDB.db";
            await ApplicationData.Current.LocalFolder.CreateFileAsync(dbName, CreationCollisionOption.OpenIfExists);
            dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbName);

            try
            {
                using (SqliteConnection db = new SqliteConnection($"Filename={dbPath}"))
                {
                    db.Open();
                    SqliteCommand createFATable = new SqliteCommand(GetResourcesValues("CreateFinancialAssetsTableCommand"), db);
                    SqliteCommand createTRTable = new SqliteCommand(GetResourcesValues("CreateSpendingTableCommand"), db);
                    SqliteCommand createRETable = new SqliteCommand(GetResourcesValues("CreateRealEstatesTableCommand"), db);
                    SqliteCommand createLBTable = new SqliteCommand(GetResourcesValues("CreateLiabilitiesTableCommand"), db);
                    createFATable.ExecuteReader();
                    createTRTable.ExecuteReader();
                    createRETable.ExecuteReader();
                    createLBTable.ExecuteReader();
                }

                using (HttpClient client = new HttpClient())
                {
                    var tempDict = new Dictionary<string, string>();
                    tempDict.Add("path", dbPath);
                    tempDict.Add("folderpath", ApplicationData.Current.LocalFolder.Path);
                    string requestJSON = JsonConvert.SerializeObject(tempDict);

                    var response = await client.PostAsync(
                            baseURL + "/DBpath",
                            new StringContent(requestJSON, Encoding.UTF8, "application/json")
                        );
                    string responseJSON = response.Content.ReadAsStringAsync().Result;
                    var deserializedDict = JsonConvert.DeserializeObject<Dictionary<string, bool>>(responseJSON);
                    if (!deserializedDict["exists"]) throw new Exception("API cannot see the database");
                }

            }
            catch (Exception ex) { ShowMessage(ex.Message); }
        }

        public string RandomAlphaNumeric(int size = 8)
        {
            string res = "";
            for (int i = 0; i < size; i++)
            {
                if (Convert.ToBoolean(new Random().Next(0, 2))) res += (char)new Random().Next(65, 90);
                else res += (char)new Random().Next(48, 58);
            }
            return res;
        }

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(FinancialAssetsPage));
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem navItem = args.SelectedItem as NavigationViewItem;

            switch (navItem.Tag.ToString())
            {
                case "FinancialAssetsPage":
                    ContentFrame.Navigate(typeof(FinancialAssetsPage));
                    break;

                case "SpendingPage":
                    ContentFrame.Navigate(typeof(SpendingPage));
                    break;
                case "RealEstatesPage":
                    ContentFrame.Navigate(typeof(RealEstates));
                    break;
                case "LiabilitiesPage":
                    ContentFrame.Navigate(typeof(LiabilitiesPage));
                    break;
                case "SpendingAnalysisPage":
                    ContentFrame.Navigate(typeof(SpendingAnalysisPage));
                    break;
                case "AssetsAnalysisPage":
                    ContentFrame.Navigate(typeof(AssetsAnalysisPage));
                    break;
                case "LiabilitiesAnalysisPage":
                    ContentFrame.Navigate(typeof(LiabilitiesAnalysisPage));
                    break;
                case "HowToUsePage":
                    ContentFrame.Navigate(typeof(HowToUse));
                    break;
                case "NetWorthPage":
                    ContentFrame.Navigate(typeof(NetWorthPage));
                    break;
            }
        }
    }
}
