#pragma once
#include "FilterParameter.h"
#include "GeneratorParameter.h"
#include "KeyData.h"
#include "Filter.h"
#include "EnvelopeParameter.h"
#include "ModPara.h"

using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;

public ref class EngineParameter
{
public:
  ModPara^ AmpAmount;
  bool FmMod;
  ModPara^ SawAmount;
  ModPara^ Pow;
  ModPara^ Tune;
  ModPara^ UniDetune;
  ModPara^ UniPan;
  int VoiceCount;
  ModPara^ VoiceSpread;
  ModPara^ VoiceDetune;
  float SampleRate;
  float MinGenFactor;

  EngineParameter() {
    ActiveFilter = gcnew List<Filter^>();
    ActiveGenerators = gcnew List<GeneratorParameter^>();
    AmpAmount = gcnew ModPara();
    SawAmount = gcnew ModPara();
    Pow = gcnew ModPara();
    Tune = gcnew ModPara();
    UniDetune = gcnew ModPara();
    UniPan = gcnew ModPara();
    VoiceSpread = gcnew ModPara();
    VoiceDetune = gcnew ModPara();
  };

  List<Filter^>^ ActiveFilter;
  List<EnvelopeParameter^>^ ActiveEnvelopes;
  List<GeneratorParameter^>^ ActiveGenerators;
};

