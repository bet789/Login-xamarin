using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using Login.Model;
using System.Net.Http;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace Login
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            loaddt();
            loadjs();
        }
        public string ccid { get; set; }
        public string resultcheck { get; set; }
        public async void loaddt()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            var resultJson = await httpClient.GetStringAsync("https://api.jun82.com/jun88-ecp/api/v1/captchas/random");
            var resultLogin = JsonConvert.DeserializeObject<Captcha>(resultJson);
            ccid = resultLogin.uuid;
            byte[] Base64Stream = Convert.FromBase64String(resultLogin.image.Substring(23));
            imagedd.Source = ImageSource.FromStream(() => new MemoryStream(Base64Stream));
        }
        private async void showFormSignUp(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new frm_SignUp());
        }
       
        public static string Encode(string input, string key)
        {
            byte[] keyvalue = Encoding.ASCII.GetBytes(key);
            HMACSHA1 myhmacsha1 = new HMACSHA1(keyvalue);
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);
            return myhmacsha1.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);
        }
        public async void loadjs()
        {
            //WebClient wc = new WebClient();
            //return wc.DownloadString("https://raw.githubusercontent.com/bet789/Login-xamarin/captain/CheckTurnOn.json");

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            var resultJson = await httpClient.GetStringAsync("https://raw.githubusercontent.com/bet789/Login-xamarin/captain/CheckTurnOn.json");
            var resultCheck = JsonConvert.DeserializeObject<CheckTurnOn>(resultJson);
            resultcheck = resultCheck.turnOnApp;
        }

        private async void btnLogin_Clicked(object sender, EventArgs e)
        {
            //await DisplayAlert("Thông Báo", resultcheck, "OK");
            try
            {
                if (txtCaptcha.Text.Length != 4)
                {
                    await DisplayAlert("Thông Báo", "Mã Định Danh Không Hợp Lệ", "OK");
                }
                else
                {
                    var log = new
                    {
                        loginname = txtUserName.Text,
                        loginpassword = Encode(txtPassword.Text, txtUserName.Text),
                        captcha = txtCaptcha.Text,
                        captchauuid = ccid,
                        fingerprint = "",
                        portalid = "EC_MOBILE"
                    };
                    var jsonString = JsonConvert.SerializeObject(log);
                    var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                    var myHttpClient = new HttpClient();
                    HttpResponseMessage response = await myHttpClient.PostAsync("https://api.jun82.com/jun88-ecp/api/v1/login", content);
                    var value = await response.Content.ReadAsStringAsync();
                    ContentResponse obj = JsonConvert.DeserializeObject<ContentResponse>(value);
                    if (response.IsSuccessStatusCode)
                    {
                        if(resultcheck=="true")
                            await Navigation.PushAsync(new Home("https://www.jun82.com/?token="+obj.token));
                        else
                            await Navigation.PushAsync(new Home("https://tinhte.vn/"));
                    }
                    else
                    {
                        await DisplayAlert("Thông Báo", "Sai Email Hoặc PassWord", "OK");
                    }
                }
            }
            catch
            {
                await DisplayAlert("Thông Báo", "Thông Tin Không Được Bỏ Trống", "OK");
            }
            loaddt();
        }
    }
}
