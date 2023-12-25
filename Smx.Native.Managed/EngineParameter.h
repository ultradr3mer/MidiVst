#pragma once
#include "FilterParameter.h"
#include "GeneratorParameter.h"

using namespace System::Collections::Generic;

public ref class EngineParameter
{
public:
  int FilterCount;
  bool FmMod;
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

  List<FilterParameter^>^ FilterParameter;
  List<GeneratorParameter^>^ ActiveGenerators;
};

