using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Xamarin.Forms;

namespace ColorPicker.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        //private int _redValue;
        //private int _blueValue;
        //private int _greenValue;

        public int RedValue { get; set; }
        public int GreenValue { get; set; }
        public int BlueValue { get; set; }

        //public int RedValue {
        //    set
        //    {
        //        SetProperty(ref _redValue, value);
        //        BackColor = Color.FromRgb(value, GreenValue, BlueValue);
        //        OnPropertyChanged(nameof(BackColor));
        //    }
        //    get => _redValue;
        //}

        //public int GreenValue {
        //    set
        //    {
        //        SetProperty(ref _greenValue, value);
        //        BackColor = Color.FromRgb(RedValue, value, BlueValue);
        //        OnPropertyChanged(nameof(BackColor));
        //    }
        //    get => _greenValue;
        //}
        
        //public int BlueValue {
        //    set
        //    {
        //        SetProperty(ref _blueValue, value);
        //        BackColor = Color.FromRgb(RedValue, GreenValue, value);
        //        OnPropertyChanged(nameof(BackColor));
        //    }
        //    get => _blueValue;
        //}


        public Color BackColor { get; set; }

        public HomeViewModel()
        {
            Observable.FromEventPattern<PropertyChangedEventArgs>(this, nameof(PropertyChanged))
                .Where(x => x.EventArgs.PropertyName == nameof(RedValue)
                            || x.EventArgs.PropertyName == nameof(GreenValue)
                            || x.EventArgs.PropertyName == nameof(BlueValue))
                .Subscribe(_ =>  BackColor = Color.FromRgb(RedValue, GreenValue, BlueValue));
        }
    }
}