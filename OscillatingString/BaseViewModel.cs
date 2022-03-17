using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace OscillatingString
{
    public abstract class BaseViewModel : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Sets a property. If values aren't equal, a new value is replaced
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">a reference of a private field of a property</param>
        /// <param name="value">the current value </param>
        /// <param name="propertyName">a name of a selected property [optional]</param>
        /// <returns></returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        /// <summary>
        /// An event that says us about changes 
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handlers = PropertyChanged;
            if (handlers is null) return;
            var invocationList = handlers.GetInvocationList();
            var arg = new PropertyChangedEventArgs(propertyName);
            foreach (var action in invocationList)
            {
                if (action.Target is DispatcherObject dispatcherObject)
                    dispatcherObject.Dispatcher.Invoke(action, this, arg);
                else action.DynamicInvoke(this, arg);
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
            {
                return;
            }
            //Освобождение управляемых ресурсов
            _disposed = true;
        }
    }
}