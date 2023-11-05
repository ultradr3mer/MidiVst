using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Jacobi.Vst.Samples.MidiNoteSampler.Enums
{
  internal enum MidiChannelVoiceMessages
  {
    NoteOffEvent = 0b10000000, // This message is sent when a note is released (ended). (kkkkkkk) is the key (note) number. (vvvvvvv) is the velocity."
    NoteOnEvent = 0b10010000, //This message is sent when a note is depressed (start). (kkkkkkk) is the key (note) number. (vvvvvvv) is the velocity."
    PolyphonicKeyPressure = 0b10100000, //This message is most often sent by pressing down on the key after it ""bottoms out"". (kkkkkkk) is the key (note) number. (vvvvvvv) is the pressure value."
    ControlChange = 0b10110000, //This message is sent when a controller value changes. Controllers include devices such as pedals and levers. Controller numbers 120-127 are reserved as ""Channel Mode Messages"" (below). (ccccccc) is the controller number (0-119). (vvvvvvv) is the controller value (0-127)."
    ProgramChange = 0b11000000, // This message sent when the patch number changes. (ppppppp) is the new program number.
    ChannelPressure = 0b11010000, // This message is most often sent by pressing down on the key after it "bottoms out". This message is different from polyphonic after-touch.Use this message to send the single greatest pressure value (of all the current depressed keys). (vvvvvvv) is the pressure value.
    PitchBendChange = 0b11100000, // This message is sent to indicate a change in the pitch bender (wheel or lever, typically). The pitch bender is measured by a fourteen bit value. Center (no pitch change) is 2000H. Sensitivity is a function of the receiver, but may be set using RPN 0. (lllllll) are the least significant 7 bits. (mmmmmmm) are the most significant 7 bits.
    
    Mask = 0b11110000 // Mask to filter MidiChannelVoiceMessages
  }
}
