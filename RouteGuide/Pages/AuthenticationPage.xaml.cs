using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Markup;
using RouteGuide.Resources;

using RouteGuide.ViewModels;

namespace RouteGuide.Pages
{
    public partial class AuthenticationPage : PhoneApplicationPage
    {
        private static AuthenticationModel authenticationViewModel = null;

        public AuthenticationPage()
        {
            InitializeComponent();

            // Загрузка данных модели отображения социальных сетей
            DataContext = LoadAuthenticationData();

            BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!authenticationViewModel.IsDataLoaded)
            {
                DataContext = LoadAuthenticationData();
            }
        }

        private object LoadAuthenticationData()
        {
            if (authenticationViewModel == null)
            {
                authenticationViewModel = new AuthenticationModel();
                authenticationViewModel.LoadData();
                return authenticationViewModel;
            }
            else if (!authenticationViewModel.IsDataLoaded)
            {
                authenticationViewModel.LoadData();
                return authenticationViewModel;
            }
            else
            {
                return authenticationViewModel;
            }
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            // Создание кнопок в Application Bar
            ApplicationBarIconButton appBarAcceptButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/Accept.png", UriKind.RelativeOrAbsolute));
            appBarAcceptButton.Text = AppResources.ApplicationBarAccept;
            appBarAcceptButton.Click += appBarAcceptButton_Click;
            ApplicationBar.Buttons.Add(appBarAcceptButton);

            ApplicationBarIconButton appBarCancelButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/Cancel.png", UriKind.RelativeOrAbsolute));
            appBarCancelButton.Text = AppResources.ApplicationBarCancel;
            appBarCancelButton.Click += appBarCancelButton_Click;
            ApplicationBar.Buttons.Add(appBarCancelButton);

            ApplicationBar.IsVisible = false;
        }

        private void SetUserCredentialsPanelVisibility(bool IsVisible, string solcialNetworkName = "")
        {
            if (IsVisible)
            {
                AuthenticationTitle.Visibility = Visibility.Collapsed;
                SocialNetworksList.IsEnabled = !IsVisible;

                if (solcialNetworkName != "")
                {
                    AuthenticationEnterTitle.Text = AppResources.AuthenticationEnterTitle + " " + solcialNetworkName;
                }

                AuthenticationLoginTextBox.Text = string.Empty;
                AuthenticationPassTextBox.Password = string.Empty;
                AuthenticationLoginTextBox.SelectAll();
                UserAccountCredentials.Visibility = Visibility.Visible;
                AuthenticationLoginTextBox.Focus();
                ApplicationBar.IsVisible = IsVisible;
            }
            else
            {
                SocialNetworksList.IsEnabled = !IsVisible;
                AuthenticationTitle.Visibility = Visibility.Visible;
                UserAccountCredentials.Visibility = Visibility.Collapsed;
                ApplicationBar.IsVisible = IsVisible;
            }
        }

        private void SetAuthenticationProgressBarVisibility(bool IsVisible)
        {
            if (UserAccountCredentials.Visibility == Visibility.Visible)
            {
                if (IsVisible)
                {
                    AuthenticationLoginTextBox.IsEnabled = !IsVisible;
                    AuthenticationPassTextBox.IsEnabled = !IsVisible;
                    AuthenticationProgressPanel.Visibility = Visibility.Visible;
                    AuthenticationProgressBar.IsIndeterminate = IsVisible;
                }
                else
                {
                    AuthenticationLoginTextBox.IsEnabled = !IsVisible;
                    AuthenticationPassTextBox.IsEnabled = !IsVisible;
                    AuthenticationProgressPanel.Visibility = Visibility.Collapsed;
                    AuthenticationProgressBar.IsIndeterminate = IsVisible;
                }
            }
        }

        private void CancelAuthentication()
        {
            SetAuthenticationProgressBarVisibility(false);
            SetUserCredentialsPanelVisibility(false);
        }

        void appBarAcceptButton_Click(object sender, EventArgs e)
        {
            if (!AuthenticationLoginTextBox.Text.Equals(string.Empty) && !AuthenticationPassTextBox.Password.Equals(string.Empty))
            {
                ApplicationBar.IsVisible = false;
                SetAuthenticationProgressBarVisibility(true);

                // Отправка данных для аутентификации в социальной сети

                // Пока предполагается только удачная аутентификация
                SetAuthenticationProgressBarVisibility(false);
                SetUserCredentialsPanelVisibility(false);

                NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.RelativeOrAbsolute));
                NavigationService.RemoveBackEntry();
            }
            else
            {
                MessageBox.Show(AppResources.AuthenticationEmptyError);
            }
        }

        void appBarCancelButton_Click(object sender, EventArgs e)
        {
            CancelAuthentication();
        }

        private void SocialNetworksList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;

            if (selector == null)
                return;

            ViewModels.AuthenticationItem item = selector.SelectedItem as ViewModels.AuthenticationItem;

            if (item == null)
                return;

            string socialNetworkName = item.Title;
            SetUserCredentialsPanelVisibility(true, socialNetworkName);

            selector.SelectedItem = null;
        }
    }
}