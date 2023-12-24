#include "AudioEngine.h"
#include <cmath>
#include "math.h"

AudioEngine::AudioEngine()
{
}

AudioEngine::~AudioEngine()
{
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
