using Jacobi.Vst.Core;
using Jacobi.Vst.Plugin.Framework;
using Jacobi.Vst.Plugin.Framework.Plugin;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;

namespace Jacobi.Vst.Samples.MidiNoteSampler
{
  /// <summary>
  /// The Plugin root class that derives from the framework provided base class that also include the interface manager.
  /// </summary>
  internal sealed class Plugin : VstPluginWithServices
  {
    private PluginCommandStub pluginCommandStub;

    /// <summary>
    /// Constructs a new instance.
    /// </summary>
    public Plugin(PluginCommandStub pluginCommandStub)
        : base("VST.NET 2 Midi Note Sampler", 36373435,
            new VstProductInfo("VST.NET 2 Code Samples", "Jacobi Software © 2008-2020", 2001),
            VstPluginCategory.Synth)
    { this.pluginCommandStub = pluginCommandStub; }

    protected override void ConfigureServices(IServiceCollection services)
    {
      //Microsoft.Extensions.Configuration.IConfiguration
      //Jacobi.Vst.Samples.MidiNoteSampler.Plugin
      //Jacobi.Vst.Plugin.Framework.IVstPlugin
      //Jacobi.Vst.Plugin.Framework.IExtensible
      //System.IDisposable
      //Jacobi.Vst.Plugin.Framework.IVstPluginEvents
      //Jacobi.Vst.Plugin.Framework.IConfigurable
      //Microsoft.Extensions.Options.IOptions`1[TOptions]
      //Microsoft.Extensions.Options.IOptionsSnapshot`1[TOptions]
      //Microsoft.Extensions.Options.IOptionsMonitor`1[TOptions]
      //Microsoft.Extensions.Options.IOptionsFactory`1[TOptions]
      //Microsoft.Extensions.Options.IOptionsMonitorCache`1[TOptions]
      //Microsoft.Extensions.Logging.ILoggerFactory
      //Microsoft.Extensions.Logging.ILogger`1[TCategoryName]
      //Microsoft.Extensions.Options.IConfigureOptions`1[Microsoft.Extensions.Logging.LoggerFilterOptions]
      //Jacobi.Vst.Samples.MidiNoteSampler.Generator
      //Jacobi.Vst.Samples.MidiNoteSampler.AudioProcessor
      //Jacobi.Vst.Plugin.Framework.IVstPluginAudioProcessor
      //Jacobi.Vst.Samples.MidiNoteSampler.MidiProcessor
      //Jacobi.Vst.Plugin.Framework.IVstMidiProcessor

      services.AddSingleton(this.pluginCommandStub)
                .AddSingleton<VstMidiProgram>()
                .AddSingleton<Generator>()
                .AddSingletonAll<AudioProcessor>()
                .AddSingletonAll<MidiProcessor>();
    }
  }
}
