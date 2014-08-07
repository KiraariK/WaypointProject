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
        /*
         * Замечание - все объекта на слое карты наносятся поверх других объектов (в порядке их добавление на слой карты)
         * Возможно, стоит пересмотреть порядок слоев карты в будущем.
         */

        // Текущее местоположение пользователя (слой карты = 0)
        private MyLocationMarker myMapMarker = null;

        // Список маркеров POI (слой карты = 1)
        private List<POIMarker> poiMapMarkers = null;
        // Радиус от пользователя в котором нужно загрузить POI (метры)
        private const double poiRadius = 1000.0;

        // Список особых меток на карте (слой карты = 2)
        private List<Marker> specialMapMarker = null;

        // Список маркеров поиска (слой карты = 3)
        private List<SearchMarker> searchMapMarker = null;

        // Список маркеров промежуточных точек маршрутов (слой карты = 4)
        private List<Marker> waypointMapMarkers = null;

        // Конструктор
        public MainPage()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;

            CreateMainApplicationBar();
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (SystemTray.ProgressIndicator == null)
                SystemTray.ProgressIndicator = new ProgressIndicator();

            LoadMapMarkers();
        }

        /*
         * функция создания нового экземпляра основного объекта Application Bar.
         * функция полностью заполняет основной Application Bar (определяет кнопки и пункты меню).
         */
        private void CreateMainApplicationBar()
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
         * Функция создает слои для отрисовки маркеров на карте,
         * загружает маркеры:
         * текущее положение пользователя + фокусировка с масштабированием на нем,
         * особые маркеры, установленные пользователем в настройках приложения,
         * маркеры POI, информация о которых грузится с сервера
         */
        private async void LoadMapMarkers()
        {
            RouteGuideMap.Layers.Clear();

            MapLayer meMapLayer = new MapLayer();
            GetMyCurrentStartPosition(meMapLayer);
            RouteGuideMap.Layers.Add(meMapLayer);

            MapLayer poiMapLayer = new MapLayer();
            // получить данные о POI, записть в слой карты
            RouteGuideMap.Layers.Add(poiMapLayer);

            MapLayer specialMapLayer = new MapLayer();
            // получить данные о специальных маркерах, записать в слой карты
            RouteGuideMap.Layers.Add(specialMapLayer);

            MapLayer searchMapLayer = new MapLayer();
            RouteGuideMap.Layers.Add(searchMapLayer);

            MapLayer waypointMapLayer = new MapLayer();
            RouteGuideMap.Layers.Add(waypointMapLayer);
        }

        /*
         * функция асинхронного определения текущего местоположения,
         * фокусировка карты на текущем местоположении c определенным уровнем приближения,
         * установка метки текущего местоположения на карте. (вызывается при загрузке карты)
         */
        private async void GetMyCurrentStartPosition(MapLayer mapLayer)
        {
            SetProgressIndicatiorVisibility(true, AppResources.ProgressIndicatorCurrentLocationText);
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracy = PositionAccuracy.High;

            myMapMarker = MyLocationMarker.Instance();
            myMapMarker.Kind = MarkerKind.Me;

            try
            {
                Geoposition myPosition = await geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(20));
                myMapMarker.Accuracy = myPosition.Coordinate.Accuracy;

                myMapMarker.Coordinate = new GeoCoordinate(myPosition.Coordinate.Latitude, myPosition.Coordinate.Longitude);
                RouteGuideMap.SetView(myMapMarker.Coordinate, 16, MapAnimationKind.Parabolic);
                DrawMyPositionMarker(mapLayer);
            }
            catch (Exception)
            {
                MessageBox.Show(AppResources.GetMyCurrentLocationError);
            }

            SetProgressIndicatiorVisibility(false);
        }

        /*
         * функция асинхронного обновления текущего местоположения пользователя,
         * перерисовывает точку текущего местоположения и фокусируется на ней, без изменения масштаба
         */
        private async void UpdateMyCurrentPosition(MapLayer mapLayer)
        {
            SetProgressIndicatiorVisibility(true, AppResources.ProgressIndicatorCurrentLocationText);
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracy = PositionAccuracy.High;

            myMapMarker = MyLocationMarker.Instance();
            myMapMarker.Kind = MarkerKind.Me;

            try
            {
                                                                                // устаревание инфы     // таймаут определения местоположения
                Geoposition myPosition = await geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(20));
                myMapMarker.Accuracy = myPosition.Coordinate.Accuracy;

                myMapMarker.Coordinate = new GeoCoordinate(myPosition.Coordinate.Latitude, myPosition.Coordinate.Longitude);
                RouteGuideMap.SetView(myMapMarker.Coordinate, RouteGuideMap.ZoomLevel, MapAnimationKind.Parabolic);
                DrawMyPositionMarker(mapLayer);
            }
            catch (Exception)
            {
                MessageBox.Show(AppResources.GetMyCurrentLocationError);
            }

            SetProgressIndicatiorVisibility(false);
        }

        /*
         * функция отрисовки маркера текущей позиции пользователя на соответствующем слое карты + 
         * отрисовка окружности погрешности определения текущей позиции пользователя
         */
        private void DrawMyPositionMarker(MapLayer mapLayer)
        {
            if (myMapMarker != null)
            {
                mapLayer.Clear();

                DrawMyLocationAccuracyRadius(mapLayer);
                DrawMapMarker(myMapMarker.Coordinate, myMapMarker.Kind, mapLayer);
            }
        }

        /*
         * Фунцкция отрисовывает радиус точности опредления текущего местположения пользователя
         * на слое карты.
         */
        private void DrawMyLocationAccuracyRadius(MapLayer mapLayer)
        {
            /* метод вычислений взят чисто из примера от Nokia для Windows Phone 8 Map API */
            // The ground resolution (in meters per pixel) varies depending on the level of detail 
            // and the latitude at which it’s measured. It can be calculated as follows:
            double metersPerPixels = (Math.Cos(myMapMarker.Coordinate.Latitude * Math.PI / 180) * 2 * Math.PI * 6378137) / (256 * Math.Pow(2, RouteGuideMap.ZoomLevel));
            double radius = myMapMarker.Accuracy / metersPerPixels;

            Ellipse locationArea = new Ellipse();
            locationArea.Width = radius * 2;
            locationArea.Height = radius * 2;
            Color circleColor = ((Color)Application.Current.Resources["PhoneAccentColor"]);
            locationArea.Fill = new SolidColorBrush(Color.FromArgb(60, circleColor.R, circleColor.G, circleColor.B));

            MapOverlay overlay = new MapOverlay();
            overlay.Content = locationArea;
            overlay.GeoCoordinate = myMapMarker.Coordinate;
            overlay.PositionOrigin = new Point(0.5, 0.5);
            mapLayer.Add(overlay);
        }

        /*
         * функция обновления радиуса точности определения текущего местположения пользователя
         */
        private void UpdateLocationAccuracyRadius(MapOverlay overlay)
        {
            /* метод вычислений взят чисто из примера от Nokia для Windows Phone 8 Map API */
            // The ground resolution (in meters per pixel) varies depending on the level of detail 
            // and the latitude at which it’s measured. It can be calculated as follows:
            double metersPerPixels = (Math.Cos(myMapMarker.Coordinate.Latitude * Math.PI / 180) * 2 * Math.PI * 6378137) / (256 * Math.Pow(2, RouteGuideMap.ZoomLevel));
            double radius = myMapMarker.Accuracy / metersPerPixels;

            Ellipse locationArea = new Ellipse();
            locationArea.Width = radius * 2;
            locationArea.Height = radius * 2;
            Color circleColor = ((Color)Application.Current.Resources["PhoneAccentColor"]);
            locationArea.Fill = new SolidColorBrush(Color.FromArgb(60, circleColor.R, circleColor.G, circleColor.B));

            overlay.Content = locationArea;
            overlay.GeoCoordinate = myMapMarker.Coordinate;
            overlay.PositionOrigin = new Point(0.5, 0.5);
        }

        /*
         * Функция для загрузки с сервера данных о POI и отображения их на слое карты
         */
        private async void LoadPoiMarkers(MapLayer mapLayer)
        {
            // отрисовывание с помощью DrawMapMarker()
        }

        /*
         * Функция для загрузки особых маркеров из памяти и отображения их на слое карты
         */
        private async void LoadSpecialMarkers(MapLayer mapLayer)
        {
            // отрисовывание с помощью DrawMapMarker()
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
            switch (markerKind)
            {
                case MarkerKind.Me:
                    //marker.Tag = new GeoCoordinate(myMapMarker.Coordinate.Latitude, myMapMarker.Coordinate.Longitude);
                    marker.Tag = "Я здесь";
                    break;
                case MarkerKind.Home:
                    marker.Tag = "Мой дом";
                    break;
                case MarkerKind.Work:
                    marker.Tag = "Моя работа";
                    break;
                case MarkerKind.Search:
                    // запихиваем структуру для маркера поиска
                    break;
                case MarkerKind.Location:
                    break;
                case MarkerKind.Destination:
                    break;
                case MarkerKind.Waypoint:
                    break;
                default:
                    // загружаем структуру для макера POI
                    break;

            }
            marker.Tap += marker_Tap;

            // создание объекта маркера на слое карты
            MapOverlay overlay = new MapOverlay();
            overlay.Content = marker;
            overlay.GeoCoordinate = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            overlay.PositionOrigin = new Point(0.5, 1.0);
            mapLayer.Add(overlay);
        }

        void marker_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image marker = sender as Image;

            if (marker == null)
                return;

            string message = marker.Tag as string;

            if (message == null)
            {
                GeoCoordinate coordinate = marker.Tag as GeoCoordinate;

                if (coordinate == null)
                    return;

                MessageBox.Show(string.Format("{0} - {1}", coordinate.Latitude.ToString(), coordinate.Longitude.ToString()));

                return;
            }

            MessageBox.Show(message);
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

        /*
         * События кнопок, расположенных над Application Bar
         */
        private void CreateRouteButton_Click(object sender, RoutedEventArgs e)
        {

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

        private void CreatePoiButton_Click(object sender, RoutedEventArgs e)
        {
            // подготовка данных для передачи на страницу настроек POI

            NavigationService.Navigate(new Uri("/Pages/PoiSettingsPage.xaml", UriKind.RelativeOrAbsolute));
        }

        /*
         * События нажатия кнопок в Application Bar
         */
        void appBarSearchButton_Click(object sender, EventArgs e)
        {
            SetSearchTextBoxVisibility(true);
        }

        void appBarLocationButton_Click(object sender, EventArgs e)
        {
            // определяем текущую позицию и перерисовываем её на соответствущем слое карты
            UpdateMyCurrentPosition(RouteGuideMap.Layers[0]);
        }

        void appBarFavoriteButton_Click(object sender, EventArgs e)
        {

        }

        void appBarLayersButton_Click(object sender, EventArgs e)
        {

        }


        /*
         * События выбора одного из пунктов меню в Application Bar
         */
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

        /*
         * События карты
         */
        private void RouteGuideMap_ZoomLevelChanged(object sender, MapZoomLevelChangedEventArgs e)
        {
            // перерисовываем изображение окружности погрешности определения местоположения пользователя
            foreach (MapOverlay overlay in RouteGuideMap.Layers[0])
            {
                Ellipse accuracyEllipce = overlay.Content as Ellipse;
                if (accuracyEllipce != null)
                {
                    UpdateLocationAccuracyRadius(overlay);
                }
            }
        }

        /*
         * События страницы
         */
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // обрабатываем передачу данных со страницы добавления POI
            if (PhoneApplicationService.Current.State.ContainsKey("type"))
            {
                // предыдущая страница передала данные
                string type = PhoneApplicationService.Current.State["type"] as string;

                if (type == null)
                {
                    // удаляем пару с ключом type из состояния приложения
                    PhoneApplicationService.Current.State.Remove("type");
                    return;
                }

                if (type.Equals("PoiSettings"))
                {
                    // данные передала страница натроек POI
                    POIMarker newPoiMarker = new POIMarker();

                    string location = PhoneApplicationService.Current.State["location"] as string;

                    if (location == null)
                        return;

                    if (location.Equals("me"))
                    {
                        // записываем в качестве места текущее местоположение пользователя
                        newPoiMarker.Coordinate = myMapMarker.Coordinate;
                    }
                    else if (location.Equals("marker"))
                    {
                        // записываем в качестве места координаты ближайшего маркера поиска

                    }
                    else
                    {
                        // считываем переданные координаты
                        double latitude = (double)PhoneApplicationService.Current.State["latitude"];
                        double longitude = (double)PhoneApplicationService.Current.State["longitude"];
                        newPoiMarker.Coordinate = new GeoCoordinate(latitude, longitude);
                        // удаляем из состояния приложения
                        PhoneApplicationService.Current.State.Remove("latitude");
                        PhoneApplicationService.Current.State.Remove("longitude");
                    }
                    // удаляем пару с ключом location из состояния прилоежния
                    PhoneApplicationService.Current.State.Remove("location");

                    // тип маркера
                    newPoiMarker.Kind = (MarkerKind)PhoneApplicationService.Current.State["markerKind"];
                    // удаляем из состояния приложения
                    PhoneApplicationService.Current.State.Remove("markerKind");

                    // получаем название и описание POI
                    newPoiMarker.Name = (string)PhoneApplicationService.Current.State["poiName"];
                    newPoiMarker.Description = (string)PhoneApplicationService.Current.State["poiDescription"];
                    // удаляем из состояния приложения
                    PhoneApplicationService.Current.State.Remove("poiName");
                    PhoneApplicationService.Current.State.Remove("poiDescription");

                    // получаем комментарий пользователя
                    newPoiMarker.Comment = (string)PhoneApplicationService.Current.State["poiComment"];
                    // удаляем из состояния приложения
                    PhoneApplicationService.Current.State.Remove("poiComment");

                    // добавляем прочие настройки к POI: параетры пользователя, например.
                }
                // удаляем пару с ключом type из состояния приложения
                PhoneApplicationService.Current.State.Remove("type");
            }
        }
    }
}