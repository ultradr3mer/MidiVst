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

            SawMgr = CreateFloatMod(name: "Saw",
                                  label: "Saw Amount (1.0 Saw - 0.0 Sin)",
                                  shortLabel: "Saw Amt.",
                                  modPara: EngineParameter.SawAmount);
            FmModMgr = CreateSwitch("FmMod",
                                    "FM Modulation",
                                    "FM Mod",
                                 updateAction: v => EngineParameter.FmMod = v);
            AttackMgr = CreateFloat(name: "Attack",
                                 label: "Attack Time",
                                 shortLabel: "Attack",
                                 updateAction: v => EngineParameter.Attack = v);
            ReleaseMgr = CreateFloat(name: "Release",
                                 label: "Release Time",
                                 shortLabel: "Release",
                                 updateAction: v => EngineParameter.Release = v);
            PowMgr = CreateFloat(name: "Pow",
                                 label: "Power to raise by",
                                 shortLabel: "Power",
                                 defaultValue: 1.0f,
                                 updateAction: v => EngineParameter.Pow = v);

            IniDetMgr = CreateFloat(name: "IniDet",
                                    label: "Initial Detune",
                                    shortLabel: "Ini.Det.",
                                    defaultValue: 1.0f,
                                    min: 0.0f,
                                    max: 2.0f,
                                    updateAction: v => EngineParameter.InitialDetune = v);
            InTuAcMgr = CreateFloat(name: "InTuAc",
                                    label: "Initial Detune Acceleration",
                                    shortLabel: "I.De.Ac.",
                                    updateAction: v => EngineParameter.InitialDetuneAcceleration = v);
            InTuFrMgr = CreateFloat(name: "InTuFr",
                                    label: "Initial Detune Friction",
                                    shortLabel: "I.De.Fr.",
                                    updateAction: v => EngineParameter.InitialDetuneFriction = v);

            UniDetMgr = CreateFloat(name: "UniDet",
                                    label: "Unison Detune",
                                    shortLabel: "Uni.Det.",
                                    updateAction: v => EngineParameter.UniDetune = v);
            UniPanMgr = CreateFloat(name: "UniPan",
                                    label: "Unison Pan",
                                    shortLabel: "Uni.Pan.",
                                    updateAction: v => EngineParameter.UniPan = v);

            TuneMgr = CreateFloat(name: "Tune",
                                  label: "Tune",
                                  shortLabel: "Tune",
                                  defaultValue: 1.0f,
                                  min: 0.0f,
                                  max: 2.0f,
                                  updateAction: v => EngineParameter.Tune = v);

            VoiceCountMgr = CreateInteger(name: "VoiCount",
                  label: "Voice Count",
                  shortLabel: "Voi.Cou.",
                  min: 1,
                  max: 8,
                  defaultValue: 1,
                  updateAction: v => EngineParameter.VoiceCount = v);

            VoiceDetuneMgr = CreateFloat(name: "VoiDet",
                  label: "Voice Detune",
                  shortLabel: "Voi.Det.",
                  updateAction: v => EngineParameter.VoiceDetune = v);

            VoiceSpreadMgr = CreateFloat(name: "VoiSprd",
                  label: "Voice Spread",
                  shortLabel: "Voi.Spr.",
                  updateAction: v => EngineParameter.VoiceSpread = v);
        }

        public SmxParameterManager AttackMgr { get; private set; }
        public EngineParameter EngineParameter { get; }
        public SmxParameterManager FmModMgr { get; private set; }
        public SmxParameterManager IniDetMgr { get; private set; }
        public SmxParameterManager InTuAcMgr { get; private set; }
        public SmxParameterManager InTuFrMgr { get; private set; }
        public SmxParameterManager PowMgr { get; private set; }
        public SmxParameterManager ReleaseMgr { get; private set; }
        public SmxParameterManager SawMgr { get; private set; }
        public SmxParameterManager TuneMgr { get; private set; }
        public SmxParameterManager UniDetMgr { get; private set; }
        public SmxParameterManager UniPanMgr { get; private set; }
        public SmxParameterManager VoiceCountMgr { get; private set; }
        public SmxParameterManager VoiceDetuneMgr { get; private set; }
        public SmxParameterManager VoiceSpreadMgr { get; private set; }
    }
}