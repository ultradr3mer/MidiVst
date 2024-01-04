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
  float Pow;
  float Tune;
  float UniDetune;
  float UniPan;
  int VoiceCount;
  float VoiceSpread;
  float VoiceDetune;
  float MinGenFactor;
  float SampleRate;

  EngineParameter() {
    ActiveFilter = gcnew List<Filter^>();
    ActiveGenerators = gcnew List<GeneratorParameter^>();
    SawAmount = gcnew ModPara();
    AmpAmount = gcnew ModPara();
  };

  List<Filter^>^ ActiveFilter;
  List<EnvelopeParameter^>^ ActiveEnvelopes;
  List<GeneratorParameter^>^ ActiveGenerators;
};

