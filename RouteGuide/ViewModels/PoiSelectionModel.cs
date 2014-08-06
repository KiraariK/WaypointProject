using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteGuide.ViewModels
{
    public class PoiSelectionModel
    {
        public PoiSelectionModel()
        {
            Items = new List<PoiSelectionItem>();
        }

        public List<PoiSelectionItem> Items { get; set; }

        public bool IsDataLoaded { get; set; }

        public void LoadData()
        {


            IsDataLoaded = true;
        }
    }
}
