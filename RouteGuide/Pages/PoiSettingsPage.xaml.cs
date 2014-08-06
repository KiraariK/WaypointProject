using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RouteGuide.Resources;
using RouteGuide.Classes;

namespace RouteGuide.Pages
{
    public partial class PoiSettingsPage : PhoneApplicationPage
    {
        private MarkerKind selectedPoiKind;
        private string selectedPoiName;
        private string selectedPoiDescription;
        private string selectedPoiComment;
        private double selectedPoiMarkerLatitude;
        private double selectedPoiMarkerLongitude;

        public PoiSettingsPage()
        {
            InitializeComponent();

            CreateApplicationBar();

            CreateDefaultPoi();
        }

        private void CreateApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton appBarApplyButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/Accept.png", UriKind.RelativeOrAbsolute));
            appBarApplyButton.Text = AppResources.ApplicationBarAccept;
            appBarApplyButton.Click += appBarApplyButton_Click;
            ApplicationBar.Buttons.Add(appBarApplyButton);

            ApplicationBarIconButton appBarCancelButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/Cancel.png", UriKind.RelativeOrAbsolute));
            appBarCancelButton.Text = AppResources.ApplicationBarCancel;
            appBarCancelButton.Click += appBarCancelButton_Click;
            ApplicationBar.Buttons.Add(appBarCancelButton);
        }

        private void CreateDefaultPoi()
        {
            // если нету сохраненной выбранной в прошлый раз POI, то выставляем по умолчанию
            // тестовый вариант!!!
            selectedPoiKind = MarkerKind.Police;
            selectedPoiName = AppResources.PoiPoliceName;
            selectedPoiDescription = AppResources.PoiPoliceDescription;
            selectedPoiComment = string.Empty;
            selectedPoiMarkerLatitude = double.NaN;
            selectedPoiMarkerLongitude = double.NaN;
        }

        /*
         * События кнопок в Application Bar
         */
        void appBarApplyButton_Click(object sender, EventArgs e)
        {
            SavePoiSettings();
            NavigationService.GoBack();
        }

        void appBarCancelButton_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        /*
         * функция, формирующая строку параметров, записываемую в URI при переходе на MainPage
         */
        private void SavePoiSettings()
        {
            PhoneApplicationService.Current.State["type"] = "PoiSettings";

            if (PoiSettingsMyLocationRadioButton.IsChecked == true)
            {
                // текущее местоположение пользователя (координаты будут определены на MainPage)
                PhoneApplicationService.Current.State["location"] = "me";
            }
            else if (PoiSettingsSearchMarkerRadioButton.IsChecked == true)
            {
                // маркер поиска на карте (координаты будут определены на MainPage)
                PhoneApplicationService.Current.State["location"] = "marker";
            }
            else
            {
                // координаты будут записаны в состояние приложения для того, чтобы их можно было извлечь на MainPage
                PhoneApplicationService.Current.State["location"] = "search";
                // !!! Только в качестве заглушки !!!
                selectedPoiMarkerLatitude = 56.4632;
                selectedPoiMarkerLongitude = 84.9726;
                PhoneApplicationService.Current.State["latitude"] = selectedPoiMarkerLatitude;
                PhoneApplicationService.Current.State["longitude"] = selectedPoiMarkerLongitude;
            }

            PhoneApplicationService.Current.State["markerKind"] = (int)selectedPoiKind;

            PhoneApplicationService.Current.State["poiName"] = selectedPoiName;
            PhoneApplicationService.Current.State["poiDescription"] = selectedPoiDescription;            

            PhoneApplicationService.Current.State["poiComment"] = selectedPoiComment;

            // Все остальное (пользователь, время установки и прочее) можно узнать на MainPage
        }

        /*
         * События выбора в качестве локации размещения POI текущего местоположения пользователя
         */
        private void PoiSettingsSelectMyLocationButton_Click(object sender, RoutedEventArgs e)
        {
            PoiSettingsMyLocationRadioButton.IsChecked = true;
            PoiSettingsSearchMarkerRadioButton.IsChecked = false;
            PoiSettingsSearchRadioButton.IsChecked = false;
        }

        /*
         * События выбора в качестве лдокации размещения POI отмеченного на карте маркером поиска объекта
         */
        private void PoiSettingsSelectSearchMarkerButton_Click(object sender, RoutedEventArgs e)
        {
            PoiSettingsMyLocationRadioButton.IsChecked = false;
            PoiSettingsSearchMarkerRadioButton.IsChecked = true;
            PoiSettingsSearchRadioButton.IsChecked = false;
        }

        /*
         * События выбора в качестве локации размещения POI места, которое будет найдено с помощью поиска
         */
        private void PoiSettingsSelectSearchLocation_Click(object sender, RoutedEventArgs e)
        {
            PoiSettingsMyLocationRadioButton.IsChecked = false;
            PoiSettingsSearchMarkerRadioButton.IsChecked = false;
            PoiSettingsSearchRadioButton.IsChecked = true;
        }

        /*
         * События страницы
         */
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
    }
}