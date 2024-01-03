#pragma once
#include "Envelope.h"

ref class KeyData
{
  public:
    double Actuation;
    double Detune;
    double DetuneVec;
    double Time;
    double KeyFrequency;
    List<Envelope^>^ ActiveEnvelopes;
};
