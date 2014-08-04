using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteGuide.Classes
{
    public class MarkerIconStore
    {
        private static string noneMarkerPath = "";
        private static string myMarkerPath = "/Assets/Markers/Me.png";
        private static string homeMarkerPath = "";
        private static string workMarkerPath = "";
        private static string searchMarkerPath = "";
        private static string locationMarkerPath = "";
        private static string destinationMarkerPath = "";
        private static string waypointMarkerPath = "";
        private static string repairMarkerPath = "";
        private static string stopautoMarkerPath = "";
        private static string stopwalkMarkerPath = "";
        private static string policeMarkerPath = "";
        private static string cameraMarkerPath = "";
        private static string crashMarkerPath = "";
        private static string badroadMarkerPath = "";
        private static string poolMarkerPath = "";
        private static string iceMarkerPath = "";
        private static string chatMarkerPath = "";
        private static string mistakeMarkerPath = "";
        private static string customMarkerPath = "";

        public static string GetMarkerPath(MarkerKind kind)
        {
            switch (kind)
            {
                case MarkerKind.Me: return myMarkerPath;
                case MarkerKind.Home: return homeMarkerPath;
                case MarkerKind.Work: return workMarkerPath;
                case MarkerKind.Search: return searchMarkerPath;
                case MarkerKind.Location: return locationMarkerPath;
                case MarkerKind.Destination: return destinationMarkerPath;
                case MarkerKind.Waypoint: return waypointMarkerPath;
                case MarkerKind.Repair: return repairMarkerPath;
                case MarkerKind.StopAuto: return stopautoMarkerPath;
                case MarkerKind.StopWalk: return stopwalkMarkerPath;
                case MarkerKind.Police: return policeMarkerPath;
                case MarkerKind.Camera: return cameraMarkerPath;
                case MarkerKind.Crash: return crashMarkerPath;
                case MarkerKind.BadRoad: return badroadMarkerPath;
                case MarkerKind.Pool: return poolMarkerPath;
                case MarkerKind.Ice: return iceMarkerPath;
                case MarkerKind.Chat: return chatMarkerPath;
                case MarkerKind.Mistake: return mistakeMarkerPath;
                case MarkerKind.Custom: return customMarkerPath;
                default: return noneMarkerPath;
            }
        }
    }
}
