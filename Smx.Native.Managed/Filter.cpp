#include "Filter.h"
#include "AudioMath.h"

Filter::Filter(FilterParameter^ parameters) {
  this->parameters = parameters;
  this->buf0 = gcnew array<double>(4);
  this->buf1 = gcnew array<double>(4);
}

double Filter::Process(double inputValue) {
  FilterMode mode = static_cast<FilterMode>(parameters->Mode);
  double wet = ProcessInternal(inputValue, mode);
  double wetAmt = parameters->WetAmt->Get();
  return (mode >= FilterMode::LowpassAdd) ? (inputValue + wet * wetAmt)
    : (mode >= FilterMode::LowpassMix) ? AudioMath::Mix(inputValue, wet, wetAmt)
    : inputValue;
}


double Filter::CalcFeedbackAmount(double cutoff) {
  double feedback = static_cast<double>(parameters->Resonance->Get());
  feedback += feedback / (1.0 - AudioMath::Clamp(cutoff, 0.99, 0.01));
  return feedback;
}

double Filter::ProcessInternal(double inputValue, FilterMode mode) {
  int length = parameters->Cycles;
  for (int i = 0; i < length; i++)
  {
    inputValue = ProcessCycle(inputValue, mode, i);
  }
  return inputValue;
}

double Filter::ProcessCycle(double inputValue, FilterMode mode, int cycle)
{
  double cutoff = pow(parameters->Cutoff->Get(), 4.0);
  double feedback = CalcFeedbackAmount(cutoff);

  buf0[cycle] += cutoff * (inputValue - buf0[cycle] + feedback * (buf0[cycle] - buf1[cycle]));
  buf1[cycle] += cutoff * (buf0[cycle] - buf1[cycle]);

  switch (mode) {
  case FilterMode::LowpassMix:
  case FilterMode::LowpassAdd:
    return buf1[cycle];

  case FilterMode::HighpassMix:
  case FilterMode::HighpassAdd:
    return inputValue - buf0[cycle];

  case FilterMode::BandpassMix:
  case FilterMode::BandpassAdd:
    return buf0[cycle] - buf1[cycle];

  default:
    return inputValue;
  }
}
