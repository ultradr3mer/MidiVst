#pragma once

#include <algorithm>

class AudioMath {
public:
  static double Clamp(double value, double min, double max) {
    return std::min(std::max(value, min), max);
  }

  static double Mix(double a, double b, double value) {
    return b * value + a * (1 - value);
  }
};