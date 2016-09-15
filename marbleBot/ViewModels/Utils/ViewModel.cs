using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace marbleBot.ViewModels.Utils
{
	abstract public class ViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void UpdateProperty([CallerMemberName] string Name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
		}

		protected void UpdateProperty(params string[] names)
		{
			foreach (var name in names)
			{
				UpdateProperty(name);
			}
		}

		protected Dictionary<string, object> propertyValues = new Dictionary<string, object>();

		protected T SetProperty<T>(T value, bool updateProperty = true, [CallerMemberName] string property = null)
		{
			this.propertyValues[property] = value;
			if(updateProperty)
			{
				UpdateProperty(property);
			}
			return value;
		}

		protected object GetProperty([CallerMemberName] string property = null)
		{
			try
			{
				return this.propertyValues[property];
			}
			catch
			{
				return null;
			}
		}
	}
}
