using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteGuide.ViewModels
{
    public class SearchResultsItem
    {
        public string Address { get; set; } // Последовательность компонентов адреса: улица, дом, город, область
        public int ItemId { get; set; } // соответствует номеру item'а в списке маркеров поиска
    }
}
