#pragma once
#include "ModPara.h"

public ref class EnvelopeLinkParameter {
public:
  double Ammount;
  int EnvelopeNr;
  int TargetId;

  ModPara^ TargetModPara;
};