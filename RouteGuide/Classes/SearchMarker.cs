using Microsoft.Phone.Maps.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteGuide.Classes
{
    /*
     * Класс для описания маркеров, определяющих объект поиска на карте
     */
    public class SearchMarker : Marker
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public MapAddress Address { get; set; }

        public SearchMarker()
        {
            Kind = MarkerKind.Search;
        }
    }
}
