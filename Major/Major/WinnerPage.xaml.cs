using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Major
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WinnerPage : ContentPage
    {
        public WinnerPage(TimeSpan timeTaken, string msg)
        {

            InitializeComponent();
            timeElapsed.Text = String.Format("Time taken: {0:D2}:{1:D2}", timeTaken.Minutes, timeTaken.Seconds);
            winnerMessage.Text = msg;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PopToRootAsync();
        }
    }
}