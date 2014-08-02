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
using Windows.Devices.Geolocation;

using RouteGuide.Classes;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media;

namespace RouteGuide
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Конструктор
        public MainPage()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;

            BuildLocalizedApplicationBar();
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            SystemTray.ProgressIndicator = new ProgressIndicator();

            GetMyCurrentLocation();
        }

        // Текущее местоположение пользователя
        private GeoCoordinate myCoordinate = null;

        // Точность определения местположения пользователя в метрах
        private double myLocationAccuracy = double.NaN;

        /*
         * функция создания нового экземпляра основного объекта Application Bar.
         * функция полностью заполняет основной Application Bar (определяет кнопки и пункты меню).
         */
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

        /*
         * функция асинхронного определения текущего местоположения,
         * фокусировка карты на текущем местоположении,
         * установка метки текущего местоположения на карте.
         */
        private async void GetMyCurrentLocation()
        {
            SetProgressIndicatiorVisibility(true, AppResources.ProgressIndicatorCurrentLocationText);
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracy = PositionAccuracy.High;

            try
            {
                Geoposition myPosition = await geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(20));
                myLocationAccuracy = myPosition.Coordinate.Accuracy;

                myCoordinate = new GeoCoordinate(myPosition.Coordinate.Latitude, myPosition.Coordinate.Longitude);
                DrawMapMarkers();
                RouteGuideMap.SetView(myCoordinate, 16, MapAnimationKind.Parabolic);
                
            }
            catch (Exception)
            {
                MessageBox.Show(AppResources.GetMyCurrentLocationError);
            }

            SetProgressIndicatiorVisibility(false);
        }

        /*
         * функция отрисовывает метки на карте, создавая слой для их отрисовки на карте.
         * Старый слой удаляется.
         */
        private void DrawMapMarkers(double radius = double.NaN)
        {
            // TODO: разделить отрисовку различных маркеров на карте
            RouteGuideMap.Layers.Clear();
            MapLayer mapLayer = new MapLayer();

            // отрисовка маркеров особых мест
            // TODO:

            // отрисовка маркера для местоположения пользователя
            if (myCoordinate != null)
            {
                DrawMyLocationAccuracyRadius(mapLayer);
                DrawMapMarker(myCoordinate, MarkerKind.Me, mapLayer);
            }

            // Отрисовкамаркера поиска
            // TODO:

            // Отрисовка всех POI в установленном радиусе
            // TODO:

            RouteGuideMap.Layers.Add(mapLayer);
        }

        /*
         * функция загружает нужную иконку маркера,
         * создает объект маркера и добавляет его на слой карты.
         */
        private void DrawMapMarker(GeoCoordinate coordinate, MarkerKind markerKind, MapLayer mapLayer)
        {
            // создание маркера
            Image marker = new Image();
            BitmapImage markerIcon = new BitmapImage(new Uri(MarkerStore.GetMarkerPath(markerKind), UriKind.RelativeOrAbsolute));
            marker.Source = markerIcon;

            // позволяем взаимодействие с маркером
            marker.Tag = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude); //!!! записываем конкретную нужную информацию о маркере
            marker.Tap += marker_Tap;

            // создание объекта маркера на слое карты
            MapOverlay overlay = new MapOverlay();
            overlay.Content = marker;
            overlay.GeoCoordinate = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            overlay.PositionOrigin = new Point(0.0D, 1.0D);
            mapLayer.Add(overlay);
        }

        void marker_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image marker = sender as Image;

            if (marker == null)
                return;

            GeoCoordinate coordinate = marker.Tag as GeoCoordinate;

            if (coordinate == null)
                return;

            MessageBox.Show(string.Format("{0} -{1}", coordinate.Latitude.ToString(), coordinate.Longitude.ToString()));
        }

        /*
         * фунцкция отрисовывает радиус точности опредления текущего местположения пользователя
         * на слое карты
         */
        private void DrawMyLocationAccuracyRadius(MapLayer mapLayer)
        {
            /* метод вычислений взят чисто из примера от Nokia для Windows Phone 8 Map API */
            // The ground resolution (in meters per pixel) varies depending on the level of detail 
            // and the latitude at which it’s measured. It can be calculated as follows:
            double metersPerPixels = (Math.Cos(myCoordinate.Latitude * Math.PI / 180) * 2 * Math.PI * 6378137) / (256 * Math.Pow(2, RouteGuideMap.ZoomLevel));
            double radius = myLocationAccuracy / metersPerPixels;

            Ellipse locationArea = new Ellipse();
            locationArea.Width = radius * 2;
            locationArea.Height = radius * 2;
            locationArea.Fill = new SolidColorBrush(Color.FromArgb(75, 68, 224, 163));

            MapOverlay overlay = new MapOverlay();
            overlay.Content = locationArea;
            overlay.GeoCoordinate = new GeoCoordinate(myCoordinate.Latitude, myCoordinate.Longitude);
            overlay.PositionOrigin = new Point(0.5, 0.5);
            mapLayer.Add(overlay);
        }

        /*
         * функция устанавливает видимость полосы загрузки с поясняющим текстом в трее телефона.
         */
        private void SetProgressIndicatiorVisibility(bool IsVisible, string Text = "")
        {
            SystemTray.ProgressIndicator.IsIndeterminate = IsVisible;
            SystemTray.ProgressIndicator.IsVisible = IsVisible;

            if (!Text.Equals(""))
                SystemTray.ProgressIndicator.Text = Text;
        }

        /*
         * функция устанавливает видимость элементов кправления на экране при использовании кнопки поиска
         * на основном Application Bar.
         */
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
                    if (newZoomLevel <= 20)
                        RouteGuideMap.SetView(RouteGuideMap.Center, newZoomLevel, MapAnimationKind.Linear);
                    else
                        RouteGuideMap.SetView(RouteGuideMap.Center, 20, MapAnimationKind.Linear);
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
                    if (newZoomLevel >= 1)
                        RouteGuideMap.SetView(RouteGuideMap.Center, newZoomLevel, MapAnimationKind.Linear);
                    else
                        RouteGuideMap.SetView(RouteGuideMap.Center, 1, MapAnimationKind.Linear);
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