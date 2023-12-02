using Jacobi.Vst.Core;
using Jacobi.Vst.Plugin.Framework;
using Jacobi.Vst.Plugin.Framework.Plugin;
using Jacobi.Vst.Samples.MidiNoteSampler.Smx;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using VstNetAudioPlugin;

namespace Jacobi.Vst.Samples.MidiNoteSampler
{
    /// <summary>
    /// The Plugin root class that derives from the framework provided base class that also include the interface manager.
    /// </summary>
    internal sealed class Plugin : VstPluginWithServices
  {
    /// <summary>
    /// Constructs a new instance.
    /// </summary>
    public Plugin()
        : base(name: "VST.NET Midi SMX",
               pluginID: 36373435,
               productInfo: new VstProductInfo("VST.NET 2 Code Samples", "Clara © 2023", 20231201),
               category: VstPluginCategory.Synth)
    { }

    protected override void ConfigureServices(IServiceCollection services)
    {
      services.AddSingleton<PluginParameters>()
                .AddSingletonAll<PluginPrograms>()
                .AddSingleton<VstMidiProgram>()
                .AddSingleton<Smx.Smx>()
                .AddSingletonAll<AudioProcessor>()
                .AddSingletonAll<MidiProcessor>()
                .AddSingletonAll<PluginEditor>();
    }
  }
}
