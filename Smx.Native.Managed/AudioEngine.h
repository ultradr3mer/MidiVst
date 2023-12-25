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

  static double Wave(double saw, double t, double pow);
  void UpdateKeys(HashSet<short>^ currentKeys);
  void Run(HashSet<short>^ currentKeys, int length, array<float*>^ outBuffer);
  double GenerateSample(HashSet<short>^ currentKeys);
  double GenerateKeys();
  double GenerateKey(KeyData^ data);
  double GenerateVoice(KeyData^ data, int voiceNr);
private:
  Dictionary<int, float>^ InitializeNoteFrequencies();
  Dictionary<int, float>^ noteFrequencies;
  EngineParameter^ params;
};
