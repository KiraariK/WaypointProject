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
        Location = 5,

        // место назначения (для маршрутов)
        Destination = 6,

        // промежуточная точка (для маршрутов)
        Waypoint = 7,

        // ремонтные работы
        Repair = 8,

        // дорожное перекрытие
        StopAuto = 9,

        // пешеходное перекрытие
        StopWalk = 10,

        // менты
        Police = 11,

        // дорожная камера
        Camera = 12,

        // авария ДТП
        Crash = 13,

        // плохая дорога
        BadRoad = 14,

        // большая лужа
        Pool = 15,

        // лед
        Ice = 16,

        // чат
        Chat = 17,

        // ошибка на карте
        Mistake = 18,

        // Пользовательская метка
        Custom = 19
    }
}
