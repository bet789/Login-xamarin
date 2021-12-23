using Login.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class frm_SignUp : ContentPage
    {
        public frm_SignUp()
        {
            InitializeComponent();
            loaddt();
        }
        public string ccid { get; set; }
        public async void loaddt()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            var resultJson = await httpClient.GetStringAsync("https://api.jun82.com/jun88-ecp/api/v1/captchas/random");
            var resultLogin = JsonConvert.DeserializeObject<Captcha>(resultJson);
            ccid = resultLogin.uuid;
            byte[] Base64Stream = Convert.FromBase64String(resultLogin.image.Substring(23));
            imagecc.Source = ImageSource.FromStream(() => new MemoryStream(Base64Stream));
        }
        public static string Encode(string input, string key)
        {
            byte[] keyvalue = Encoding.ASCII.GetBytes(key);
            HMACSHA1 myhmacsha1 = new HMACSHA1(keyvalue);
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);
            return myhmacsha1.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);
        }
        private async void btnSignUp_Clicked(object sender, EventArgs e)
        {
            try 
            {
                if(txtNumberPhone.Text.Length!=9)
                {
                    await DisplayAlert("Thông Báo", "Số Điện Thoại Không Hợp Lệ!", "OK");
                }
                else
                {
                    var values = new[]
                    {
                        new KeyValuePair<string, string>("ulagentaccount", "bongda88"),
                        new KeyValuePair<string, string>("playerid", txtUserName.Text),
                        new KeyValuePair<string, string>("password", Encode(txtPassword.Text,txtUserName.Text)),
                        new KeyValuePair<string, string>("captcha", txtCaptcha.Text),
                        new KeyValuePair<string, string>("captchauuid", ccid),
                        new KeyValuePair<string, string>("currency", "VND2"),
                        new KeyValuePair<string, string>("firstname", txtFullName.Text),
                        new KeyValuePair<string, string>("mobile", "84 "+txtNumberPhone.Text),
                        new KeyValuePair<string, string>("portalid", "EC_MOBILE"),
                        new KeyValuePair<string, string>("pin", Encode(txtPassword.Text, txtUserName.Text)),
                        new KeyValuePair<string, string>("language", "4"),
                    };
                    var httpClient = new HttpClient();
                    var json = JsonConvert.SerializeObject(values);
                    var content = new MultipartFormDataContent();
                    foreach (var keyValuePair in values)
                        content.Add(new StringContent(keyValuePair.Value), string.Format("\"{0}\"", keyValuePair.Key));
                    using (var result = await httpClient.PostAsync("https://api.jun82.com/jun88-ecp/api/v1/register", content))
                    {

                        if (result.IsSuccessStatusCode)
                        {
                            await DisplayAlert("Thông Báo", "Đăng Ký Thành Công", "OK");
                        }
                        else
                            await DisplayAlert("Thông Báo", "Tài Khoản Đã Tồn Tại", "OK");
                    }
                }        
            }
            catch
            {
                await DisplayAlert("Thông Báo", "Đăng Ký Thất Bại, Vui Lòng Kiểm Tra Lại!", "OK");
            }
            loaddt();            
        }
    }
}
// https://github.com/bet789/Login-xamarin/blob/captain/CheckTurnOn.json