#include "AudioEngineUnmanaged.h"
#include <cmath>
#include "math.h"

AudioEngineUnmanaged::AudioEngineUnmanaged()
{
  this->noteFrequencies = this->InitializeNoteFrequencies();
}

AudioEngineUnmanaged::~AudioEngineUnmanaged()
{
}

double AudioEngineUnmanaged::Wave(double saw, double t, double pow)
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

std::map<int, float> AudioEngineUnmanaged::InitializeNoteFrequencies()
{
  std::map<int, float> result;

  float a4Frequency = 440.0f;
  float multiplier = pow(2.0f, 1.0f / 12.0f);

  for (int x = 0; x < 127; ++x) {
    result[x] = static_cast<float>(a4Frequency * pow(multiplier, x - 69));
  }

  return result;
}

//double AudioEngineUnmanaged::GenerateVoice(unsigned char key, KeyData data, std::vector<GeneratorParameterUnmanaged> gens, int v) {
//  //double shiftPerVoice = para->VoiceSpread / noteFrequencies[key] / (*std::min_element(gens.begin(), gens.end(), [](const auto& a, const auto& b) { return a.Factor < b.Factor; })).Factor;
//  //double voiceTime = data.Time * (1.0 + v / 10.0 * parameters.VoiceDetuneMgrCurrentValue) + shiftPerVoice * v;
//
//  //int shiftNr = 0;
//  //auto CalcTime = [this, &voiceTime, &key](GeneratorParameter genPara) {
//  //  return voiceTime * noteFrequencies[key] * 4.0 * parameters.TuneMgrCurrentValue * genPara.Factor
//  //    * (1.0 + shiftNr / 100.0 * parameters.UniDetMgrCurrentValue)
//  //    + parameters.UniPanMgrCurrentValue * shiftNr++ / gens.size();
//  //  };
//
//  //double result = (data.Actuation * ((parameters. == 1)
//  //  ? std::accumulate(gens.begin(), gens.end(), 1.0, [&CalcTime, this](double a, const GeneratorParameterUnmanaged& g) {
//  //    return a * 1.5 * AudioEngineUnmanaged::Wave(parameters.SawMgrCurrentValue, CalcTime(g), parameters.PowMgrCurrentValue);
//  //    })
//  //  : std::accumulate(gens.begin(), gens.end(), 0.0, [&CalcTime, this](double a, const GeneratorParameterUnmanaged& g) {
//  //      return a + AudioEngineUnmanaged::Wave(parameters.SawMgrCurrentValue, CalcTime(g), parameters.PowMgrCurrentValue);
//  //    })));
//
//  //return result;
//  //return 0;
//}