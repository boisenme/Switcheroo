using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Switcheroo.Core;

namespace Switcheroo
{
    public class AppWindowViewModel : INotifyPropertyChanged, IWindowText
    {
        public AppWindowViewModel(AppWindow appWindow)
        {
            AppWindow = appWindow;
        }

        public AppWindowViewModel(JToken tab, int port)
        {
            ChromeTab = tab;
            AppWindow = AppWindow.CreateForChromeTab(
                tab["title"].ToString(),
                tab["url"].ToString(),
                tab["id"].ToString(),
                port);
        }

        public AppWindow AppWindow { get; private set; }
        private readonly JToken _chromeTab;

        #region IWindowText Members

        public string WindowTitle
        {
            get { return AppWindow != null ? AppWindow.GetTitle() : _chromeTab["title"].ToString(); }
        }

        public string ProcessTitle
        {
            get { return AppWindow != null ? AppWindow.ProcessTitle : "Chrome"; }
        }

        #endregion

        #region Bindable properties

        public IntPtr HWnd
        {
            get { return AppWindow != null ? AppWindow.HWnd : IntPtr.Zero; }
        }

        private string _formattedTitle;

        public string FormattedTitle
        {
            get { return _formattedTitle; }
            set
            {
                _formattedTitle = value;
                NotifyOfPropertyChange(() => FormattedTitle);
            }
        }

        private string _formattedProcessTitle;

        public string FormattedProcessTitle
        {
            get { return _formattedProcessTitle; }
            set
            {
                _formattedProcessTitle = value;
                NotifyOfPropertyChange(() => FormattedProcessTitle);
            }
        }

        private bool _isBeingClosed = false;
        private JToken tab;

        public bool IsBeingClosed
        {
            get { return _isBeingClosed; }
            set
            {
                _isBeingClosed = value;
                NotifyOfPropertyChange(() => IsBeingClosed);
            }
        }

        public JToken ChromeTab { get; }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyOfPropertyChange<T>(Expression<Func<T>> property)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(GetPropertyName(property)));
        }

        private string GetPropertyName<T>(Expression<Func<T>> property)
        {
            var lambda = (LambdaExpression) property;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression) lambda.Body;
                memberExpression = (MemberExpression) unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression) lambda.Body;
            }

            return memberExpression.Member.Name;
        }

        #endregion
    }
}