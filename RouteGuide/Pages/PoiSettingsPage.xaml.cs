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
using System.Windows.Media.Imaging;

using RouteGuide.ViewModels;

namespace RouteGuide.Pages
{
    public partial class PoiSettingsPage : PhoneApplicationPage
    {
        private MarkerKind selectedPoiKind;
        private string selectedPoiName;
        private string selectedPoiDescription;
        private string selectedPoiIconPath;
        private string selectedPoiComment;
        private double selectedPoiMarkerLatitude;
        private double selectedPoiMarkerLongitude;

        private string markerSearchLocationContent = string.Empty;

        // Показывает, происходит ли переход с данной страницы вперед
        private bool _isGoForward = false;

        private static PoiLocationSettingsModel poilocationModel = null;

        public PoiSettingsPage()
        {
            InitializeComponent();

            CreateDefaultPoiData();

            CreateApplicationBar();
        }

        /*
         * Загрузка данных для модели отображения списка локаций для выставления маркера POI
         */
        private object LoadPoiLocationData(string markerSearchContent = "")
        {
            if (poilocationModel == null)
                poilocationModel = new PoiLocationSettingsModel();

            poilocationModel.LoadData(markerSearchContent);
            return poilocationModel;
        }

        /*
         * Перезагружает данные модели, вставляя результаты поиска новой локации
         */
        private object ReloadPoiLocationData(string searchContent, double latitude, double longitude, string markerSearchContent = "")
        {
            if (poilocationModel == null)
                poilocationModel = new PoiLocationSettingsModel();

            poilocationModel.LoadData(markerSearchContent);
            poilocationModel.InsertSearchResultData(searchContent, latitude, longitude);
            return poilocationModel;
        }

        /*
         * Отмечает item с указанным индеском как checked (при выборе item'а из списка)
         */
        private object UpdateCheckingModelItems(PoiLocationSettingsItem item, string markerSearchContent = "")
        {
            if (poilocationModel == null)
                poilocationModel = new PoiLocationSettingsModel();

            poilocationModel.LoadData(markerSearchContent);
            poilocationModel.CheckItem(item);
            return poilocationModel;
        }

        /*
         * Сбрасывает показатель того, что модель была изменена.
         * При следующей загрузке она загрузится с нуля.
         */
        private void ResetModel()
        {
            if (poilocationModel != null)
                poilocationModel.ResetModel();
        }

        public List<PoiLocationSettingsItem> GetModelDataList(string markerSearchContent = "")
        {
            if (poilocationModel == null)
                poilocationModel = new PoiLocationSettingsModel();

            poilocationModel.LoadData(markerSearchContent);
            return poilocationModel.Items;
        }

        /*
         * Создает Application Bar, запихивая кнопки 'принять' и 'отменить'
         */
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

        /*
         * Заполняет данные о POI по умлочанию:
         * если до этого в данном сеансе работы с приложением пользователь не выбирал POI,
         * то выставляется POI по-умлочанию,
         * иначе выставляется последняя выбранная POI
         */
        private void CreateDefaultPoiData()
        {
            // если нету сохраненной выбранной в прошлый раз POI, то выставляем по умолчанию
            // тестовый вариант!!!
            selectedPoiKind = MarkerKind.Police;
            selectedPoiName = AppResources.PoiPoliceName;
            selectedPoiDescription = AppResources.PoiPoliceDescription;
            selectedPoiIconPath = "/Assets/PoiIcons/Police.png";
            selectedPoiComment = string.Empty;
            selectedPoiMarkerLatitude = double.NaN;
            selectedPoiMarkerLongitude = double.NaN;
        }

        /*
         * Отрисовывает данные о POI
         */
        private void SetViewFromData()
        {
            BitmapImage image = new BitmapImage(new Uri(selectedPoiIconPath, UriKind.RelativeOrAbsolute));
            PoiSettingsImage.Source = image;
            PoiSettingsName.Text = selectedPoiName;
            PoiSettingsDescription.Text = selectedPoiDescription;
        }

        /*
         * События кнопок в Application Bar
         */
        void appBarApplyButton_Click(object sender, EventArgs e)
        {
            SavePoiSettings();
            ResetModel();
            NavigationService.GoBack();
        }

        void appBarCancelButton_Click(object sender, EventArgs e)
        {
            ResetModel();
            NavigationService.GoBack();
        }

        /*
         * функция сохранения параметрорв для передачи на предыдущую страницу перед переходом
         */
        private void SavePoiSettings()
        {
            // забираем комментарий пользователя
            selectedPoiComment = PoiSettingsCommentTextBox.Text;

            // Выгружаем данные из модели
            List<PoiLocationSettingsItem> modelItems = GetModelDataList(markerSearchLocationContent);
            if (modelItems == null)
                return;

            PoiLocationSettingsItem selectedLocation = null;
            for (int i = 0; i < modelItems.Count; i++)
            {
                if (modelItems[i].IsChecked == Visibility.Visible)
                    selectedLocation = modelItems[i];
            }
            if (selectedLocation == null)
                return;

            PhoneApplicationService.Current.State["type"] = "PoiSettings";

            if (selectedLocation.Id == 0)
            {
                // текущее местоположение пользователя (координаты будут определены на MainPage)
                PhoneApplicationService.Current.State["location"] = "me";
            }
            else if (selectedLocation.Id == 1)
            {
                // маркер поиска на карте (координаты будут определены на MainPage)
                PhoneApplicationService.Current.State["location"] = "marker";
            }
            else if (selectedLocation.Id == 2)
            {
                // координаты будут записаны в состояние приложения для того, чтобы их можно было извлечь на MainPage
                PhoneApplicationService.Current.State["location"] = "search";

                // !!! Только в качестве заглушки !!!
                selectedPoiMarkerLatitude = 56.4632;
                selectedPoiMarkerLongitude = 84.9726;
                // !!! Только в качестве заглушки !!!
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
         * Событие нажатия на кнопку выбора POI
         */
        private void PoiSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            _isGoForward = true;
            NavigationService.Navigate(new Uri("/Pages/PoiSelectionPage.xaml", UriKind.RelativeOrAbsolute));
        }

        /*
         * Событие выбора одного из item'ов списка
         */
        private void SelectPoiLocationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector list = sender as LongListSelector;

            if (list == null)
                return;

            PoiLocationSettingsItem data = list.SelectedItem as PoiLocationSettingsItem;

            if (data == null)
                return;

            DataContext = null;
            DataContext = UpdateCheckingModelItems(data, markerSearchLocationContent);
        }

        /*
         * События страницы
         */
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // обрабатываем передачу данных со станицы выбора POI
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

                if (type.Equals("SelectedPoi"))
                {
                    // данные передала страница выбра POI

                    selectedPoiName = PhoneApplicationService.Current.State["poiName"] as string;
                    // удаляем из состояния приложения
                    PhoneApplicationService.Current.State.Remove("poiName");

                    selectedPoiDescription = PhoneApplicationService.Current.State["poiDescription"] as string;
                    // удаляем из состояния приложения
                    PhoneApplicationService.Current.State.Remove("poiDescription");

                    selectedPoiIconPath = PhoneApplicationService.Current.State["poiIconPath"] as string;
                    // удаляем из состояния приложения
                    PhoneApplicationService.Current.State.Remove("poiIconPath");

                    selectedPoiKind = (MarkerKind)PhoneApplicationService.Current.State["poiKind"];
                    // удаляем из состояния приложения
                    PhoneApplicationService.Current.State.Remove("poiKind");

                    SetViewFromData();
                }
                // удаляем пару с ключом type из состояния приложения
                PhoneApplicationService.Current.State.Remove("type");
            }

            // Проверяем, посылались ди данные с MainPage
            if (PhoneApplicationService.Current.State.ContainsKey("markerTransmitting"))
            {
                if ((bool)PhoneApplicationService.Current.State["markerTransmitting"])
                {
                    string street = string.Empty;
                    if (NavigationContext.QueryString.TryGetValue("street", out street))
                        markerSearchLocationContent += street + " ";
                    string houseNumber = string.Empty;
                    if (NavigationContext.QueryString.TryGetValue("houseNumber", out houseNumber))
                        markerSearchLocationContent += houseNumber + " ";
                    string city = string.Empty;
                    if (NavigationContext.QueryString.TryGetValue("city", out city))
                        markerSearchLocationContent += city + " ";
                    string county = string.Empty;
                    if (NavigationContext.QueryString.TryGetValue("county", out county))
                        markerSearchLocationContent += county;
                }
                PhoneApplicationService.Current.State.Remove("markerTransmitting");
            }

            DataContext = null;
            DataContext = LoadPoiLocationData(markerSearchLocationContent);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // Если уходим на предыдущую страницу, то сбрасываем модель
            if (!_isGoForward)
                ResetModel();
            _isGoForward = false;
        }
    }
}