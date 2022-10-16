using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRG.Models
{
  public class BenchmarkHistoryPerformance
  {
    public string Name { get; set; }
    public DateTime PriceDateStart { get; set; }
    public DateTime PriceDateEnd { get; set; }
    public float Performance { get; set; }
  }
}
