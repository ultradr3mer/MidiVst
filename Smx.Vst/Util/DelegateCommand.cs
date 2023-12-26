using System;
using System.Windows.Input;

namespace Smx.Vst.Util
{
  internal class DelegateCommand : ICommand
  {
    bool? lastCanExecute = null;
    private readonly Func<object, bool> canExecute;
    private readonly Action<object> execute;

    public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
    {
      this.execute = execute;
      this.canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
      var newCanExecute = canExecute?.Invoke(parameter) != false;
      if(lastCanExecute != newCanExecute)
      {
        lastCanExecute = newCanExecute;
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
      }

      return newCanExecute;
    }

    public void Execute(object? parameter)
    {
      execute(parameter);
    }
  }
}