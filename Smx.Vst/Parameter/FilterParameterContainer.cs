using Jacobi.Vst.Plugin.Framework;
using Smx.Vst.Util;

namespace Smx.Vst.Parameter
{
    internal class FilterParameterContainer : ParameterContainer
    {
        public FilterParameterContainer(VstParameterCategory paramCategory, int i) : base(paramCategory)
        {
            FilterParamer = new FilterParameter();

            FltrMdMgr = CreateInteger(name: "FltrMd" + i,
                                      label: "Filter Mode " + i,
                                      shortLabel: "Fil.Md." + i,
                                      min: (int)FilterMode.LowpassMix,
                                      max: (int)FilterMode.BandpassAdd,
                                      defaultValue: (int)FilterMode.None,
                                      updateAction: v => FilterParamer.Mode = v);
            FltrCtMgr = CreateFloat(name: "FltrCt" + i,
                                 label: "Filter Cutoff " + i,
                                 shortLabel: "Fil.Ct." + i,
                                 defaultValue: 0.5f,
                                 updateAction: v => FilterParamer.Cutoff = v);
            FltrWAMgr = CreateFloat(name: "FltrWA" + i,
                                 label: "Filter Wet Amt" + i,
                                 shortLabel: "Fl.W.A." + i,
                                 defaultValue: 1.0f,
                                 updateAction: v => FilterParamer.WetAmt = v);
            FltrReMgr = CreateFloat(name: "FltrRe" + i,
                        label: "Filter Resonance " + i,
                        shortLabel: "Fl.F.b." + i,
                        updateAction: v => FilterParamer.Resonance = v);
        }

        public FilterParameter FilterParamer { get; }
        public SmxParameterManager FltrCtMgr { get; }
        public SmxParameterManager FltrMdMgr { get; }
        public SmxParameterManager FltrReMgr { get; }
        public SmxParameterManager FltrWAMgr { get; }
    }
}