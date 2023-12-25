using Jacobi.Vst.Core;
using Jacobi.Vst.Plugin.Framework;
using Smx.Vst.Data;
using Smx.Vst.Util;
using System.Linq;
using System.Reflection;
using VstNetAudioPlugin;

//using static Smx.Vst.Smx.Filter;

namespace Smx.Vst.Smx
{
  /// <summary>
  /// Encapsulated delay parameters.
  /// </summary>
  internal sealed class SmxParameters
  {

    /// <summary>
    /// Initializes the paramaters for the Delay component.
    /// </summary>
    /// <param name="parameters"></param>
    public SmxParameters(PluginParameters parameters)
    {
      Throw.IfArgumentIsNull(parameters, nameof(parameters));

      InitializeParameters(parameters);
    }

    public GeneralParameterContainer GeneralParameter { get; private set; }
    public GenParameterContainer GenParameterContainer { get; private set; }
    public FilterParameterContainer[] FilterManagerAry { get; private set; }

    private void InitializeParameters(PluginParameters parameters)
    {
      VstParameterInfoCollection parameterInfos = parameters.ParameterInfos;

      VstParameterCategory paramCategory = parameters.GetParameterCategory("General");

      this.GeneralParameter = new GeneralParameterContainer(paramCategory);
      parameterInfos.AddRange(this.GeneralParameter.ParameterInfos);

      paramCategory = parameters.GetParameterCategory("Filter");

      FilterManagerAry = Enumerable.Range(0, AudioEngine.MaxFilter)
                                    .Select(i => new FilterParameterContainer(paramCategory, i))
                                    .ToArray();
      parameterInfos.AddRange(this.FilterManagerAry.SelectMany(m => m.ParameterInfos));

      paramCategory = parameters.GetParameterCategory("Oscillators");

      this.GenParameterContainer = new GenParameterContainer(paramCategory);
      parameterInfos.AddRange(this.GenParameterContainer.ParameterInfos);
    }
  }
}