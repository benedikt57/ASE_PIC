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
            //Commands
            //ButtonClick = new RelayCommand(_ => Click());
        }

        //Variablen
        public List<string> Code
        {
            get { return pic.Code; }
            set
            {
                pic.Code = value;
                OnPropertyChanged(nameof(Code));
            }
        }
        //Commands
        //public ICommand ButtonClick { get; }
        //public void Click()
        //{
        //    pic.Click();
        //    OnPropertyChanged(nameof(TestString));
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        
    }
}
