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
        public string Address { get; set; }
    }
}
