#pragma once
#include "ModPara.h"

public ref class FilterParameter {
public:
  int Mode;
  int Cycles;
  ModPara^ Cutoff;
  ModPara^ WetAmt;
  ModPara^ Resonance;

  FilterParameter() {
    Cycles = 1;
    Cutoff = gcnew ModPara();
    WetAmt = gcnew ModPara();
    Resonance = gcnew ModPara();
  }
};