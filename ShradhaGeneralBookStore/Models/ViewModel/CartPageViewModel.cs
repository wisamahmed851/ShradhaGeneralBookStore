using ShradhaGeneralBookStore.Models.Entities;

namespace ShradhaGeneralBookStore.Models.ViewModel
{
    public class CartPageViewModel
    {
        public List<Cart> CartItems { get; set; }
        public List<Order> Orders { get; set; }
    }
}
