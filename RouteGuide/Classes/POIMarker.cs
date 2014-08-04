using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RouteGuide.Classes
{
    /*
     * Класс для описания маркеров, определяющих метку интереса пользователя
     */
    public class POIMarker : Marker
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatorId { get; set; }
        public string CreatorName { get; set; }
        public Image CreatorAvatar { get; set; }
        public string Comment { get; set; }
    }
}
