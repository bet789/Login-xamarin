using Login.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class frm_Content : ContentPage
    {
        
        public frm_Content()
        {
            InitializeComponent();
            loadpage();
        }
        public static string GetUserCountryByIp(string ip)
        {
            IpInfo ipInfo = new IpInfo();
            try
            {
                string info = new WebClient().DownloadString("http://ipinfo.io/" + ip);
                ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
            }
            catch (Exception)
            {
                ipInfo.Country = null;
            }
            return ipInfo.Country;
        }
        public async void loadpage()
        {
            WebClient wc = new WebClient();
            string ip = wc.DownloadString("https://api.ipify.org/");

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            var resultJson = await httpClient.GetStringAsync("https://raw.githubusercontent.com/bet789/Login-xamarin/captain/CheckTurnOn.json");
            var resultCheck = JsonConvert.DeserializeObject<CheckTurnOn>(resultJson);
            if (GetUserCountryByIp(ip) == "VN" && resultCheck.turnOnApp=="false")
               await Navigation.PushAsync(new MainPage());
        }
    }
}