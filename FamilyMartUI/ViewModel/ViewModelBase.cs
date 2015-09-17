
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using DAL.Common;
using FamilyMartUI.Common;

namespace FamilyMartUI.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Multicast event for property change notifications.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private object obj = new object();
        protected void RunOnUIThread(Action action)
        {
            if (null == action || Application.Current == null)
            {
                return;
            }

            try
            {
                Dispatcher dispatcher = Application.Current.Dispatcher;
                bool isSameThread = false;
                lock (obj)
                {
                    isSameThread = dispatcher.CheckAccess();
                }

                if (isSameThread)
                {
                    action();
                }
                else
                {
                    dispatcher.Invoke((Delegate)(action));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("RunOnUIThread error:{0}", ex.Message));
            }
        }

        protected void RunOnUIThreadAsync(Action action)
        {
            if (null == action || Application.Current == null)
            {
                return;
            }

            try
            {
                Dispatcher dispatcher = Application.Current.Dispatcher;
                bool isSameThread = false;
                lock (obj)
                {
                    isSameThread = dispatcher.CheckAccess();
                }

                if (isSameThread)
                {
                    action();
                }
                else
                {
                    dispatcher.BeginInvoke(DispatcherPriority.Normal, (Delegate)(action));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("RunOnUIThreadAsync error:{0}", ex.Message));
            }
        }

        /// <summary>
        /// Checks if a property already matches a desired value.  Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"),
         System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        protected bool SetProperty<T>(ref T storage, T value, params string[] propertyNames)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            if (!propertyNames.IsNullOrEmpty())
            {
                foreach (var item in propertyNames)
                {
                    this.OnPropertyChanged(item);
                }
            }

            return true;
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected void OnPropertyChanged(string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected bool isDisposed = false;
        protected virtual void OnDisposing(bool isDisposing)
        {
        }

        public void Dispose()
        {
            OnDisposing(true);
            GC.SuppressFinalize(this);
            isDisposed = true;
        }
    }
}
