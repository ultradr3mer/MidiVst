using Jacobi.Vst.Core;
using Jacobi.Vst.Plugin.Framework;
using Smx.Vst.Data;
using Smx.Vst.Util;
using VstNetAudioPlugin;

//using static Smx.Vst.Smx.Filter;

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

    public SmxParameterManager AttackMgr { get; private set; }
    public SmxParameterManager FilterCountMgr { get; private set; }
    public FilterParameter[] FilterParameterAry { get; private set; }
    public SmxParameterManager FmModMgr { get; private set; }
    public SmxParameterManager[] GenMgrs { get; private set; }
    public SmxParameterManager IniDetMgr { get; private set; }
    public SmxParameterManager InTuAcMgr { get; private set; }
    public SmxParameterManager InTuFrMgr { get; private set; }
    public SmxParameterManager MembraneMixMgr { get; private set; }
    public SmxParameterManager PowMgr { get; private set; }
    public SmxParameterManager ReleaseMgr { get; private set; }
    public SmxParameterManager SawMgr { get; private set; }
    public SmxParameterManager TuneMgr { get; private set; }
    public SmxParameterManager UniDetMgr { get; private set; }
    public SmxParameterManager UniPanMgr { get; private set; }
    public SmxParameterManager VoiceCountMgr { get; private set; }
    public SmxParameterManager VoiceDetuneMgr { get; private set; }
    public SmxParameterManager VoiceSpreadMgr { get; private set; }

    // This method initializes the plugin parameters this Dsp component owns.
    private void InitializeParameters(PluginParameters parameters)
    {
      // all parameter definitions are added to a central list.
      VstParameterInfoCollection parameterInfos = parameters.ParameterInfos;

      // retrieve the category for all delay parameters.
      VstParameterCategory paramCategory =
          parameters.GetParameterCategory(ParameterCategoryName);

      SmxParameterManager CreateFloat(string name, string label, string shortLabel, float defaultValue = 0)
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

        var manager = paramInfo
        .Normalize()
        .ToManager();

        return new SmxParameterManager(manager);
      }

      SmxParameterManager CreateSwitch(string name, string label, string shortLabel, bool defaultValue = false)
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

        var manager = paramInfo
        .Normalize()
        .ToManager();

        return new SmxParameterManager(manager);
      }

      SmxParameterManager CreateInteger(string name, string label, string shortLabel, int max, int defaultValue = 0, int min = 0)
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

        var manager = paramInfo
        .Normalize()
        .ToManager();

        return new SmxParameterManager(manager);
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
                  defaultValue: (int)FilterMode.None);

      FilterParameterAry = new FilterParameter[AudioEngine.MaxFilter];
      for (int i = 0; i < 4; i++)
      {
        var p = new FilterParameter();

        var mode = CreateInteger(name: "FltrMd" + i,
                                  label: "Filter Mode " + i,
                                  shortLabel: "Fil.Md." + i,
                                  min: (int)FilterMode.LowpassMix,
                                  max: (int)FilterMode.BandpassAdd,
                                  defaultValue: (int)FilterMode.None);
        var cut = CreateFloat(name: "FltrCut" + i,
                               label: "Filter Cutoff " + i,
                               shortLabel: "Fil.Ct." + i,
                               defaultValue: 0.5f);
        var wet = CreateFloat(name: "FltrWAm" + i,
                              label: "Filter Wet Amt" + i,
                              shortLabel: "Fl.W.A." + i,
                              defaultValue: 1.0f);
        var res = CreateFloat(name: "FltrRes" + i,
                              label: "Filter Resonance " + i,
                              shortLabel: "Fl.F.b." + i);

        mode.ValueChanged += (o, a) => p.Mode = (int)a.NewValue;
        cut.ValueChanged += (o, a) => p.Cutoff = a.NewValue;
        wet.ValueChanged += (o, a) => p.WetAmt = a.NewValue;
        res.ValueChanged += (o, a) => p.Resonance = a.NewValue;

        FilterParameterAry[i] = p;
      }

      GenMgrs = new SmxParameterManager[GeneratorList.List.Count];
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