using Jacobi.Vst.Plugin.Framework;
using Smx.Vst.Util;

namespace Smx.Vst.Parameter
{
  internal class EnvelopeLinkParameterContainer : ParameterContainer
  {
    public EnvelopeLinkParameterContainer(VstParameterCategory paramCategory, int i) : base(paramCategory)
    {
      char letter = (char)('A' + i);

      Parameter = new EnvelopeLinkParameter();

      AmmountMgr = CreateFloat(name: "LinkAm" + letter,
                              label: "Link Ammount " + letter,
                              shortLabel: "Lnk.Am." + letter,
                              updateAction: v => Parameter.Ammount = v);
      EnvelopeMgr = CreateInteger(name: "LinkEn" + letter,
                             label: "Link Envelope " + letter,
                             shortLabel: "Lnk.En." + letter,
                             min: -1, // -1 if not assigned
                             max: AudioEngine.MaxEnvelopes,
                             defaultValue: -1,
                             updateAction: v => Parameter.EnvelopeNr = v);
      TargetIdMgr = CreateInteger(name: "LinkTr" + letter,
                                label: "Envelope Target " + letter,
                                shortLabel: "Lnk.Tr." + letter,
                                min: -1, // -1 if not assigned
                                max: 200,
                                defaultValue: -1,
                                updateAction: v => Parameter.TargetId = v);
    }

    public EnvelopeLinkParameter Parameter { get; }
    public SmxParameterManager AmmountMgr { get; }
    public SmxParameterManager EnvelopeMgr { get; }
    public SmxParameterManager TargetIdMgr { get; }
  }
}