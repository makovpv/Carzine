using CarzineCore.Models;

namespace Carzine.Models
{
    public class SearchResultViewModel
    {
        public CartItemViewModel? BestPrice { get; set; }

        public CartItemViewModel? ExpressDelivery { get; set; }

        public CartItemViewModel? Optimal { get; set; }

        public IEnumerable<CartItemViewModel>? Products { get; set; }

        public decimal UsdRate { get; set; }
    }
}
