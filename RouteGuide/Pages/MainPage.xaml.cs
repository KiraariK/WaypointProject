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
using Microsoft.Phone.Maps.Services;
using RouteGuide.Resources;
using Windows.Devices.Geolocation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Threading;

using RouteGuide.Classes;
using RouteGuide.ViewModels;

namespace RouteGuide
{
    public partial class MainPage : PhoneApplicationPage
    {
        /*
         * Замечание - все объекта на слое карты наносятся поверх других объектов (в порядке их добавление на слой карты).
         * А также каждый слой наносится поверх другого слоя от 0 и далее до самого верхнего.
         */
        /*
         * Реализация маркеров: отображение отделено от логики. Отображение (картинка маркера на карте)
         * содержит только ссылку (индекс списка) на информацию о маркере - списки маркеров, представленные ниже.
         */

        // Список особых меток на карте (слой карты = 0)
        private List<Marker> specialMapMarkers = null;

        // Текущее местоположение пользователя (слой карты = 1)
        private MyLocationMarker myMapMarker = null;

        // Список маркеров POI (слой карты = 2)
        private List<POIMarker> poiMapMarkers = null;
        // Радиус от пользователя в котором нужно загрузить POI (метры)
        private const double poiRadius = 1000.0;

        // Список маркеров поиска (слой карты = 3)
        private List<SearchMarker> searchMapMarkers = null;

        // Список маркеров промежуточных точек маршрутов (слой карты = 4)
        private List<Marker> waypointMapMarkers = null;


        // Объект, с помощью которого производится запрос на поиск на карте
        private GeocodeQuery searchQuery = null;

        // Флаг, показывающий, происходит ли поиск маршрута
        private bool _isRouteSearch = false;

        // Флаг, показывающий, хочет ли пользователь увидеть все найденные результаты
        private bool _isSearchAll = false;

        // Поле, хранящей последнюю введенную пользователем фразу поиска
        private string lastSearchPhrase = string.Empty;

        // Таймер, отсчитывающий время после последнего ввода информации в поле поиска
        private DispatcherTimer searchLatencyTimer = null;

        private static SearchResultsModel searchresultViewModel = null;

        // Конструктор
        public MainPage()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;

            CreateSearchTimer();

            CreateMainApplicationBar();
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (SystemTray.ProgressIndicator == null)
                SystemTray.ProgressIndicator = new ProgressIndicator();

            LoadMapMarkers();
        }

        private void CreateSearchTimer()
        {
            searchLatencyTimer = new DispatcherTimer();
            searchLatencyTimer.Interval = new TimeSpan(0, 0, 0, 1, 300);
            searchLatencyTimer.Tick += searchLatencyTimer_Tick;
        }

        /*
         * функция производит загрузку данных модели для отображения результатов поиска.
         * функция использует данные списка маркеров поиска.
         */
        private object LoadSearchResultData()
        {
            if (searchMapMarkers == null)
                return null;

            if (searchMapMarkers.Count == 0)
                return null;

            if (searchresultViewModel == null)
                searchresultViewModel = new SearchResultsModel();

            List<string> results = new List<string>();
            for (int i = 0; i < searchMapMarkers.Count; i++)
            {
                results.Add(searchMapMarkers[i].Address.Street + " " +
                    searchMapMarkers[i].Address.HouseNumber + " " +
                    searchMapMarkers[i].Address.City + " " +
                    searchMapMarkers[i].Address.County);
            }

            searchresultViewModel = new SearchResultsModel();
            searchresultViewModel.LoadData(results);
            return searchresultViewModel;
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
         * функция создания нового Apply-Cancel Application Bar.
         * функция полностью заполняет Application Bar (определяет кнопки и пункты меню).
         */
        private void CreateDialogApplicationBar()
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

            MapLayer specialMapLayer = new MapLayer();
            // получить данные о специальных маркерах, записать в слой карты
            RouteGuideMap.Layers.Add(specialMapLayer);

            MapLayer meMapLayer = new MapLayer();
            GetMyCurrentStartPosition(meMapLayer);
            RouteGuideMap.Layers.Add(meMapLayer);

            MapLayer poiMapLayer = new MapLayer();
            // получить данные о POI, записть в слой карты
            RouteGuideMap.Layers.Add(poiMapLayer);

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

            try
            {
                                                                                // устаревание инфы     // таймаут определения местоположения
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
        private void UpdateMyLocationAccuracyRadius(MapOverlay overlay)
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
         * Аргумент index используется для ссылки на объект маркера из списка.
         */
        private void DrawMapMarker(GeoCoordinate coordinate, MarkerKind markerKind, MapLayer mapLayer, int index = 0)
        {
            // создание маркера
            Image marker = new Image();
            BitmapImage markerIcon = new BitmapImage(new Uri(MarkerStore.GetMarkerPath(markerKind), UriKind.RelativeOrAbsolute));
            marker.Source = markerIcon;

            // позволяем взаимодействие с маркером
            switch (markerKind)
            {
                    /* 
                     * В совйство Tag записывается строка, состоящая из 2-х частей, разделенных символом '_'.
                     * В первой части записываеся тип маркера - число перечисления MarkerKind,
                     * а во второй - информация о маркере (обычно ссылка на элемент соответствующего списка)
                     */
                case MarkerKind.Me:
                    marker.Tag = (int)markerKind + "_" + "Я здесь";
                    break;
                case MarkerKind.Home:
                    marker.Tag = (int)markerKind + "_" + "Здесь мой дом";
                    break;
                case MarkerKind.Work:
                    marker.Tag = (int)markerKind + "_" + "Здесь моя работа";
                    break;
                case MarkerKind.Search:
                    marker.Tag = (int)markerKind + "_" + index;
                    break;
                case MarkerKind.Location:
                    break;
                case MarkerKind.Destination:
                    break;
                case MarkerKind.Waypoint:
                    break;
                default: // подразумевается POI
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
                return;

            string[] parts = message.Split('_');
            MarkerKind kind = (MarkerKind)int.Parse(parts[0]);
            switch(kind)
            {
                case MarkerKind.Me:
                    MessageBox.Show(parts[1]);
                    break;
                case MarkerKind.Home:
                    MessageBox.Show(parts[1]);
                    break;
                case MarkerKind.Work:
                    MessageBox.Show(parts[1]);
                    break;
                case MarkerKind.Search:
                    MessageBox.Show("Индекс точки: " + parts[1]);
                    break;
                case MarkerKind.Location:
                    break;
                case MarkerKind.Destination:
                    break;
                case MarkerKind.Waypoint:
                    break;
                default: // подразумевается POI
                    // загружаем структуру для макера POI
                    break;
            }
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
         * функция устанавливает видимость элементов управления на экране при использовании кнопки поиска
         * на основном Application Bar.
         */
        private void SetSearchTextBoxVisibility(bool IsVisible)
        {
            if (IsVisible)
            {
                RouteGuideMap.IsEnabled = !IsVisible;

                SearchTextBox.SelectAll();
                SearchStaskPanel.Visibility = Visibility.Visible;
                SearchTextBox.Focus();

                CreateDialogApplicationBar();
            }
            else
            {
                RouteGuideMap.IsEnabled = !IsVisible;

                SearchStaskPanel.Visibility = Visibility.Collapsed;

                CreateMainApplicationBar();
            }
        }

        /*
         * функция устанавливает видимость списка результатов поиска
         */
        private void SetSearchResultsListVisibility(bool IsVisible)
        {
            if (IsVisible)
            {
                SearchResultsGrid.Visibility = Visibility.Visible;
            }
            else
            {
                SearchResultsGrid.Visibility = Visibility.Collapsed;
            }
        }

        /*
         * Функция, запускающая процесс поиска по заданному фрагменту строки
         */
        private void SearchForTerm(string term)
        {
            SetProgressIndicatiorVisibility(true, AppResources.ProgressIndicatorSearchTermText);
            searchQuery = new GeocodeQuery();
            searchQuery.SearchTerm = term;
            searchQuery.GeoCoordinate = myMapMarker.Coordinate == null ? new GeoCoordinate(0, 0) : myMapMarker.Coordinate;
            searchQuery.QueryCompleted += searchQuery_QueryCompleted;
            searchQuery.QueryAsync();
        }

        void searchQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            SetProgressIndicatiorVisibility(false);
            if (e.Error == null)
            {
                if (e.Result.Count > 0)
                {
                    if (_isRouteSearch)
                    {
                        // Был произведен поиск пункта назначения маршрута
                        //...
                    }
                    else
                    {
                        // Был произведен поиск объекта на карте по ключевому слову

                        // Записываем результаты поиска в список маркеров поиска
                        if (searchMapMarkers == null)
                            searchMapMarkers = new List<SearchMarker>();
                        else
                            searchMapMarkers.Clear();

                        for (int i = 0; i < e.Result.Count; i++)
                        {
                            SearchMarker searchMarker = new SearchMarker();
                            searchMarker.Coordinate = e.Result[i].GeoCoordinate;
                            searchMarker.Name = e.Result[i].Information.Name;
                            searchMarker.Description = e.Result[i].Information.Description;
                            searchMarker.Address = e.Result[i].Information.Address;
                            searchMapMarkers.Add(searchMarker);
                        }

                        // загрузка данных модели для отображения результатов поиска
                        if (!_isSearchAll)
                        {
                            DataContext = LoadSearchResultData();
                            SetSearchResultsListVisibility(true);
                        }
                        else
                            DrawSearchMarkers(RouteGuideMap.Layers[3]);

                        _isSearchAll = false;
                    }
                }
                else
                {
                    // очищаем DataContext и скрываем список результатов поиска
                    DataContext = null;
                    SetSearchResultsListVisibility(false);

                    //MessageBox.Show(AppResources.MainPageSearchNotFoundMessage);
                }

                searchQuery.Dispose();
            }
        }

        /*
         * функция дляотрисовки маркеров поиска на карте.
         * Если задан параметр index, то необходимо отобразить только один маркер с указанным индексом.
         */
        private void DrawSearchMarkers(MapLayer mapLayer, int index = -1)
        {
            if (searchMapMarkers != null)
            {
                mapLayer.Clear();

                if (index != -1)
                {
                    DrawMapMarker(searchMapMarkers[index].Coordinate, searchMapMarkers[index].Kind, mapLayer, index);
                    RouteGuideMap.SetView(searchMapMarkers[index].Coordinate, RouteGuideMap.ZoomLevel, MapAnimationKind.Parabolic);
                }
                else
                {

                    for (int i = 0; i < searchMapMarkers.Count; i++)
                    {
                        DrawMapMarker(searchMapMarkers[i].Coordinate, searchMapMarkers[i].Kind, mapLayer, i);
                    }
                    RouteGuideMap.SetView(searchMapMarkers[0].Coordinate, RouteGuideMap.ZoomLevel, MapAnimationKind.Parabolic);
                }
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
            if (searchMapMarkers == null)
                NavigationService.Navigate(new Uri("/Pages/PoiSettingsPage.xaml", UriKind.RelativeOrAbsolute));
            else if (searchMapMarkers.Count == 0)
                NavigationService.Navigate(new Uri("/Pages/PoiSettingsPage.xaml", UriKind.RelativeOrAbsolute));
            else
            {
                // Отмечаем в состоянии приложения, что будут передаваться данные на страницу настроек POI
                PhoneApplicationService.Current.State["markerTransmitting"] = true;

                // подготовка данных для передачи на страницу настроек POI
                string parameters = string.Format("?street={0}&houseNumber={1}&city={2}&county={3}",
                    searchMapMarkers[0].Address.Street,
                    searchMapMarkers[0].Address.HouseNumber,
                    searchMapMarkers[0].Address.City,
                    searchMapMarkers[0].Address.County);
                NavigationService.Navigate(new Uri("/Pages/PoiSettingsPage.xaml" + parameters, UriKind.RelativeOrAbsolute));
            }
        }

        /*
         * События нажатия кнопок в основном Application Bar
         */
        void appBarSearchButton_Click(object sender, EventArgs e)
        {
            SetSearchTextBoxVisibility(true);
        }

        void appBarLocationButton_Click(object sender, EventArgs e)
        {
            // определяем текущую позицию и перерисовываем её на соответствущем слое карты
            UpdateMyCurrentPosition(RouteGuideMap.Layers[1]);
        }

        void appBarFavoriteButton_Click(object sender, EventArgs e)
        {

        }

        void appBarLayersButton_Click(object sender, EventArgs e)
        {

        }


        /*
         * События нажатия кнопок Apply-Cancel Application Bar
         */
        void appBarApplyButton_Click(object sender, EventArgs e)
        {
            // обработка строки, введенной в SearchTextBox
            if (!SearchTextBox.Text.Equals(string.Empty))
            {
                _isSearchAll = true;
                SearchForTerm(SearchTextBox.Text);
            }

            DataContext = null;
            SetSearchResultsListVisibility(false);
            SetSearchTextBoxVisibility(false);
        }

        void appBarCancelButton_Click(object sender, EventArgs e)
        {
            DataContext = null;
            SetSearchResultsListVisibility(false);
            SetSearchTextBoxVisibility(false);
        }

        /*
         * События с элементами управления, связанными с функцией поиска
         */
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTextBox.Text.Equals(string.Empty))
            {
                lastSearchPhrase = string.Empty;

                // скрываем список найденных объектов
                DataContext = null;
                SetSearchResultsListVisibility(false);

                return;
            }

            // останавливаем и заново запускаем таймер
            searchLatencyTimer.Stop();
            searchLatencyTimer.Start();
        }

        /*
         * Событие, возникающее при задержке ввода символов в поле поиска
         */
        void searchLatencyTimer_Tick(object sender, EventArgs e)
        {
            searchLatencyTimer.Stop();

            if (SearchTextBox.Text.Equals(string.Empty))
            {
                lastSearchPhrase = string.Empty;

                // скрываем список найденных объектов
                DataContext = null;
                SetSearchResultsListVisibility(false);

                return;
            }

            // ничего не делаем, если фраза после изменения эквивалентна фразе до изменения
            if (SearchTextBox.Text.ToLower().Equals(lastSearchPhrase.ToLower()))
                return;

            // запуск поиска
            lastSearchPhrase = SearchTextBox.Text;
            SearchForTerm(SearchTextBox.Text);
        }

        /*
         * Событие изменения выбранного item'а в списке результатов поиска
         */
        private void SearchResultsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector list = sender as LongListSelector;

            if (list == null)
                return;

            SearchResultsItem selectedItem = list.SelectedItem as SearchResultsItem;

            if (selectedItem == null)
                return;

            // определяем по ItemId какой маркер из списка маркеров нужно отобразить
            DrawSearchMarkers(RouteGuideMap.Layers[3], selectedItem.ItemId);

            // скрываем всю панель поиска
            selectedItem = null;
            DataContext = null;
            SetSearchResultsListVisibility(false);
            SetSearchTextBoxVisibility(false);
        }

        /*
         * События выбора одного из пунктов меню основного Application Bar
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

        /*
         * События карты
         */
        private void RouteGuideMap_ZoomLevelChanged(object sender, MapZoomLevelChangedEventArgs e)
        {
            // перерисовываем изображение окружности погрешности определения местоположения пользователя
            foreach (MapOverlay overlay in RouteGuideMap.Layers[1])
            {
                Ellipse accuracyEllipce = overlay.Content as Ellipse;
                if (accuracyEllipce != null)
                {
                    UpdateMyLocationAccuracyRadius(overlay);
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