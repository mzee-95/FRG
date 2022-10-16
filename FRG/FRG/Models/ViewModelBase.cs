using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FRG.Models
{
  [Serializable]
  public abstract class ViewModelBase : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(object sender, string propertyName)
    {
      if (propertyName != null)
      {
        this.PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
      }
    }

    public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
    {
      NotifyPropertyChanged(this, propertyName);
    }
  }
}
