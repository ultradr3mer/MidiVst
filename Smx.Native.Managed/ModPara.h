#pragma once
#include <math.h>

public ref class ModPara
{
public:
  float Get() {
    return result;
  }
  void SetBase(float value)
  {
    result = base = value;
  }
  void SetEnv(float value)
  {
    mod = value;
    result = fmaxf(fminf(base + value, max), min);
  }
  void SetMin(float value)
  {
    min = value;
    this->UpdateScale();
  }
  void SetMax(float value)
  {
    max = value;
    this->UpdateScale();
  }

private:
  float base = 0;
  float mod = 0;
  float min = 0;
  float max = 1;
  float result = 0;
  float scale;
  void UpdateScale()
  {
    scale = max - min;
  }
};
