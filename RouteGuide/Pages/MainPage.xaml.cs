using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Device.Location;
using Microsoft.Phone.Maps.Controls;
using RouteGuide.Resources;

namespace RouteGuide
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Конструктор
        public MainPage()
        {
            InitializeComponent();

            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            // Создание кнопок в Application Bar
            ApplicationBarIconButton appBarSearchButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/Search.png", UriKind.RelativeOrAbsolute));
            appBarSearchButton.Text = AppResources.MainPageAppBarSearchText;
            appBarSearchButton.Click += appBarSearchButton_Click;
            ApplicationBar.Buttons.Add(appBarSearchButton);

            ApplicationBarIconButton appBarLocationButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/Current_location.png", UriKind.RelativeOrAbsolute));
            appBarLocationButton.Text = AppResources.MainPageAppBarLocationText;
            appBarLocationButton.Click += appBarLocationButton_Click;
            ApplicationBar.Buttons.Add(appBarLocationButton);

            ApplicationBarIconButton appBarFavoriteButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/Favorite.png", UriKind.RelativeOrAbsolute));
            appBarFavoriteButton.Text = AppResources.MainPageAppBarFavoriteText;
            appBarFavoriteButton.Click += appBarFavoriteButton_Click;
            ApplicationBar.Buttons.Add(appBarFavoriteButton);

            ApplicationBarIconButton appBarLayersButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/Square.png", UriKind.RelativeOrAbsolute));
            appBarLayersButton.Text = AppResources.MainPageAppBarLayersText;
            appBarLayersButton.Click += appBarLayersButton_Click;
            ApplicationBar.Buttons.Add(appBarLayersButton);

            // Содание пунктов меню в Application Bar
            ApplicationBarMenuItem appBarMenuAccount = new ApplicationBarMenuItem(AppResources.MainPageMenuAccount);
            appBarMenuAccount.Click += appBarMenuAccount_Click;
            ApplicationBar.MenuItems.Add(appBarMenuAccount);

            ApplicationBarMenuItem appBarMenuPoi = new ApplicationBarMenuItem(AppResources.MainPageMenuPoi);
            appBarMenuPoi.Click += appBarMenuPoi_Click;
            ApplicationBar.MenuItems.Add(appBarMenuPoi);

            ApplicationBarMenuItem appBarMenuTracks = new ApplicationBarMenuItem(AppResources.MainPageMenuTracks);
            appBarMenuTracks.Click += appBarMenuTracks_Click;
            ApplicationBar.MenuItems.Add(appBarMenuTracks);

            ApplicationBarMenuItem appBarMenuFavorite = new ApplicationBarMenuItem(AppResources.MainPageMenuFavorite);
            appBarMenuFavorite.Click += appBarMenuFavorite_Click;
            ApplicationBar.MenuItems.Add(appBarMenuFavorite);

            ApplicationBarMenuItem appBarMenuPlaces = new ApplicationBarMenuItem(AppResources.MainPageMenuPlaces);
            appBarMenuPlaces.Click += appBarMenuPlaces_Click;
            ApplicationBar.MenuItems.Add(appBarMenuPlaces);

            ApplicationBarMenuItem appBarMenuHistory = new ApplicationBarMenuItem(AppResources.MainPageMenuHistory);
            appBarMenuHistory.Click += appBarMenuHistory_Click;
            ApplicationBar.MenuItems.Add(appBarMenuHistory);

            ApplicationBarMenuItem appBarMenuSettings = new ApplicationBarMenuItem(AppResources.MainPageMenuSettings);
            appBarMenuSettings.Click += appBarMenuSettings_Click;
            ApplicationBar.MenuItems.Add(appBarMenuSettings);

            ApplicationBarMenuItem appBarMenuRespect = new ApplicationBarMenuItem(AppResources.MainPageMenuRespect);
            appBarMenuRespect.Click += appBarMenuRespect_Click;
            ApplicationBar.MenuItems.Add(appBarMenuRespect);

            ApplicationBarMenuItem appBarMenuAbout = new ApplicationBarMenuItem(AppResources.MainPageMenuAbout);
            appBarMenuAbout.Click += appBarMenuAbout_Click;
            ApplicationBar.MenuItems.Add(appBarMenuAbout);
        }

        private void SetSearchTextBoxVisibility(bool isVisible)
        {
            if (isVisible)
            {
                RouteGuideMap.IsEnabled = !isVisible;

                SearchTextBox.SelectAll();
                SearchTextBox.Visibility = Visibility.Visible;
                SearchTextBox.Focus();
            }
            else
            {
                RouteGuideMap.IsEnabled = !isVisible;

                SearchTextBox.Visibility = Visibility.Collapsed;
            }
        }

        private void CloserButton_Click(object sender, RoutedEventArgs e)
        {
            if (RouteGuideMap.ZoomLevel != 20) // Самый крупный масштаб
            {
                if (RouteGuideMap.ZoomLevel > 14) // Близко к земле - приближаем помедленее
                {
                    double newZoomLevel = RouteGuideMap.ZoomLevel + 1;
                    RouteGuideMap.SetView(RouteGuideMap.Center, newZoomLevel, MapAnimationKind.Linear);
                }
                else // Далеко от земли - приближем побыстрее
                {
                    double newZoomLevel = RouteGuideMap.ZoomLevel + 1.5;
                    RouteGuideMap.SetView(RouteGuideMap.Center, newZoomLevel, MapAnimationKind.Linear);
                }
            }
        }

        private void FurtherButton_Click(object sender, RoutedEventArgs e)
        {
            if (RouteGuideMap.ZoomLevel != 1) // Самый мелкий масштаб
            {
                if (RouteGuideMap.ZoomLevel <= 14) // Далеко от земли - отдаляем побыстрее
                {
                    double newZoomLevel = RouteGuideMap.ZoomLevel - 1.5;
                    RouteGuideMap.SetView(RouteGuideMap.Center, newZoomLevel, MapAnimationKind.Linear);
                }
                else // Близко к земле - отдаляем помедленее
                {
                    double newZoomLevel = RouteGuideMap.ZoomLevel - 1;
                    RouteGuideMap.SetView(RouteGuideMap.Center, newZoomLevel,  MapAnimationKind.Linear);
                }
            }
        }

        void appBarSearchButton_Click(object sender, EventArgs e)
        {
            SetSearchTextBoxVisibility(true);
        }

        void appBarLocationButton_Click(object sender, EventArgs e)
        {

        }

        void appBarFavoriteButton_Click(object sender, EventArgs e)
        {

        }

        void appBarLayersButton_Click(object sender, EventArgs e)
        {

        }



        void appBarMenuAccount_Click(object sender, EventArgs e)
        {

        }

        void appBarMenuPoi_Click(object sender, EventArgs e)
        {

        }

        void appBarMenuTracks_Click(object sender, EventArgs e)
        {

        }

        void appBarMenuFavorite_Click(object sender, EventArgs e)
        {

        }

        void appBarMenuPlaces_Click(object sender, EventArgs e)
        {

        }

        void appBarMenuHistory_Click(object sender, EventArgs e)
        {

        }

        void appBarMenuSettings_Click(object sender, EventArgs e)
        {

        }

        void appBarMenuRespect_Click(object sender, EventArgs e)
        {

        }

        void appBarMenuAbout_Click(object sender, EventArgs e)
        {

        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SetSearchTextBoxVisibility(false);
        }
    }
}