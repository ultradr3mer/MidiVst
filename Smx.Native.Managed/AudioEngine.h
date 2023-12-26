#pragma once
#include "EngineParameter.h"
#include <gcroot.h>
#include <map>
#include <vector>
#include "KeyData.h"

using namespace System::Collections::Generic;

public ref class AudioEngine {
public:
  AudioEngine(EngineParameter^ params);
  ~AudioEngine(); 

  void Run(HashSet<short>^ currentKeys, int length, array<float*>^ outBuffer);
  static int MaxFilter = 4;
  static int MaxEnvelopes = 4;
  static int MaxEnvelopeLinks = 12;
private:
  Dictionary<int, float>^ InitializeNoteFrequencies();
  Dictionary<int, float>^ noteFrequencies;
  EngineParameter^ params;
  static double Wave(double saw, double t, double pow);
  void UpdateKeys(HashSet<short>^ currentKeys);
  double GenerateSample(HashSet<short>^ currentKeys);
  double GenerateKeys();
  double GenerateKey(KeyData^ data);
  double GenerateVoice(KeyData^ data, int voiceNr);
};
