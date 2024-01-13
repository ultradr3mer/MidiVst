#include "AudioEngine.h"
#include <cmath>
#include "math.h"

AudioEngine::AudioEngine(EngineParameter^ params)
{
  this->params = params;
  this->noteFrequencies = InitializeNoteFrequencies();
  this->activeKeys = gcnew Dictionary<int, KeyData^>();
  this->keysToRemove = gcnew List<short>();
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

void AudioEngine::UpdateKeys(Dictionary<short, int>^ currentKeys)
{
  for each (auto item in currentKeys)
  {
    KeyData^ data;
    if (!this->activeKeys->TryGetValue(item.Value, data))
    {
      data = gcnew KeyData();
      data->KeyFrequency = noteFrequencies[item.Key];

      auto envs = gcnew List<Envelope^>();
      int length = params->ActiveEnvelopes->Count;
      for (int envId = 0; envId < length; envId++)
      {
        auto linkParas = gcnew List<ModPara^>();
        for each (auto linkP in params->EnvelopeLinks)
        {
          if (linkP->EnvelopeNr != envId)
          {
            continue;
          }

          auto modPara = params->ModParameter[linkP->TargetId];
          modPara->SetAmt(linkP->Ammount);

          linkParas->Add(modPara);
        }
        
        if (linkParas->Count == 0)
        {
          continue;
        }

        envs->Add(gcnew Envelope(params->ActiveEnvelopes[envId], linkParas, params->SampleRate));
      }

      data->ActiveEnvelopes = envs;

      auto filter = gcnew List<Filter^>();
      for each (auto p in params->ActiveFilter)
      {
        if (p->Mode == FilterMode::None)
        {
          continue;
        }

        filter->Add(gcnew Filter(p));
      }

      data->ActiveFilter = filter;

      this->activeKeys[item.Value] = data;
    }
  }

  if (keysToRemove->Count > 0)
  {
    for each (short key in this->keysToRemove)
    {
      this->activeKeys->Remove(key);
    }
    keysToRemove->Clear();
  }
}

void AudioEngine::Run(Dictionary<short, int>^ currentKeys, int length, array<float*>^ outBuffer)
{

  int channelCount = outBuffer->Length;
  for (int sampleIndex = 0; sampleIndex < length; sampleIndex++)
  {
    float sample = GenerateSample(currentKeys);

    for (int channelIndex = 0; channelIndex < channelCount; channelIndex++)
    {
      float* ptr = outBuffer[channelIndex];
      *ptr = sample;
      ptr++;
      outBuffer[channelIndex] = ptr;
    }
  }
}

double AudioEngine::GenerateSample(Dictionary<short, int>^ currentKeys)
{
  UpdateKeys(currentKeys);

  double sample = GenerateKeys(currentKeys);

  return sample;
}

double AudioEngine::GenerateKeys(Dictionary<short, int>^ currentKeys)
{
  double sample = 0.0;
  for each (auto item in this->activeKeys)
  {
    bool envOff = true;
    bool released = !currentKeys->ContainsValue(item.Key);

    params->AmpAmount->SetEnv(released ? -1.0 : 0.0);

    for each (auto env in item.Value->ActiveEnvelopes)
    {
      envOff &= !env->Step(released);
    }

    if (envOff && released)
    {
      this->keysToRemove->Add(item.Key);
      continue;
    }

    item.Value->Time += params->Tune->Get() / params->SampleRate;

    double keySample = GenerateKey(item.Value);

    for each (Filter ^ filter in item.Value->ActiveFilter)
    {
      keySample = filter->Process(keySample);
    }

    sample += keySample;
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
  return sample / length * params->AmpAmount->Get();
}

double AudioEngine::GenerateVoice(KeyData^ data, int voiceNr) {
  double shiftPerVoice = params->VoiceSpread->Get() / data->KeyFrequency / params->MinGenFactor;
  double voiceTime = data->Time * (1.0 + voiceNr / 10.0 * params->VoiceDetune->Get()) + shiftPerVoice * voiceNr;

  int shiftNr = 0;
  double generatorAggregate = params->FmMod ? 1.0 : 0.0;

  for each (GeneratorParameter ^ genPara in params->ActiveGenerators)
  {
    auto time = voiceTime * data->KeyFrequency * 4.0 * genPara->Factor
      * (1.0 + shiftNr / 100.0 * params->UniDetune->Get())
      + params->UniPan->Get() * shiftNr++ / params->ActiveGenerators->Count;

    double sample = AudioEngine::Wave(params->SawAmount->Get(), time, params->Pow->Get());

    generatorAggregate = params->FmMod ? (generatorAggregate * 1.5 * sample)
      : (generatorAggregate + sample / params->ActiveGenerators->Count);
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