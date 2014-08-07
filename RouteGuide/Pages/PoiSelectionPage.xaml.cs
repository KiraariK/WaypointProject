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
using RouteGuide.ViewModels;
using RouteGuide.Classes;

namespace RouteGuide.Pages
{
    public partial class PoiSelectionPage : PhoneApplicationPage
    {
        private static PoiSelectionModel poiselectionViewModel = null;

        // последний выделенный элемент списка меток
        private PoiSelectionItem lastSelectedItem;
        // если выбрана пользовательская метка
        private string customPoiName;
        private string customPoiDescription;

        // показывает, показана ли сейчас панель ввода пользовательских настроек POI
        private bool IsCustomSettingsInputEnabled;

        public PoiSelectionPage()
        {
            InitializeComponent();
            IsCustomSettingsInputEnabled = false;

            DataContext = LoadPoiSelectionData();

            CreateApplicationBar();
        }

        /*
         * Загрузка данных для модели отображения списка меток
         */
        private object LoadPoiSelectionData()
        {
            if (poiselectionViewModel == null)
            {
                poiselectionViewModel = new PoiSelectionModel();
                poiselectionViewModel.LoadData();
                return poiselectionViewModel;
            }
            else if (!poiselectionViewModel.IsDataLoaded)
            {
                poiselectionViewModel.LoadData();
                return poiselectionViewModel;
            }
            else
            {
                return poiselectionViewModel;
            }
        }

        /*
         * Создание Application Bar и кнопок в нем
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

            ApplicationBar.IsVisible = false;
        }

        /*
         * События кнопок Application Bar
         */
        void appBarApplyButton_Click(object sender, EventArgs e)
        {
            if (!IsCustomSettingsInputEnabled)
            {
                // Если панель ввода пользовательских настроек POI скрыта
                SaveSelectedPoiParameters();
                NavigationService.GoBack();
            }
            else
            {
                // Открыта панель ввода пользовательских настроек POI
                if (!CustomNameTextBox.Text.Equals(string.Empty) && !CustomDescriptionTextBox.Text.Equals(string.Empty))
                {
                    customPoiName = CustomNameTextBox.Text;
                    customPoiDescription = CustomDescriptionTextBox.Text;

                    SetCustomPoiSettingsVisibility(false);
                }
                else
                {
                    MessageBox.Show(AppResources.PoiSettingsCustomEmptyError);
                }
            }
        }

        void appBarCancelButton_Click(object sender, EventArgs e)
        {
            if (!IsCustomSettingsInputEnabled)
            {
                // Если панель ввода пользовательских настроек POI скрыта
                NavigationService.GoBack();
            }
            else
            {
                // Открыта панель ввода пользовательских настроек POI
                PoiSelectionList.SelectedItems.Clear();
                SetCustomPoiSettingsVisibility(false);
                ApplicationBar.IsVisible = false;
            }
        }

        /*
         * функция сохранения параметрорв для передачи на предыдущую страницу перед переходом
         */
        private void SaveSelectedPoiParameters()
        {
            if (lastSelectedItem == null)
                return;

            PhoneApplicationService.Current.State["type"] = "SelectedPoi";

            if (lastSelectedItem.PoiKind != (int)MarkerKind.Custom)
            {
                PhoneApplicationService.Current.State["poiName"] = lastSelectedItem.PoiName;
                PhoneApplicationService.Current.State["poiDescription"] = lastSelectedItem.PoiDescription;
            }
            else
            {
                PhoneApplicationService.Current.State["poiName"] = customPoiName;
                PhoneApplicationService.Current.State["poiDescription"] = customPoiDescription;
            }
            PhoneApplicationService.Current.State["poiIconPath"] = lastSelectedItem.PoiIconPath;
            PhoneApplicationService.Current.State["poiKind"] = lastSelectedItem.PoiKind;
        }

        /*
         * Функция, которая устанавливает видимость панели ввода пользовательских настроек метки
         */
        private void SetCustomPoiSettingsVisibility(bool IsVisible)
        {
            if (IsVisible)
            {
                PoiSelectionTitle.Visibility = Visibility.Collapsed;
                PoiSelectionList.IsEnabled = !IsVisible;
                IsCustomSettingsInputEnabled = true;

                CustomNameTextBox.Text = string.Empty;
                CustomDescriptionTextBox.Text = string.Empty;
                CustomNameTextBox.SelectAll();
                CustomPoiSettings.Visibility = Visibility.Visible;
                CustomNameTextBox.Focus();
            }
            else
            {
                PoiSelectionList.IsEnabled = !IsVisible;
                PoiSelectionTitle.Visibility = Visibility.Visible;
                CustomPoiSettings.Visibility = Visibility.Collapsed;
                IsCustomSettingsInputEnabled = false;
            }
        }

        /*
         * События списка
         */
        private void PoiSelectionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
             * Т.к. используется список с множественным выбором, то необхоимо оставлять в списке только
             * последний выделенный пункт.
             */
            if (PoiSelectionList.SelectedItems.Count == 1)
            {
                if (PoiSelectionList.SelectedItems[0] == lastSelectedItem && lastSelectedItem.PoiKind != (int)MarkerKind.Custom)
                    return;

                lastSelectedItem = PoiSelectionList.SelectedItems[0] as PoiSelectionItem;
            }
            else if (PoiSelectionList.SelectedItems.Count > 1)
            {
                PoiSelectionList.SelectedItems.Remove(lastSelectedItem);
                lastSelectedItem = PoiSelectionList.SelectedItems[0] as PoiSelectionItem;
            }
            else
            {
                // Выбрана метка, которая была выделена перед этим (щелчок по той же метке)
                if (lastSelectedItem.PoiKind != (int)MarkerKind.Custom)
                    PoiSelectionList.SelectedItems.Add(lastSelectedItem);
            }

            if (PoiSelectionList.SelectedItems.Count != 0)
                ApplicationBar.IsVisible = true;
            else
                ApplicationBar.IsVisible = false;

            if (lastSelectedItem.PoiKind == (int)MarkerKind.Custom)
            {
                SetCustomPoiSettingsVisibility(true);
            }
        }

        /*
         * События страницы
         */
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!poiselectionViewModel.IsDataLoaded)
            {
                DataContext = LoadPoiSelectionData();
            }
        }
    }
}