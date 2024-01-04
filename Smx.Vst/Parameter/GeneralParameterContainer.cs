using Jacobi.Vst.Plugin.Framework;
using Smx.Vst.Util;
using System.Collections.Generic;

namespace Smx.Vst.Parameter
{
  internal class GeneralParameterContainer : ParameterContainer
  {
    public GeneralParameterContainer(VstParameterCategory paramCategory) : base(paramCategory)
    {
      EngineParameter = new EngineParameter();

      AmpMgr = CreateFloatMod(name: "Amp",
                      label: "Amp Amount",
                      shortLabel: "Amp Amt.",
                      defaultValue: 1,
                      modPara: EngineParameter.AmpAmount);
      SawMgr = CreateFloatMod(name: "Saw",
                                  label: "Saw Amount (1.0 Saw - 0.0 Sin)",
                                  shortLabel: "Saw Amt.",
                                  modPara: EngineParameter.SawAmount);
      FmModMgr = CreateSwitch("FmMod",
                              "FM Modulation",
                              "FM Mod",
                           updateAction: v => EngineParameter.FmMod = v);
      PowMgr = CreateFloatMod(name: "Pow",
                           label: "Power to raise by",
                           shortLabel: "Power",
                           defaultValue: 1.0f,
                           modPara: EngineParameter.Pow);


      UniDetMgr = CreateFloatMod(name: "UniDet",
                              label: "Unison Detune",
                              shortLabel: "Uni.Det.",
                              modPara: EngineParameter.UniDetune);
      UniPanMgr = CreateFloatMod(name: "UniPan",
                              label: "Unison Pan",
                              shortLabel: "Uni.Pan.",
                              modPara: EngineParameter.UniPan);

      TuneMgr = CreateFloatMod(name: "Tune",
                            label: "Tune",
                            shortLabel: "Tune",
                            defaultValue: 1.0f,
                            min: 0.0f,
                            max: 2.0f,
                            modPara: EngineParameter.Tune);

      VoiceCountMgr = CreateInteger(name: "VoiCount",
            label: "Voice Count",
            shortLabel: "Voi.Cou.",
            min: 1,
            max: 8,
            defaultValue: 1,
            updateAction: v => EngineParameter.VoiceCount = v);

      VoiceDetuneMgr = CreateFloatMod(name: "VoiDet",
            label: "Voice Detune",
            shortLabel: "Voi.Det.",
            modPara: EngineParameter.VoiceDetune);

      VoiceSpreadMgr = CreateFloatMod(name: "VoiSprd",
            label: "Voice Spread",
            shortLabel: "Voi.Spr.",
            modPara: EngineParameter.VoiceSpread);
    }

    public EngineParameter EngineParameter { get; }
    public SmxParameterManager AmpMgr { get; }
    public SmxParameterManager FmModMgr { get; private set; }
    public SmxParameterManager PowMgr { get; private set; }
    public SmxParameterManager SawMgr { get; private set; }
    public SmxParameterManager TuneMgr { get; private set; }
    public SmxParameterManager UniDetMgr { get; private set; }
    public SmxParameterManager UniPanMgr { get; private set; }
    public SmxParameterManager VoiceCountMgr { get; private set; }
    public SmxParameterManager VoiceDetuneMgr { get; private set; }
    public SmxParameterManager VoiceSpreadMgr { get; private set; }
  }
}