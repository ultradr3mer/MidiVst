#pragma once
#include "EngineParameter.h"
#include <gcroot.h>
#include <map>
#include <vector>
#include "KeyData.h"
#include "GeneratorParametersUnmanaged2.h"

using namespace System::Collections::Generic;

public class AudioEngineUnmanaged {
public:
  AudioEngineUnmanaged();
  ~AudioEngineUnmanaged(); 

  static double Wave(double saw, double t, double pow);
private:
  std::map<int, float> InitializeNoteFrequencies();
  //double GenerateVoice(unsigned char key, KeyData data, std::vector<GeneratorParameterUnmanaged> gens, int v);
  std::map<int, float> noteFrequencies;
};
