using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteGuide.Classes
{
    /*
     * указывает тип метки, помещаемой на карту
     */
    public enum MarkerKind
    {
        // никакой
        None = 0,

        // мое местположение
        Me = 1,

        // мой дом
        Home = 2,

        // моя работа
        Work = 3,

        // маркер поиска
        Search = 4,

        // место оправления (для матршрутов)
        Source = 5,

        // место назначения (для маршрутов)
        Destination = 6,

        // ремонтные работы
        Repair = 7,

        // дорожное перекрытие
        StopAuto = 8,

        // пешеходное перекрытие
        StopWalk = 9,

        // менты
        Police = 10,

        // дорожная камера
        Camera = 11,

        // авария ДТП
        Crash = 12,

        // плохая дорога
        BadRoad = 13,

        // большая лужа
        Pool = 14,

        // лед
        Ice = 15,

        // ошибка на карте
        Mistake = 16,

        // Пользовательская метка
        Custom = 17
    }
}
