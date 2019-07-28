using System;
using System.ComponentModel;
using ColorPicker.ViewModels;
using Xamarin.Forms;

namespace ColorPicker.Views
{
    [DesignTimeVisible(false)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            BindingContext = new HomeViewModel();
        }
    }
}