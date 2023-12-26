using Jacobi.Vst.Plugin.Framework;
using Smx.Vst.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace Smx.Vst.Parameter
{
  internal class ParameterContainer : List<SmxParameterManager>
  {
    private VstParameterCategory paramCategory;

    public ParameterContainer(VstParameterCategory paramCategory)
    {
      this.paramCategory = paramCategory;
    }

    protected SmxParameterManager CreateFloatMod(string name, string label, string shortLabel, ModPara modPara, float defaultValue = 0, float min = 0, float max = 1)
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

      Action<float>? updateAction = v => modPara.SetBase(v);
      var smxManager = new SmxParameterManager(paramInfo, updateAction, min, max);
      smxManager.ModPara = modPara;
      Add(smxManager);
      return smxManager;
    }

    protected SmxParameterManager CreateFloat(string name, string label, string shortLabel, float defaultValue = 0, Action<float> updateAction = null, float min = 0, float max = 1)
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

      var smxManager = new SmxParameterManager(paramInfo, updateAction, min, max);
      Add(smxManager);
      return smxManager;
    }

    protected SmxParameterManager CreateInteger(string name, string label, string shortLabel, int max, int defaultValue = 0, int min = 0, Action<int> updateAction = null)
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

      Action<float>? intUpdateAction = updateAction != null ? f => updateAction((int)f) : null;
      var smxManager = new SmxParameterManager(paramInfo, intUpdateAction, min, max);
      smxManager.IsInteger = true;
      Add(smxManager);
      return smxManager;
    }

    protected SmxParameterManager CreateSwitch(string name, string label, string shortLabel, bool defaultValue = false, Action<bool> updateAction = null)
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

      Action<float>? boolUpdateAction = updateAction != null ? f => updateAction(f == 1) : null;
      var smxManager = new SmxParameterManager(paramInfo, boolUpdateAction);
      smxManager.IsSwitch = true;
      Add(smxManager);
      return smxManager;
    }
  }
}