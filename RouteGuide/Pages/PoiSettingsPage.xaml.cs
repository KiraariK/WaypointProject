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

        public PoiSettingsPage()
        {
            InitializeComponent();

            CreateDefaultPoiData();

            CreateApplicationBar();
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
            NavigationService.GoBack();
        }

        void appBarCancelButton_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        /*
         * функция сохранения параметрорв для передачи на предыдущую страницу перед переходом
         */
        private void SavePoiSettings()
        {
            // забираем комментарий пользователя
            selectedPoiComment = PoiSettingsCommentTextBox.Text;

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
         * Событие нажатия на кнопку выбора метки
         */
        private void PoiSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/PoiSelectionPage.xaml", UriKind.RelativeOrAbsolute));
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
        }
    }
}