using System;

namespace Smx.Vst.Util
{
  internal class AudioMath
  {
    internal static double Clamp(double value, double min, double max)
    {
      return Math.Min(Math.Max(value, min), max);
    }

    internal static double Mix(double a, double b, double value)
    {
      return b * value + a * (1 - value);
    }
  }
}