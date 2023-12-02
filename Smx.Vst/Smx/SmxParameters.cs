using Jacobi.Vst.Core;
using Jacobi.Vst.Plugin.Framework;
using Smx.Vst.Data;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Xml.Linq;
using VstNetAudioPlugin;

namespace Smx.Vst.Smx
{
  /// <summary>
  /// Encapsulated delay parameters.
  /// </summary>
  internal sealed class SmxParameters
  {
    public const string SawParameterName = "Saw";
    public const string AttackParameterName = "Attack";
    public const string ReleaseParameterName = "Release";
    public const string FmModParameterName = "FmMod";
    public const string PowParameterName = "Pow";

    private const string ParameterCategoryName = "Smx";

    /// <summary>
    /// Initializes the paramaters for the Delay component.
    /// </summary>
    /// <param name="parameters"></param>
    public SmxParameters(PluginParameters parameters)
    {
      Throw.IfArgumentIsNull(parameters, nameof(parameters));

      InitializeParameters(parameters);
    }

    public VstParameterManager FmModMgr { get; private set; }

    public VstParameterManager AttackMgr;
    public VstParameterManager ReleaseMgr;
    public VstParameterManager PowMgr;

    public VstParameterManager SawMgr { get; private set; }
    public VstParameterManager[] GenMgrs { get; private set; }

    // This method initializes the plugin parameters this Dsp component owns.
    private void InitializeParameters(PluginParameters parameters)
    {
      // all parameter definitions are added to a central list.
      VstParameterInfoCollection parameterInfos = parameters.ParameterInfos;

      // retrieve the category for all delay parameters.
      VstParameterCategory paramCategory =
          parameters.GetParameterCategory(ParameterCategoryName);

      VstParameterManager CreateFloat(string name, string label, string shortLabel)
      {
        var paramInfo = new VstParameterInfo
        {
          Category = paramCategory,
          CanBeAutomated = true,
          Name = name,
          Label = label,
          ShortLabel = shortLabel,
          LargeStepFloat = 0.1f,
          SmallStepFloat = 0.01f,
          StepFloat = 0.05f,
          DefaultValue = 0.0f
        };

        parameterInfos.Add(paramInfo);

        return paramInfo
        .Normalize()
        .ToManager();
      }

      VstParameterManager CreateSwitch(string name, string label, string shortLabel)
      {
        var paramInfo = new VstParameterInfo
        {
          Category = paramCategory,
          CanBeAutomated = true,
          Name = name,
          Label = label,
          ShortLabel = shortLabel,
          IsSwitch = true,
        };

        parameterInfos.Add(paramInfo);

        return paramInfo
        .Normalize()
        .ToManager();
      }

      SawMgr = CreateFloat(name: SawParameterName,
                           label: "Saw Amount (1.0 Saw - 0.0 Sin)",
                           shortLabel: "Saw Amt.");
      FmModMgr = CreateSwitch(FmModParameterName,
                              "FM Modulation",
                              "FM Mod");
      AttackMgr = CreateFloat(name: AttackParameterName,
                           label: "Attack Time",
                           shortLabel: "Attack");
      ReleaseMgr = CreateFloat(name: ReleaseParameterName,
                           label: "Release Time",
                           shortLabel: "Release");
      PowMgr = CreateFloat(name: PowParameterName,
                           label: "Power to raise by",
                           shortLabel: "Power");

      GenMgrs = new VstParameterManager[GeneratorList.List.Count];
      foreach (var gen in GeneratorList.List) {

        GenMgrs[gen.Index] = CreateSwitch(name: gen.ParameterName,
                             label: gen.ParameterLabel,
                             shortLabel: gen.DisplayName);
      }
    }
  }
}
