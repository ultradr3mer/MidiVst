#pragma once

#include "EnvelopeLinkParameter.h"

using namespace System::Collections::Generic;

public ref class EnvelopeParameter {
public:
  EnvelopeParameter() {
  };

  double Attack = 0.01;
  double Decay = 0.5;
  double Sustain = 0.1;
  double Release = 1.0;
};