using Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSimulator
{
    public interface IPicViewModel : IPic
    {
        ObservableCollection<CodeLine> Code { get; set; }
    }
}
