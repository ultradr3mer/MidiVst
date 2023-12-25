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
  double GenerateKey(KeyData^ data);
  double GenerateVoice(KeyData^ data, int voiceNr);
private:
  std::map<int, float> InitializeNoteFrequencies();
  std::map<int, float>* noteFrequencies;
  EngineParameter^ params;
};
