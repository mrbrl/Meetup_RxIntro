using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using Xamarin.Forms;

using ColorPicker.Views;

namespace ColorPicker.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        // TODO 1 - to auto properties
        private int _redValue;
        private int _blueValue;
        private int _greenValue;

        public int RedValue {
            set
            {
                SetProperty(ref _redValue, value);
                BackColor = Color.FromRgb(value, GreenValue, BlueValue);
                OnPropertyChanged(nameof(BackColor));
            }
            get => _redValue;
        }

        
        public int GreenValue {
            set
            {
                SetProperty(ref _greenValue, value);
                BackColor = Color.FromRgb(RedValue, value, BlueValue);
                OnPropertyChanged(nameof(BackColor));
            }
            get => _greenValue;
        }
        
        public int BlueValue {
            set
            {
                SetProperty(ref _blueValue, value);
                BackColor = Color.FromRgb(RedValue, GreenValue, value);
                OnPropertyChanged(nameof(BackColor));
            }
            get => _blueValue;
        }


        public Color BackColor { get; set; }

        public HomeViewModel()
        {
            // TODO 2 Rx FromEventPattern 
        }
    }
}