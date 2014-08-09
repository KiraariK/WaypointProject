using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RouteGuide.ViewModels
{
    public class PoiLocationSettingsItem
    {
        public string Content { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Visibility IsChecked { get; set; }
        public int Id { get; set; }
    }
}
