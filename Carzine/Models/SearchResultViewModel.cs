using CarzineCore.Models;

namespace Carzine.Models
{
    public class SearchResultViewModel
    {
        public StandardProductModel? BestPrice { get; set; }

        public StandardProductModel? ExpressDelivery { get; set; }

        public IEnumerable<StandardProductModel>? Products { get; set; }
    }
}
