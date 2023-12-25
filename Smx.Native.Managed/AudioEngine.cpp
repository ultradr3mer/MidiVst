#include "AudioEngine.h"
#include <cmath>
#include "math.h"

AudioEngine::AudioEngine(EngineParameter^ params)
{
  this->params = params;
  this->noteFrequencies = InitializeNoteFrequencies();
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

void AudioEngine::UpdateKeys(HashSet<short>^ currentKeys)
{
  for each (short key in currentKeys)
  {
    KeyData^ data;
    if (!params->ActiveKeys->TryGetValue(key, data))
    {
      data = gcnew KeyData();
      data->Actuation;
      data->Detune = pow(params->InitialDetune * 2.0f, 2.0),
        data->KeyFrequency = noteFrequencies[key];

      params->ActiveKeys[key] = data;
    }

    if (data->Actuation < 1)
    {
      if (params->Attack == 0)
      {
        data->Actuation = 1;
      }
      else
      {
        data->Actuation = fmin(data->Actuation + 1.0 / params->Attack / params->SampleRate, 1.0);
      }
    }
  }

  for each (auto item in params->ActiveKeys)
  {
    float sampleRate = params->SampleRate;
    auto data = item.Value;
    if (data->Detune != 1.0 || data->DetuneVec != 0.0)
    {
      double pull = (1.0 - data->Detune) * params->InitialDetuneAcceleration * 100000.0;
      double scaledFriction = params->InitialDetuneFriction / sampleRate * 1000.0;
      data->DetuneVec = (data->DetuneVec + pull / sampleRate) * (1.0 - scaledFriction);
      data->Detune += data->DetuneVec / sampleRate;
      data->Time += data->Detune / sampleRate;
    }
    else
    {
      data->Time += 1.0 / sampleRate;
    }

    if (currentKeys->Contains(item.Key))
    {
      continue;
    }

    float keyActuation;
    if (params->Release == 0)
    {
      keyActuation = 0;
    }
    else
    {
      keyActuation = fmax(data->Actuation - 1.0 / params->Release / sampleRate, 0.0);
    }

    if (keyActuation <= 0)
    {
      params->ActiveKeys->Remove(item.Key);
    }
    else
    {
      params->ActiveKeys[item.Key]->Actuation = keyActuation;
    }
  }
}

void AudioEngine::Run(HashSet<short>^ currentKeys, int length, array<float*>^ outBuffer)
{
  int channelCount = outBuffer->Length;
  for (int i = 0; i < length; i++)
  {
    float sample = GenerateSample(currentKeys);

    for (int channelIndex = 0; channelIndex < channelCount; channelIndex++)
    {
      float *ptr = outBuffer[channelIndex];
      *ptr = sample;
      ptr++;
      outBuffer[channelIndex] = ptr;
    }
  }
}

double AudioEngine::GenerateSample(HashSet<short>^ currentKeys)
{
  UpdateKeys(currentKeys);

  double sample = GenerateKeys();

  return sample;
}

double AudioEngine::GenerateKeys()
{
  double sample = 0.0;
  for each (KeyData ^ keyData in params->ActiveKeys->Values)
  {
    sample += GenerateKey(keyData);
  }
  return sample;
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

  for each (GeneratorParameter ^ genPara in params->ActiveGenerators)
  {
    auto time = voiceTime * data->KeyFrequency * 4.0 * params->Tune * genPara->Factor
      * (1.0 + shiftNr / 100.0 * params->UniDetune)
      + params->UniPan * shiftNr++ / params->ActiveGenerators->Count;

    double sample = AudioEngine::Wave(params->SawAmount, time, params->Pow);

    generatorAggregate = params->FmMod ? generatorAggregate * 1.5 * sample : generatorAggregate + sample / params->ActiveGenerators->Count;
  }

  return generatorAggregate;
}

Dictionary<int, float>^ AudioEngine::InitializeNoteFrequencies()
{
  auto result = gcnew Dictionary<int, float>();

  float a4Frequency = 440.0f;
  float multiplier = pow(2.0f, 1.0f / 12.0f);

  for (int x = 0; x < 127; ++x) {
    result[x] = static_cast<float>(a4Frequency * pow(multiplier, x - 69));
  }

  return result;
}