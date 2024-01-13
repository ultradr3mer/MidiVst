#pragma once

#include <cmath>
#include "EnvelopeParameter.h"
#include "EnvelopeStage.h"

public ref class Envelope {
public:
  bool Step(bool released);
  inline EnvelopeStage getCurrentStage() { return currentStage; };

  Envelope(EnvelopeParameter^ paras, List<ModPara^>^ linkedParams, double sampleRate) :
    minimumLevel(0.0001),
    attackMinimumLevel(0.0001),
    currentStage(EnvelopeStage::ENVELOPE_STAGE_OFF),
    currentLevel(minimumLevel),
    multiplier(1.0),
    sampleRate(sampleRate),
    currentSampleIndex(0),
    nextStageSampleIndex(0),
    linkedParams(linkedParams),
    parameters(paras) {
    enterStage(EnvelopeStage::ENVELOPE_STAGE_ATTACK);
  };
private:
  double minimumLevel;
  double attackMinimumLevel;
  EnvelopeStage currentStage;
  double currentLevel;
  double multiplier;
  double sampleRate;
  EnvelopeParameter^ parameters;
  List<ModPara^>^ linkedParams;
  void calculateMultiplier(double startLevel, double endLevel, unsigned long long lengthInSamples);
  double getStageValue(EnvelopeStage stage);
  unsigned long long currentSampleIndex;
  unsigned long long nextStageSampleIndex;
  void setSampleRate(double newSampleRate);
  void enterStage(EnvelopeStage newStage);
  double nextSample();
};
