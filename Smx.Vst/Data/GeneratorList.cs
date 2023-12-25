using System.Collections.Generic;
using System.Linq;

namespace Smx.Vst.Data
{
  public class GeneratorList
  {
    private static List<GeneratorItem>? proplist;
    private static Dictionary<int, GeneratorItem>? propDict;

    public static List<GeneratorItem> List
    {
      get => proplist ?? (proplist = CreateList());
      set => proplist = value;
    }

    public static Dictionary<int,GeneratorItem> Dict
    {
      get => propDict ?? (propDict = List.ToDictionary(item => item.Index));
      set => propDict = value;
    }

    private static List<GeneratorItem> CreateList()
    {
      var list = new List<GeneratorItem>();
      int index  = 0;
      for (int i = 1; i <= 24; i++)
      {
        list.Add(new GeneratorItem
        {
          Index = index++,
          DisplayName = $"{i}/12",
          ParameterName = $"Gen{i}/12",
          ParameterLabel = $"Generator {i}/12",
          Mult = i,
          Factor = i / 12.0,
        });
      }
      return list;
    }

    public class GeneratorItem : GeneratorParameter
    {
      public string DisplayName { get; set; }
      public string ParameterName { get; set; }
      //public double Factor { get; set; }
      //public int Mult { get; set; }
      //public int Index { get; set; }
      public string ParameterLabel { get; internal set; }

      public override string ToString()
      {
        return DisplayName;
      }
    }
  }
}