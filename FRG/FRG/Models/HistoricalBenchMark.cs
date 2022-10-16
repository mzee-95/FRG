using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FRG.Models
{
  public class HistoricalBenchMark : ViewModelBase
  {
    private string name;

    public string Name
    {
      get { return name; }
      set
      {
        name = value;
        NotifyPropertyChanged(nameof(Name));
      }
    }
    private string oneMonth;

    public string OneMonth
    {
      get { return oneMonth; }
      set
      {
        oneMonth = value;
        NotifyPropertyChanged(nameof(OneMonth));
      }
    }
    private string threeMonth;

    public string ThreeMonth
    {
      get { return threeMonth; }
      set
      {
        threeMonth = value;
        NotifyPropertyChanged(nameof(ThreeMonth));
      }
    }
    private string sixMonth;

    public string SixMonth
    {
      get { return sixMonth; }
      set
      {
        sixMonth = value;
        NotifyPropertyChanged(nameof(SixMonth));
      }
    }
    private string oneYear;

    public string OneYear
    {
      get { return oneYear; }
      set
      {
        oneYear = value;
        NotifyPropertyChanged(nameof(OneYear));
      }
    }
    private string twoYear;

    public string TwoYear
    {
      get { return twoYear; }
      set
      {
        twoYear = value;
        NotifyPropertyChanged(nameof(TwoYear));
      }
    }

    private string threeYear;

    public string ThreeYear
    {
      get { return threeYear; }
      set
      {
        threeYear = value;
        NotifyPropertyChanged(nameof(ThreeYear));
      }
    }

    private string fiveYear;

    public string FiveYear
    {
      get { return fiveYear; }
      set
      {
        fiveYear = value;
        NotifyPropertyChanged(nameof(FiveYear));
      }
    }

    private string sevenYear;

    public string SevenYear
    {
      get { return sevenYear; }
      set
      {
        sevenYear = value;
        NotifyPropertyChanged(nameof(SevenYear));
      }
    }
    private int[] periods = new int[] { 1, 3, 6, 12, 24, 36, 60, 84, 120 };

    private string tenYear;

    public string TenYear
    {
      get { return tenYear; }
      set
      {
        tenYear = value;
        NotifyPropertyChanged(nameof(TenYear));
      }
    }

    public void CalculatePerformances(List<BenchmarkHistoryPerformance> histBenchmarks, List<Benchmark> benchmarks, DateTime SelectedDate, DateTime SwitchOverDate)
    {
      foreach (var p in periods)
      {
        SetViewData(p, CalculateCombinedData(histBenchmarks, benchmarks, SelectedDate,SwitchOverDate, p));
      }
    }

    public void CalculateHistoricPerformances(List<BenchmarkHistoryPerformance> benchmarks, DateTime selectedDate, int period =-1)
    {
      if(period == -1)
      {
        foreach(var p in periods)
        {
          SetViewData(p,GetHistoricPeriods(benchmarks, selectedDate, p));
        }
      }
      else
        GetHistoricPeriods(benchmarks, selectedDate, period);
    }
    private float CalculateCombinedData(List<BenchmarkHistoryPerformance> histBenchmarks, List<Benchmark> benchmarks, DateTime SelectedDate, DateTime SwitchOverDate, int period)
    {
      var dateDiff = GetMonthDifference(SelectedDate, SwitchOverDate);
      float res = 0;

      //Base Case
      if (SwitchOverDate>=SelectedDate)
        res = GetHistoricPeriods(histBenchmarks, SelectedDate, period);
      else if(dateDiff >=period)
      {
        res = CalculatePeriod(benchmarks, SelectedDate, period);
      }
      else
      {
        var hist = GetHistoricPeriods(histBenchmarks, SwitchOverDate, period - dateDiff);
        var curr = CalculatePeriod(benchmarks, SelectedDate, dateDiff);
        res = hist + curr;
      }

      return res;
    }

    public void CalculateJointReport(ObservableCollection<InstrumentDictionary> instrumentDic, Dictionary<string, List<BenchmarkHistoryPerformance>> map, DateTime selectedDate)
    {
      float res =0;
      int remainingPeriod = 0;
      foreach (var period in periods)
      {
        remainingPeriod = period;
        //loop through instrumentDic
        foreach (InstrumentDictionary instrument in instrumentDic)
        {
          var dateDiff = GetMonthDifference(selectedDate, instrument.ReportDate);
          if (instrument.ReportDate >= selectedDate || dateDiff >= remainingPeriod || instrument == instrumentDic.Last())
          {
            res += GetHistoricPeriods(map[instrument.InstrumentName], selectedDate, remainingPeriod);
            break;
          }
          else
          {
            res += GetHistoricPeriods(map[instrument.InstrumentName], selectedDate, dateDiff);
            selectedDate = instrument.ReportDate;
            remainingPeriod -= dateDiff;
          }
        }
        SetViewData(period, res);
        res = 0;
      }
      
    }

    private float GetHistoricPeriods(List<BenchmarkHistoryPerformance> benchmarks, DateTime selectedDate, int period)
    {
      float res = 0;
      int idx = benchmarks.FindIndex(x => x.PriceDateEnd.Year == selectedDate.Year && x.PriceDateEnd.Month == selectedDate.Month);
      for (int i = idx; i > (idx - period); i--)
      {
        var o = benchmarks[i].Performance;
        res += o;
      }
      return res;
    }

    private float CalculatePeriod(List<Benchmark> benchmarks, DateTime SelectedDate, int period)
    {
      List<Benchmark> temp = new List<Benchmark>();
      if (period >= 12)
      {
        temp = benchmarks.Where(x => (x.PriceDate.Year == SelectedDate.Year
         || x.PriceDate.Year == SelectedDate.AddYears(-(period / 12)).Year))
           .ToList();
      }
      else
      {
        temp = benchmarks.Where(x => x.PriceDate.Year == SelectedDate.Year
         && (x.PriceDate.Month == SelectedDate.Month
         || x.PriceDate.Month == SelectedDate.AddMonths(-period).Month))
           .ToList();
      }

      var startItem = temp.First(a => a.PriceDate == SelectedDate);
      int idx = temp.FindIndex(a => a.PriceDate == startItem.PriceDate);
      var nextItem = temp[--idx];
      return (CalcValue(startItem, nextItem, temp, period) - 1) * 100;

    }

    private void SetViewData(int period, float res)
    {
      switch (period)
      {
        case 1:
          OneMonth = res.ToString();
          break;
        case 3:
          ThreeMonth = res.ToString();
          break;
        case 6:
          SixMonth = res.ToString();
          break;
        case 12:
          OneYear = res.ToString();
          break;
        case 24:
          TwoYear = res.ToString();
          break;
        case 36:
          ThreeYear = res.ToString();
          break;
        case 60:
          FiveYear = res.ToString();
          break;
        case 84:
          SevenYear = res.ToString();
          break;
        case 120:
          TenYear = res.ToString();
          break;
        default:
          MessageBox.Show("An Unexpected Period has been detected", "Unexpected Period");
          break;
      }
    }

    private float CalcValue(Benchmark item1, Benchmark item2, List<Benchmark> items, int period)
    {
      bool condition = false;
      float tmp = GetSubCalc(GetPerformanceFigure(item1.Price, item2.Price));
      if (period >= 12)
        condition = item2.PriceDate.Year == item1.PriceDate.AddYears(-(period / 12)).Year;
      else
        condition = item2.PriceDate.Month == item1.PriceDate.AddMonths(-period).Month;

      if (condition)
        return tmp;

      int idx = items.FindIndex(a => a.PriceDate == item2.PriceDate);
      return tmp * CalcValue(item2, items[--idx], items, period);
    }

    private float GetSubCalc(float price)
    {
      return (price / 100) + 1;
    }

    private float GetPerformanceFigure(float currentPrice, float prevPrice)
    {
      return ((currentPrice / prevPrice) - 1) * 100; ;
    }

    public int GetMonthDifference(DateTime startDate, DateTime endDate)
    {
      int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
      return Math.Abs(monthsApart);
    }
  }
}
