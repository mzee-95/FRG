using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRG.Models
{
  public class InstrumentDictionary : ViewModelBase
  {
    private string instrumentName;

    public string InstrumentName
    {
      get { return instrumentName; }
      set
      {
        instrumentName = value;
        NotifyPropertyChanged(InstrumentName);
      }
    }

    private DateTime reportDate;

    public DateTime ReportDate
    {
      get { return reportDate; }
      set
      {
        reportDate = value;
        NotifyPropertyChanged(nameof(ReportDate));
      }
    }

  }
}
