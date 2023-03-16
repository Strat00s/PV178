/* Pretty straight forward
 */

using HW02.BussinessContext.Services;

namespace HW02.Helpers
{
    public static class Seeder
    {
        public static void FillDB(CategoryService categoryService, ProductService productService)
        {
            int categoryId = categoryService.Create("Lake").Id;
            productService.Create("Canoe",       categoryId, (decimal)3599.99);
            productService.Create("Tent",        categoryId, (decimal)5685.22);
            productService.Create("Fishing rod", categoryId, (decimal)1599.39);

            categoryId     = categoryService.Create("House").Id;
            productService.Create("Chair",        categoryId, (decimal)399.99);
            productService.Create("Kitchen sink", categoryId, (decimal)4999.99);

            categoryId = categoryService.Create("Car").Id;
            productService.Create("Tire",       categoryId, (decimal)6000.25);
            productService.Create("Muffler",    categoryId, (decimal)3200.99);
            productService.Create("Seatbelt",   categoryId, (decimal)568.89);
            productService.Create("Windshield", categoryId, (decimal)15260.00);
        }
    }
}
