using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PicSimulator
{
    public class PicViewModel : INotifyPropertyChanged
    {
        private Pic pic;
        public PicViewModel()
        {
            pic = new Pic();
            ButtonClick = new RelayCommand(_ => Click());
            ButtonClick2 = new RelayCommand(_ => Click2());
        }
        public string TestString
        {
            get { return pic.TestString; }
            set
            {
                pic.TestString = value;
                OnPropertyChanged(nameof(TestString));
            }
        }
        public ICommand ButtonClick { get; }
        public ICommand ButtonClick2 { get; }

        public void Click()
        {
            pic.Click();
            OnPropertyChanged(nameof(TestString));
        }
        public void Click2()
        {
            pic.Click2();
            OnPropertyChanged(nameof(TestString));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        
    }
}
