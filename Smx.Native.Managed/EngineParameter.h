#pragma once
#include "FilterParameter.h"
#include "GeneratorParameter.h"
#include "KeyData.h"
#include "Filter.h"

using namespace System::Collections::Generic;

public ref class EngineParameter
{
public:
  int FilterCount;
  bool FmMod;
  float Attack;
  float Release;
  float InitialDetune;
  float InitialDetuneAcceleration;
  float InitialDetuneFriction;
  float SawAmount;
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
    ActiveKeys = gcnew Dictionary<short, KeyData^>();
    ActiveFilter = gcnew List<Filter^>();
    ActiveGenerators = gcnew List<GeneratorParameter^>();
  };

  List<Filter^>^ ActiveFilter;
  List<GeneratorParameter^>^ ActiveGenerators;
  Dictionary<short, KeyData^>^ ActiveKeys;
};

