namespace Smx.Vst.Util
{
  internal class AudioMath
  {
    internal static double Mix(double a, double b, double value)
    {
      return b * value + a * (1 - value);
    }
  }
}