using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteGuide.Classes
{
    /*
     * Класс для описания объектов обычных маркеров.
     * С ними нельзя выполнять каких либо действий.
     * Их можно только устанавливать, обновлять и удалять.
     * (особый маркер - дом, работа)
     */
    public class Marker
    {
        public GeoCoordinate Coordinate { get; set; }
        public MarkerKind Kind { get; set; }
    }
}
