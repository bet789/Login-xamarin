using Login.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
    {
        public Home(string para)
        {
            InitializeComponent();
            //webmain.Source = "https://www.jun82.com/?token=" + para;
            webmain.Source = "https://tinhte.vn/";
        }
    }
}