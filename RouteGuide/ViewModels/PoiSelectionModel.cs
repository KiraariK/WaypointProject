using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteGuide.Resources;

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
            PoiSelectionModel viewModel = new PoiSelectionModel();

            viewModel.Items.Add(new PoiSelectionItem
            {
                PoiName = AppResources.PoiRepairsName,
                PoiDescription = AppResources.PoiRepairsDescription,
                PoiIconPath = "/Assets/PoiIcons/Repairs.png",
                PoiKind = (int)Classes.MarkerKind.Repair
            });

            viewModel.Items.Add(new PoiSelectionItem
            {
                PoiName = AppResources.PoiStopAutoName,
                PoiDescription = AppResources.PoiStopAutoDescription,
                PoiIconPath = "/Assets/PoiIcons/StopAuto.png",
                PoiKind = (int)Classes.MarkerKind.StopAuto
            });

            viewModel.Items.Add(new PoiSelectionItem
            {
                PoiName = AppResources.PoiStopWalkName,
                PoiDescription = AppResources.PoiStopWalkDescription,
                PoiIconPath = "/Assets/PoiIcons/StopWalk.png",
                PoiKind = (int)Classes.MarkerKind.StopWalk
            });

            viewModel.Items.Add(new PoiSelectionItem
            {
                PoiName = AppResources.PoiPoliceName,
                PoiDescription = AppResources.PoiPoliceDescription,
                PoiIconPath = "/Assets/PoiIcons/Police.png",
                PoiKind = (int)Classes.MarkerKind.Police
            });

            viewModel.Items.Add(new PoiSelectionItem
            {
                PoiName = AppResources.PoiCameraName,
                PoiDescription = AppResources.PoiCameraDescription,
                PoiIconPath = "/Assets/PoiIcons/Camera.png",
                PoiKind = (int)Classes.MarkerKind.Camera
            });

            viewModel.Items.Add(new PoiSelectionItem
            {
                PoiName = AppResources.PoiCrashName,
                PoiDescription = AppResources.PoiCrashDescription,
                PoiIconPath = "/Assets/PoiIcons/Crash.png",
                PoiKind = (int)Classes.MarkerKind.Crash
            });

            viewModel.Items.Add(new PoiSelectionItem
            {
                PoiName = AppResources.PoiBadRoadName,
                PoiDescription = AppResources.PoiBadRoadDescription,
                PoiIconPath = "/Assets/PoiIcons/BadRoad.png",
                PoiKind = (int)Classes.MarkerKind.BadRoad
            });

            viewModel.Items.Add(new PoiSelectionItem
            {
                PoiName = AppResources.PoiPoolName,
                PoiDescription = AppResources.PoiPoolDescription,
                PoiIconPath = "/Assets/PoiIcons/Pool.png",
                PoiKind = (int)Classes.MarkerKind.Pool
            });

            viewModel.Items.Add(new PoiSelectionItem
            {
                PoiName = AppResources.PoiIceName,
                PoiDescription = AppResources.PoiIceDescription,
                PoiIconPath = "/Assets/PoiIcons/Ice.png",
                PoiKind = (int)Classes.MarkerKind.Ice
            });

            viewModel.Items.Add(new PoiSelectionItem
            {
                PoiName = AppResources.PoiMistakeName,
                PoiDescription = AppResources.PoiMistakeDescription,
                PoiIconPath = "/Assets/PoiIcons/Mistake.png",
                PoiKind = (int)Classes.MarkerKind.Mistake
            });

            viewModel.Items.Add(new PoiSelectionItem
            {
                PoiName = AppResources.PoiCustomName,
                PoiDescription = AppResources.PoiCustomDescription,
                PoiIconPath = "/Assets/PoiIcons/Custom.png",
                PoiKind = (int)Classes.MarkerKind.Custom
            });

            Items = viewModel.Items;

            IsDataLoaded = true;
        }
    }
}
