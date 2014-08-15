using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteGuide.ViewModels
{
    public class SearchResultsModel
    {
        public SearchResultsModel()
        {
            Items = new List<SearchResultsItem>();
        }

        public List<SearchResultsItem> Items { get; set; }

        public void LoadData(List<string> results)
        {
            SearchResultsModel viewModel = new SearchResultsModel();

            for (int i = 0; i < results.Count; i++)
            {
                viewModel.Items.Add(new SearchResultsItem
                {
                    Address = results.ElementAt(i),
                    ItemId = i
                });
            }

            Items.Clear();
            Items = viewModel.Items;

        }
    }
}
