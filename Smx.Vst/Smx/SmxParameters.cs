using Jacobi.Vst.Core;
using Jacobi.Vst.Plugin.Framework;
using Smx.Vst.Data;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Xml.Linq;
using VstNetAudioPlugin;
using static Smx.Vst.Smx.Filter;

namespace Smx.Vst.Smx
{
  /// <summary>
  /// Encapsulated delay parameters.
  /// </summary>
  internal sealed class SmxParameters
  {
    public const string AttackParameterName = "Attack";
    public const string FmModParameterName = "FmMod";
    public const string PowParameterName = "Pow";
    public const string ReleaseParameterName = "Release";
    public const string SawParameterName = "Saw";
    public VstParameterManager AttackMgr;
    public VstParameterManager PowMgr;
    public VstParameterManager ReleaseMgr;
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

    public VstParameterManager FilterCountMgr { get; private set; }
    public FilterParameter[] FilterParameterAry { get; private set; }
    public VstParameterManager FmModMgr { get; private set; }
    public VstParameterManager[] GenMgrs { get; private set; }
    public VstParameterManager IniDetMgr { get; private set; }
    public VstParameterManager InTuAcMgr { get; private set; }
    public VstParameterManager InTuFrMgr { get; private set; }
    public VstParameterManager MembraneMixMgr { get; private set; }
    public VstParameterManager SawMgr { get; private set; }
    public VstParameterManager TuneMgr { get; private set; }
    public VstParameterManager UniDetMgr { get; private set; }
    public VstParameterManager UniPanMgr { get; private set; }
    public VstParameterManager VoiceCountMgr { get; private set; }
    public VstParameterManager VoiceDetuneMgr { get; private set; }
    public VstParameterManager VoiceSpreadMgr { get; private set; }

    // This method initializes the plugin parameters this Dsp component owns.
    private void InitializeParameters(PluginParameters parameters)
    {
      // all parameter definitions are added to a central list.
      VstParameterInfoCollection parameterInfos = parameters.ParameterInfos;

      // retrieve the category for all delay parameters.
      VstParameterCategory paramCategory =
          parameters.GetParameterCategory(ParameterCategoryName);

      VstParameterManager CreateFloat(string name, string label, string shortLabel, float defaultValue = 0)
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
          DefaultValue = defaultValue,
        };

        parameterInfos.Add(paramInfo);

        return paramInfo
        .Normalize()
        .ToManager();
      }

      VstParameterManager CreateSwitch(string name, string label, string shortLabel, bool defaultValue = false)
      {
        var paramInfo = new VstParameterInfo
        {
          Category = paramCategory,
          CanBeAutomated = true,
          Name = name,
          Label = label,
          ShortLabel = shortLabel,
          IsSwitch = true,
          DefaultValue = defaultValue ? 1 : 0
        };

        parameterInfos.Add(paramInfo);

        return paramInfo
        .Normalize()
        .ToManager();
      }

      VstParameterManager CreateInteger(string name, string label, string shortLabel, int max, int defaultValue = 0, int min = 0)
      {
        var paramInfo = new VstParameterInfo
        {
          Category = paramCategory,
          CanBeAutomated = true,
          Name = name,
          Label = label,
          ShortLabel = shortLabel,
          MinInteger = min,
          MaxInteger = max,
          StepInteger = 1,
          LargeStepInteger = 3,
          DefaultValue = defaultValue,
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
                           shortLabel: "Power",
                           defaultValue: 1.0f);

      IniDetMgr = CreateFloat(name: "IniDet",
                  label: "Initial Detune",
                  shortLabel: "Ini.Det.",
                  defaultValue: 0.5f);
      InTuAcMgr = CreateFloat(name: "InTuAc",
                  label: "Initial Detune Acceleration",
                  shortLabel: "I.De.Ac.");
      InTuFrMgr = CreateFloat(name: "InTuFr",
                  label: "Initial Detune Friction",
                  shortLabel: "I.De.Fr.");

      VoiceCountMgr = CreateInteger(name: "VoiCount",
            label: "Voice Count",
            shortLabel: "Voi.Cou.",
            min: 1,
            max: 16,
            defaultValue: 1);
      VoiceDetuneMgr = CreateFloat(name: "VoiDet",
            label: "Voice Detune",
            shortLabel: "Voi.Det.");
      VoiceSpreadMgr = CreateFloat(name: "VoiSprd",
            label: "Voice Spread",
            shortLabel: "Voi.Spr.");

      UniDetMgr = CreateFloat(name: "UniDet",
                           label: "Unison Detune",
                           shortLabel: "Uni.Det.");
      UniPanMgr = CreateFloat(name: "UniPan",
                           label: "Unison Pan",
                           shortLabel: "Uni.Pan.");

      TuneMgr = CreateFloat(name: "Tune",
                     label: "Tune",
                     shortLabel: "Tune",
                     defaultValue: 0.5f);

      FilterCountMgr = CreateInteger(name: "FltrCt",
                  label: "Filter Count",
                  shortLabel: "Fil.Cnt.",
                  min: 0,
                  max: 4,
                  defaultValue: (int)Filter.Mode.None);

      int maxFilterCount = 4;
      FilterParameterAry = new FilterParameter[maxFilterCount];
      for (int i = 0; i < 4; i++)
      {
        var p = new Filter.FilterParameter(modeMgrs: CreateInteger(name: "FltrMd" + i,
                                                                   label: "Filter Mode " + i,
                                                                   shortLabel: "Fil.Md." + i,
                                                                   min: (int)Filter.Mode.None,
                                                                   max: (int)Filter.Mode.BandpassAdd,
                                                                   defaultValue: (int)Filter.Mode.None),
                                           cutoffMgr: CreateFloat(name: "FltrCut" + i,
                                                                  label: "Filter Cutoff " + i,
                                                                  shortLabel: "Fil.Ct." + i,
                                                                  defaultValue: 0.5f),
                                           dryWetMgr: CreateFloat(name: "FltrDrW" + i,
                                                                  label: "Filter Dry/Wet " + i,
                                                                  shortLabel: "Fl.D.W." + i,
                                                                  defaultValue: 1.0f),
                                           resonanceMgr: CreateFloat(name: "FltrRes" + i,
                                                                  label: "Filter Resonance " + i,
                                                                  shortLabel: "Fl.F.b." + i));
        FilterParameterAry[i] = p;
      }

      GenMgrs = new VstParameterManager[GeneratorList.List.Count];
      foreach (var gen in GeneratorList.List)
      {
        GenMgrs[gen.Index] = CreateSwitch(name: gen.ParameterName,
                             label: gen.ParameterLabel,
                             shortLabel: gen.DisplayName,
                             defaultValue: gen.Factor == 1.0f);
      }
    }
  }
}