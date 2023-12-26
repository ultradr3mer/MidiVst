#pragma once
#include "EngineParameter.h"

public ref class EngineParameterAccessor
{
public:
  EngineParameter^ engineParameter;
  EngineParameterAccessor()
  {
    var acc = gcnew EnvelopeAccessor(&getSawAmount)
  }
  double Detune;
  double DetuneVec;
  double Time;
  double KeyFrequency;

private:
  float getSawAmount()
  {
    return engineParameter->SawAmount;
  }
};
