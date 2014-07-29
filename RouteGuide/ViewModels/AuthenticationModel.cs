using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace RouteGuide.ViewModels
{
    public class AuthenticationModel
    {
        public AuthenticationModel()
        {
            Items = new List<AuthenticationItem>();
        }

        public List<AuthenticationItem> Items { get; set; }

        public bool IsDataLoaded { get; set; }

        public void LoadData()
        {
            // !!! - не смог достучаться по относительному пути к AuthenticationSampleData.xaml - !!!
            //string dataPath = "../SampleData/AuthenticationSampleData.xaml";
            //using (StreamReader reader = new StreamReader(dataPath))
            //{
            //    AuthenticationModel viewModel = XamlReader.Load(reader.ReadToEnd()) as AuthenticationModel;
            //    Items = viewModel.Items;
            //}

            AuthenticationModel viewModel = new AuthenticationModel();

            viewModel.Items.Add(new AuthenticationItem
            {
                Title = "facebook",
                LogoPath = "/Assets/Logos/Facebook_logo.png",
                BackgroundColor = "#3b579d"
            });

            viewModel.Items.Add(new AuthenticationItem
            {
                Title = "twitter",
                LogoPath = "/Assets/Logos/Twitter_logo.png",
                BackgroundColor = "#00aced"
            });

            viewModel.Items.Add(new AuthenticationItem
            {
                Title = "ВКонтакте",
                LogoPath = "/Assets/Logos/VK_logo.png",
                BackgroundColor = "#155e8b"
            });

            Items = viewModel.Items;

            IsDataLoaded = true;
        }
    }
}
