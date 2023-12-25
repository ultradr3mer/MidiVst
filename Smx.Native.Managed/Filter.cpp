#include "Filter.h"
#include "AudioMath.h"

Filter::Filter(FilterParameter^ parameters) {
  this->parameters = parameters;
}

double Filter::Process(double inputValue) {
  FilterMode mode = static_cast<FilterMode>(parameters->Mode);
  double wet = ProcessInternal(inputValue, mode);
  double wetAmt = parameters->WetAmt;
  return (mode >= FilterMode::LowpassAdd) ? (inputValue + wet * wetAmt)
    : (mode >= FilterMode::LowpassMix) ? AudioMath::Mix(inputValue, wet, wetAmt)
    : inputValue;
}


double Filter::CalcFeedbackAmount(double cutoff) {
  double feedback = static_cast<double>(parameters->Resonance);
  feedback += feedback / (1.0 - AudioMath::Clamp(cutoff, 0.99, 0.01));
  return feedback;
}

double Filter::ProcessInternal(double inputValue, FilterMode mode) {
  double cutoff = pow(parameters->Cutoff, 4.0);
  double feedback = CalcFeedbackAmount(cutoff);

  buf0 += cutoff * (inputValue - buf0 + feedback * (buf0 - buf1));
  buf1 += cutoff * (buf0 - buf1);

  switch (mode) {
  case FilterMode::LowpassMix:
  case FilterMode::LowpassAdd:
    return buf1;

  case FilterMode::HighpassMix:
  case FilterMode::HighpassAdd:
    return inputValue - buf0;

  case FilterMode::BandpassMix:
  case FilterMode::BandpassAdd:
    return buf0 - buf1;

  default:
    return inputValue;
  }
}