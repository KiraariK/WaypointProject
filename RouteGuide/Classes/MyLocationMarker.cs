using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteGuide.Classes
{
    /*
     * Класс для описания маркеров, определяющих текущее местоположение
     */
    public class MyLocationMarker : Marker
    {
        private static MyLocationMarker _instance = null;
        protected MyLocationMarker() { }
        public static MyLocationMarker Instance()
        {
            if (_instance == null)
                return new MyLocationMarker();
            else
                return _instance;
        }

        public double Accuracy { get; set; }
    }
}
