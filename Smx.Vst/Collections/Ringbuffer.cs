using System;

namespace Smx.Vst.Collections
{
  public class Ringbuffer<T>
  {
    #region Fields

    private readonly T[] data;
    private readonly int size;
    private int count = 0;
    private int reader = -1;
    private int writer = -1;

    #endregion Fields

    #region Constructors

    public Ringbuffer(int size)
    {
      this.size = size;
      this.data = new T[size];
    }

    #endregion Constructors

    #region Methods

    public void Add(T value)
    {
      this.writer = this.GetNextIndex(this.writer);
      this.data[this.writer] = value;

      if (reader == -1)
      {
        reader = 0;
        count += 1;
      }
      else if (this.reader == this.writer)
      {
        this.reader = this.GetNextIndex(this.reader);
      }
      else
      {
        count += 1;
      }
    }

    public int Count()
    {
      return this.count;
    }

    public T GetFromTop(int index)
    {
      if (index >= count)
        throw new ArgumentException(nameof(index), "The specified index does not exist");

      var pos = GetPrevIndex(this.writer, index);

      return this.data[pos];
    }

    public int Size()
    {
      return this.size;
    }

    public T Take()
    {
      if (this.Count() == 0)
      {
        throw new Exception("Buffer empty");
      }

      var result = this.data[this.reader];
      this.reader = this.GetNextIndex(this.reader);
      this.count -= 1;
      return result;
    }

    private int GetNextIndex(int pos)
    {
      int next = pos + 1;
      return next < this.size ? next : 0;
    }

    private int GetPrevIndex(int pos, int steps)
    {
      int next = pos - steps;
      return next >= 0 ? next : next + size;
    }

    #endregion Methods
  }
}