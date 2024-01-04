#pragma once
#include "FilterParameter.h"
#include "FilterMode.h"

public ref class Filter {
private:
  FilterParameter^ parameters;
  array<double>^ buf0;
  array<double>^ buf1;

public:
  Filter(FilterParameter^ parameters);
  double Process(double inputValue);

private:
  double CalcFeedbackAmount(double cutoff);
  double ProcessInternal(double inputValue, FilterMode mode);
  double ProcessCycle(double inputValue, FilterMode mode, int cycle);
};
