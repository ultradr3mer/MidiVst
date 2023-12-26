#pragma once

public ref class ModPara
{
public:
  float Get() {
    return base + mod;
  }
  void SetBase(float value)
  {
    base = value;
  }
  void SetMod(float value)
  {
    mod = value;
  }

private:
  float base;
  float mod;
};
