namespace Smx.Vst
{
  using global::Smx.Vst.Enums;
  using Jacobi.Vst.Core;
  using Jacobi.Vst.Plugin.Framework;
  using System;
  using System.Collections.Concurrent;

  /// <summary>
  /// Manages incoming midi events and sents them to the <see cref="SampleManager"/>.
  /// </summary>
  internal sealed class MidiProcessor : IVstMidiProcessor
  {
    private readonly Smx.SmxGenerator generator;

    /// <summary>
    /// Constructs a new instance.
    /// </summary>
    /// <param name="plugin"></param>
    public MidiProcessor(Smx.SmxGenerator generator)
    {
      this.generator = generator ?? throw new ArgumentNullException(nameof(generator));
    }

    #region IVstMidiProcessor Members

    /// <summary>
    /// Always returns 16.
    /// </summary>
    public int ChannelCount
    {
      get { return 16; }
    }

    /// <summary>
    /// Handles and filters the incoming midi events.
    /// </summary>
    /// <param name="events">The midi events for the current cycle.</param>
    public void Process(VstEventCollection events)
    {
      var keyOffQueue = new ConcurrentQueue<byte>();
      var keyOnQueue = new ConcurrentQueue<byte>();

      foreach (VstEvent evnt in events)
      {
        if (evnt.EventType == VstEventTypes.MidiEvent)
        {
          var midiEvent = (VstMidiEvent)evnt;

          // pass note on and note off to the sample manager
          if ((midiEvent.Data[0] & (int)MidiChannelVoiceMessages.Mask) == (int)MidiChannelVoiceMessages.NoteOffEvent)
          {
            keyOffQueue.Enqueue(midiEvent.Data[1]);
          }

          if ((midiEvent.Data[0] & (int)MidiChannelVoiceMessages.Mask) == (int)MidiChannelVoiceMessages.NoteOnEvent)
          {
            // note on with velocity = 0 is a note off
            if (midiEvent.Data[2] == 0)
            {
              keyOffQueue.Enqueue(midiEvent.Data[1]);
            }
            else
            {
              keyOnQueue.Enqueue(midiEvent.Data[1]);
            }
          }
        }
      }

      foreach (var item in keyOnQueue)
      {
        generator.ProcessNoteOnEvent(item);
      }

      foreach (var item in keyOffQueue)
      {
        generator.ProcessNoteOffEvent(item);
      }
    }

    #endregion IVstMidiProcessor Members
  }
}