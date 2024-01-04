#pragma once
#include "Envelope.h"
#include "Filter.h"

ref class KeyData
{
  public:
    double Time;
    double KeyFrequency;
    List<Envelope^>^ ActiveEnvelopes;
    List<Filter^>^ ActiveFilter;
};
