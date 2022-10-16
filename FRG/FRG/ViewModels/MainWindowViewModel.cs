using FRG.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FRG.ViewModels
{
  public class MainWindowViewModel : ViewModelBase
  {
    #region Private members
    private string as_At;
    private bool instrumentAdded;
    private DateTime selectedDate;
    private DateTime instrumentDate;
    private string selectedBenchmark;
    private DateTime historicalSwitch;
    private ObservableCollection<string> benchmarkList;
    private ObservableCollection<InstrumentDictionary> instrumentDic;
    private ObservableCollection<Benchmark> benchmarkCollection;
    private ObservableCollection<BenchmarkHistoryPerformance> _benchmarkHistoryPerformance;
    #endregion

    #region Public members
    public DateTime InstrumentDate
    {
      get { return instrumentDate; }
      set
      {
        instrumentDate = value;
        NotifyPropertyChanged(nameof(InstrumentDate));
      }
    }
    public bool InstrumentAdded
    {
      get { return instrumentAdded; }
      set
      {
        instrumentAdded = value;
        NotifyPropertyChanged(nameof(InstrumentAdded));
      }
    }
    public ObservableCollection<InstrumentDictionary> InstrumentDic
    {
      get { return instrumentDic; }
      set
      {
        instrumentDic = value;
        NotifyPropertyChanged(nameof(InstrumentDic));
      }
    }

    public DateTime HistoricalSwitch
    {
      get { return historicalSwitch; }
      set
      {
        historicalSwitch = value;
        As_At = value.ToString("dd-MMM-YYYY");
        NotifyPropertyChanged(nameof(HistoricalSwitch));
      }
    }

    public ObservableCollection<string> BenchmarkList
    {
      get { return benchmarkList; }
      set { benchmarkList = value; }
    }


    public ObservableCollection<BenchmarkHistoryPerformance> _BenchmarkHistoryPerformance
    {
      get { return _benchmarkHistoryPerformance; }
      set { _benchmarkHistoryPerformance = value; }
    }

    public string SelectedBenchmark
    {
      get { return selectedBenchmark; }
      set
      {
        selectedBenchmark = value;
        NotifyPropertyChanged(nameof(SelectedBenchmark));
      }
    }

    public string As_At
    {
      get { return as_At; }
      set
      {
        as_At = value;
        NotifyPropertyChanged(nameof(As_At));
      }
    }


    public DateTime SelectedDate
    {
      get { return selectedDate; }
      set
      {
        selectedDate = value;
        As_At = value.ToString("dd-MMM-YYYY");
        NotifyPropertyChanged(nameof(SelectedDate));
      }
    }
    #endregion

    #region Commands

    public ICommand ExtractCommand
    {
      get
      {
        return new DelegateCommand(Extract);
      }
    }



    public ICommand GenerateReportCommand
    {
      get
      {
        return new DelegateCommand(Search);
      }
    }
    public ICommand AddInstrumentCommand
    {
      get
      {
        return new DelegateCommand(AddInstrument);
      }
    }


    public ICommand GenerateReportJointCommand
    {
      get
      {
        return new DelegateCommand(GenerateJointReport);
      }
    }

    private void GenerateJointReport(object obj)
    {
      try
      {
        Dictionary<string, List<BenchmarkHistoryPerformance>> map = new Dictionary<string, List<BenchmarkHistoryPerformance>>();
        InstrumentDic.OrderByDescending(x => x.ReportDate);
        foreach (var inst in InstrumentDic)
        {
          map.Add(inst.InstrumentName, _BenchmarkHistoryPerformance.Where(x => x.Name == inst.InstrumentName).ToList());
        }

        _BenchmarkResult.CalculateJointReport(InstrumentDic, map, SelectedDate);
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message, "Error Generating Joint Report");
      }

    }

    public ICommand ClearCommand
    {
      get
      {
        return new DelegateCommand(Clear);
      }
    }

    

    public ICommand SearchCommand
    {
      get
      {
        return new DelegateCommand(GenerateReport);
      }
    }
    #endregion

    #region Functions

    private void Clear(object obj)
    {
      InstrumentDic.Clear();
      _BenchmarkResult = new HistoricalBenchMark();
    }

    private bool VerifySearchFields()
    {
      Type myType = _BenchmarkResult.GetType();
      if (myType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).Any(property =>
         property.GetValue(_BenchmarkResult) != null && property.Name != "periods" && !string.IsNullOrEmpty(property.Name)))
      {
        return true;
      }

      return false;
    }

    private void Extract(object obj)
    {

      try
      {
        if (!VerifySearchFields())
        {
          throw new Exception();
        }
        string path = Environment.CurrentDirectory + @"\results.csv";
        if (!File.Exists(path))
        {
          string header = "1_month,3_months,6_months,1_year,2_years,5_years,7_years,10_years\n";
          string Data = $"{_BenchmarkResult.OneMonth},{_BenchmarkResult.ThreeMonth},{_BenchmarkResult.SixMonth},{_BenchmarkResult.OneYear}," +
                        $"{_BenchmarkResult.TwoYear},{_BenchmarkResult.FiveYear},{_BenchmarkResult.SevenYear},{_BenchmarkResult.TenYear}\n";
          File.AppendAllText(path, header);
          File.AppendAllText(path, Data);

        }
        else
        {
          string Data = $"{_BenchmarkResult.OneMonth},{_BenchmarkResult.ThreeMonth},{_BenchmarkResult.SixMonth},{_BenchmarkResult.OneYear}," +
                        $"{_BenchmarkResult.TwoYear},{_BenchmarkResult.FiveYear},{_BenchmarkResult.SevenYear},{_BenchmarkResult.TenYear}\n";
          File.AppendAllText(path, Data);
        }
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message, "Error Extracting Data");
      }
    }

    private void AddInstrument(object obj)
    {
      if (SelectedBenchmark == null)
      {
        MessageBox.Show("Select a benchmark to calculate", "Benchmark not selected");
        return;
      }
      InstrumentDic.Add(new InstrumentDictionary()
      {
        InstrumentName = SelectedBenchmark,
        ReportDate = InstrumentDate
      });
      InstrumentAdded = true;
    }

    private void GenerateReport(object obj)
    {
      try
      {
        var benchmarks = _BenchmarkHistoryPerformance.Where(x => x.Name == SelectedBenchmark).ToList();
        GenerateHistoricReport(benchmarks);
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message, "Error Generating Historic Report");
      }
    }

    private void GenerateHistoricReport(List<BenchmarkHistoryPerformance> benchmarks)
    {
      _BenchmarkResult.CalculateHistoricPerformances(benchmarks, SelectedDate);
    }

    private void Search(object obj)
    {
      if (SelectedBenchmark != null)
        try
        {
          CalculatePerformance();
        }
        catch (Exception e)
        {
          MessageBox.Show(e.Message, "Error Calculateing Performance");
        }
    }

    private HistoricalBenchMark _benchmarkResult;

    public HistoricalBenchMark _BenchmarkResult
    {
      get { return _benchmarkResult; }
      set
      {
        _benchmarkResult = value;
        NotifyPropertyChanged(nameof(_BenchmarkResult));
      }
    }


    public ObservableCollection<Benchmark> BenchmarkCollection
    {
      get { return benchmarkCollection; }
      set
      {
        benchmarkCollection = value;
        NotifyPropertyChanged(nameof(BenchmarkCollection));
      }
    }

    public MainWindowViewModel()
    {
      InstrumentDic = new ObservableCollection<InstrumentDictionary>();
      SelectedDate = DateTime.Now.AddYears(-4);
      HistoricalSwitch = DateTime.Now.AddYears(-4);
      InstrumentDate = DateTime.Now.AddYears(-4);
      BenchmarkList = new ObservableCollection<string>()
      {
        "JB01","JB03","JB06","JB06","JB12","LIBOR3","LIBOR6"
      };
      BenchmarkCollection = new ObservableCollection<Benchmark>();
      _BenchmarkHistoryPerformance = new ObservableCollection<BenchmarkHistoryPerformance>();
      _BenchmarkResult = new HistoricalBenchMark();
      try
      {
        Init();
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message, "Error initializing");
      }
    }

    private void CalculatePerformance()
    {
      var benchmarks = BenchmarkCollection.Where(x => x.Name == SelectedBenchmark).ToList();
      var histBenchmarks = _BenchmarkHistoryPerformance.Where(x => x.Name == SelectedBenchmark).ToList();
      _BenchmarkResult.CalculatePerformances(histBenchmarks, benchmarks, SelectedDate, HistoricalSwitch);
    }



    private void Init()
    {
      using var reader = new StreamReader(@"Prices2019.csv");
      int row = 0;
      while (!reader.EndOfStream)
      {
        string? line = reader.ReadLine();
        row++;
        if (row == 1)
        {
          continue;
        }
        if (line != null)
        {
          string[] values = line.Split(',');
          BenchmarkCollection.Add(new Benchmark()
          {
            Name = values[0],
            PriceDate = DateTime.ParseExact(values[1], "yyyy-MM-dd", CultureInfo.InvariantCulture),
            Price = float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat)
          });
        }
      }


      using var reader2 = new StreamReader("BenchmarkPerformanceHistory2019.csv");
      row = 0;
      while (!reader2.EndOfStream)
      {
        string? line = reader2.ReadLine();
        row++;
        if (row == 1)
        {
          continue;
        }
        if (line != null)
        {
          string[] values = line.Split(',');
          _BenchmarkHistoryPerformance.Add(new BenchmarkHistoryPerformance()
          {
            Name = values[0],
            PriceDateStart = DateTime.ParseExact(values[1], "yyyy-MM-dd", CultureInfo.InvariantCulture),
            PriceDateEnd = DateTime.ParseExact(values[2], "yyyy-MM-dd", CultureInfo.InvariantCulture),
            Performance = float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat)
          });
        }
      }
    }

    #endregion
  }
}
