#include "AudioEngine.h"
#include <cmath>
#include "math.h"

AudioEngine::AudioEngine(EngineParameter^ params)
{
  this->params = params;
  this->noteFrequencies = &InitializeNoteFrequencies();
}

AudioEngine::~AudioEngine()
{
  delete this->noteFrequencies;
}

double AudioEngine::Wave(double saw, double t, double pow)
{
  t = fmod(t, 1.0);
  double segment13_length = (1.0 - saw) / 4.0;
  double segment2_length = (1.0 + saw) / 2.0;

  if (t < segment13_length) {
    t = t / (1 - saw);
  }
  else if (t < segment13_length + segment2_length) {
    t = (t - segment13_length) / (1.0 + saw) + 1.0 / 4.0;
  }
  else {
    t = 1 - (1 - t) / (1 - saw);
  }

  double sin_wave = std::sin(2 * M_PI * t);

  double saw_wave = t < 1.0 / 4.0 ? t * 4.0 :
    t < 3.0 / 4.0 ? 1 - (t - 1.0 / 4.0) * 4.0 :
    -4.0 + 4.0 * t;

  double combined = (1 - saw) * sin_wave + saw * saw_wave;

  return std::pow(std::abs(combined), pow) * std::copysign(1.0, combined);
}

double AudioEngine::GenerateKey(KeyData^ data)
{
  int length = params->VoiceCount;
  double sample = 0.0;
  for (int i = 0; i < length; i++)
  {
    sample += GenerateVoice(data, i);
  }
  return sample / length;
}

double AudioEngine::GenerateVoice(KeyData^ data, int vocieNr) {

  double shiftPerVoice = params->VoiceSpread / data->KeyFrequency / params->MinGenFactor;
  double voiceTime = data->Time * (1.0 + vocieNr / 10.0 * params->VoiceDetune) + shiftPerVoice * vocieNr;

  int shiftNr = 0;
  double generatorAggregate = params->FmMod ? 1.0 : 0.0;

  for each (GeneratorParameter^ genPara in params->ActiveGenerators)
  {
    auto time =  voiceTime * data->KeyFrequency * 4.0 * params->Tune * genPara->Factor
     * (1.0 + shiftNr / 100.0 * params->UniDetune)
     + params->UniPan * shiftNr++ / params->ActiveGenerators->Count;

    double sample = AudioEngine::Wave(params->SawAmount, time, params->Pow);

    generatorAggregate = params->FmMod ? generatorAggregate * 1.5 * sample : generatorAggregate + sample / params->ActiveGenerators->Count;
  }

  return generatorAggregate;
}

std::map<int, float> AudioEngine::InitializeNoteFrequencies()
{
  std::map<int, float> result;

  float a4Frequency = 440.0f;
  float multiplier = pow(2.0f, 1.0f / 12.0f);

  for (int x = 0; x < 127; ++x) {
    result[x] = static_cast<float>(a4Frequency * pow(multiplier, x - 69));
  }

  return result;
}
