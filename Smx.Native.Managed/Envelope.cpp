#include "Envelope.h"

double Envelope::nextSample() {
  if (currentStage != ENVELOPE_STAGE_OFF &&
    currentStage != ENVELOPE_STAGE_SUSTAIN) {
    if (currentSampleIndex == nextStageSampleIndex) {
      EnvelopeStage newStage = static_cast<EnvelopeStage>(
        (currentStage + 1) % kNumEnvelopeStages
        );
      enterStage(newStage);
    }
    currentLevel *= multiplier;
    currentSampleIndex++;
  }
  return currentLevel;
}

bool Envelope::Step(bool released)
{
  bool result = currentStage != ENVELOPE_STAGE_OFF;

  bool isReleasing = currentStage == ENVELOPE_STAGE_RELEASE
                    || currentStage == ENVELOPE_STAGE_OFF;

  if (released 
    && !isReleasing)
  {
    this->enterStage(ENVELOPE_STAGE_RELEASE);
  }
  else if (!released 
    && isReleasing)
  {
    attackMinimumLevel = currentLevel;
    this->enterStage(ENVELOPE_STAGE_ATTACK);
  }

  double sample = this->nextSample();

  for each (auto item in parameters->Links)
  {
    double value = sample * item->Ammount;
    item->TargetModPara->SetEnv(value);
  }

  return result;
}

void Envelope::calculateMultiplier(double startLevel,
  double endLevel,
  unsigned long long lengthInSamples) {
  multiplier = 1.0 + (log(endLevel) - log(startLevel)) / (lengthInSamples);
}

double Envelope::getStageValue(EnvelopeStage stage)
{
  switch (stage)
  {
  case ENVELOPE_STAGE_ATTACK:
    return parameters->Attack;
  case ENVELOPE_STAGE_DECAY:
    return parameters->Decay;
  case ENVELOPE_STAGE_SUSTAIN:
    return parameters->Sustain;
  case ENVELOPE_STAGE_RELEASE:
    return parameters->Release;
  default:
    return 0.0;
    break;
  }
}

void Envelope::enterStage(EnvelopeStage newStage) {
  currentStage = newStage;
  currentSampleIndex = 0;
  if (currentStage == ENVELOPE_STAGE_OFF ||
    currentStage == ENVELOPE_STAGE_SUSTAIN) {
    nextStageSampleIndex = 0;
  }
  else {
    nextStageSampleIndex = getStageValue(currentStage) * sampleRate;
  }
  switch (newStage) {
  case ENVELOPE_STAGE_OFF:
    currentLevel = 0.0;
    multiplier = 1.0;
    break;
  case ENVELOPE_STAGE_ATTACK:
    currentLevel = attackMinimumLevel;
    calculateMultiplier(currentLevel,
      1.0,
      nextStageSampleIndex);
    break;
  case ENVELOPE_STAGE_DECAY:
    currentLevel = 1.0;
    calculateMultiplier(currentLevel,
      fmax(getStageValue(ENVELOPE_STAGE_SUSTAIN), minimumLevel),
      nextStageSampleIndex);
    break;
  case ENVELOPE_STAGE_SUSTAIN:
    currentLevel = getStageValue(ENVELOPE_STAGE_SUSTAIN);
    multiplier = 1.0;
    break;
  case ENVELOPE_STAGE_RELEASE:
    // We could go from ATTACK/DECAY to RELEASE,
    // so we're not changing currentLevel here.
    calculateMultiplier(currentLevel,
      minimumLevel,
      nextStageSampleIndex);
    break;
  default:
    break;
  }
}

void Envelope::setSampleRate(double newSampleRate) {
  sampleRate = newSampleRate;
}

