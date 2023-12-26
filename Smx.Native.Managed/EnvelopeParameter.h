#pragma once

#include "EnvelopeLinkParameter.h"

using namespace System::Collections::Generic;

public ref class EnvelopeParameter {
public:
  EnvelopeParameter() {
    Links = gcnew List<EnvelopeLinkParameter^>();
  };

  double Attack;
  double Decay;
  double Sustain;
  double Release;

  List<EnvelopeLinkParameter^>^ Links;
};