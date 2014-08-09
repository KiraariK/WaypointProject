using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RouteGuide.Resources;
using System.Windows;

namespace RouteGuide.ViewModels
{
    public class PoiLocationSettingsModel
    {
        public PoiLocationSettingsModel()
        {
            Items = new List<PoiLocationSettingsItem>();
        }

        public List<PoiLocationSettingsItem> Items { get; set; }

        public bool IsDataChanged { get; set; }

        public void LoadData(string searchMarkerContent = "")
        {
            PoiLocationSettingsModel viewModel = new PoiLocationSettingsModel();

            if (!IsDataChanged)
            {
                // первое всегда: мое местоположение
                viewModel.Items.Add(new PoiLocationSettingsItem
                {
                    Content = AppResources.PoiSettingsMyLocation,
                    IsChecked = Visibility.Visible,
                    Latitude = double.NaN,
                    Longitude = double.NaN,
                    Id = 0
                });

                // второе и до предпоследнего включительно: может быть любым
                if (!searchMarkerContent.Equals(""))
                {
                    viewModel.Items.Add(new PoiLocationSettingsItem
                    {
                        Content = searchMarkerContent,
                        IsChecked = Visibility.Collapsed,
                        Latitude = double.NaN,
                        Longitude = double.NaN,
                        Id = 1
                    });
                }

                // последнее: указать другое место (запустить поиск)
                viewModel.Items.Add(new PoiLocationSettingsItem
                {
                    Content = AppResources.PoiSettingsSearchLocation,
                    IsChecked = Visibility.Collapsed,
                    Latitude = double.NaN,
                    Longitude = double.NaN,
                    Id = 2
                });
            }
            else
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    viewModel.Items.Add(new PoiLocationSettingsItem
                    {
                        Content = Items[i].Content,
                        IsChecked = Items[i].IsChecked,
                        Latitude = Items[i].Latitude,
                        Longitude = Items[i].Longitude,
                        Id = Items[i].Id
                    });
                }
            }

            Items.Clear();

            Items = viewModel.Items;
        }

        /* 
         * Перезагружает модель, вставляя информацию о найденном месте в конец
         */
        public void InsertSearchResultData(string searchContent, double latitude, double longitude)
        {
            PoiLocationSettingsModel viewModel = new PoiLocationSettingsModel();

            for (int i = 0; i < Items.Count - 1; i++)
            {
                viewModel.Items.Add(new PoiLocationSettingsItem
                {
                    Content = Items[i].Content,
                    IsChecked = Visibility.Collapsed,
                    Latitude = Items[i].Latitude,
                    Longitude = Items[i].Longitude,
                    Id = Items[i].Id
                });
            }

            viewModel.Items.Add(new PoiLocationSettingsItem
            {
                Content = searchContent,
                IsChecked = Visibility.Visible,
                Latitude = latitude,
                Longitude = longitude,
                Id = 2
            });

            Items.Clear();

            Items = viewModel.Items;
        }

        /*
         * Отмечает указанный item как checked
         */
        public void CheckItem(PoiLocationSettingsItem replacedItem)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (replacedItem.Id == Items[i].Id)
                    Items[i].IsChecked = Visibility.Visible;
                else
                    Items[i].IsChecked = Visibility.Collapsed;
            }

            IsDataChanged = true;
        }

        /*
         * Сбрасывает показатель того, что модель была изменена.
         * При следующей загрузке она загрузится с нуля.
         */
        public void ResetModel()
        {
            IsDataChanged = false;
        }
    }
}
