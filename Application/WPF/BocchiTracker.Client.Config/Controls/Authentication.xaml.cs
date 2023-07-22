using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BocchiTracker.Client.Config.Controls
{
    public enum AuthenticationType
    {
        None,
        UsernameAndPassword,
        APIKey
    }

    public partial class Authentication : UserControl
    {
        public static readonly DependencyProperty AuthenticationTypeProperty =
            DependencyProperty.Register("AuthType", typeof(AuthenticationType), typeof(Authentication), new PropertyMetadata(AuthTypePropertyChanged));

        public AuthenticationType AuthType
        {
            get { return (AuthenticationType)GetValue(AuthenticationTypeProperty); }
            set { SetValue(AuthenticationTypeProperty, value); }
        }

        public static readonly DependencyProperty UsernameTextProperty =
            DependencyProperty.Register("UsernameText", typeof(string), typeof(Authentication), new PropertyMetadata(null));

        public string UsernameText
        {
            get { return (string)GetValue(UsernameTextProperty); }
            set { SetValue(UsernameTextProperty, value); }
        }

        public static readonly DependencyProperty PasswordTextProperty =
            DependencyProperty.Register("PasswordText", typeof(string), typeof(Authentication), new PropertyMetadata(null));

        public string PasswordText
        {
            get { return (string)GetValue(PasswordTextProperty); }
            set { SetValue(PasswordTextProperty, value); }
        }

        public static readonly DependencyProperty APIKeyTextProperty =
            DependencyProperty.Register("APIKeyText", typeof(string), typeof(Authentication), new PropertyMetadata(null));

        public string APIKeyText
        {
            get { return (string)GetValue(APIKeyTextProperty); }
            set { SetValue(APIKeyTextProperty, value); }
        }

        public Authentication()
        {
            InitializeComponent();

            this.Password.PasswordChanged += (_, __) => OnAPIKeyChanged();
            this.APIKey.PasswordChanged += (_, __) => OnPasswordChanged();
        }

        private static void AuthTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var context = d as Authentication;
            switch(context.AuthType)
            {
                case AuthenticationType.UsernameAndPassword:
                    {
                        context.Input_UserAndPass.Visibility = Visibility.Visible;
                        context.Input_APIKey.Visibility = Visibility.Collapsed;
                    }
                    break;
                case AuthenticationType.APIKey:
                    {
                        context.Input_UserAndPass.Visibility = Visibility.Collapsed;
                        context.Input_APIKey.Visibility = Visibility.Visible;
                    }
                    break;
            }
        }

        private void OnAPIKeyChanged()
        {
            PasswordText = this.Password.Password;
        }

        private void OnPasswordChanged()
        {
            APIKeyText = this.APIKey.Password;
        }
    }
}
