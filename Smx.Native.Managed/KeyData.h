#pragma once
#include "Envelope.h"

ref class KeyData
{
  public:
    double Time;
    double KeyFrequency;
    List<Envelope^>^ ActiveEnvelopes;
};
