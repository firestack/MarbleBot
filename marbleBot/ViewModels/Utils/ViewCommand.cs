using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace marbleBot.ViewModels.Utils
{
	public class ViewCommand : ICommand
	{
		protected Action<object> execute;
		protected Predicate<object> canExecute;

		public ViewCommand(Action<object> Execute) : this(Execute, null) { }

		public ViewCommand(Action<object> Execute, Predicate<object> CanExecute)
		{
			execute = Execute;
			canExecute = CanExecute;
		}

		public void ReEval()
		{
			CanExecuteChanged(this, EventArgs.Empty);
		}

#pragma warning disable 67 // "Is Never used" callback method that other things bind to
		public event EventHandler CanExecuteChanged;
#pragma warning restore 67

		public virtual bool CanExecute(object parameter)
		{
			return canExecute == null ? true : canExecute(parameter);
		}

		public virtual void Execute(object parameter)
		{
			execute(parameter);
		}
	}
}
