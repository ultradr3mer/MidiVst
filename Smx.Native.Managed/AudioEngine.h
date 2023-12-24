#pragma once
public ref class AudioEngine {
public:
  AudioEngine();  // Constructor
  ~AudioEngine(); // Destructor

  static double Wave(double saw, double t, double pow);
private:
  // Add any private members if needed
};
