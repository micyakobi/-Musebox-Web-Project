using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Musebox_Web_Project.Data;
using Musebox_Web_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace My_Musebox.Components
{

    public class RecommendedViewComponent : ViewComponent
    {
        private readonly Musebox_Web_ProjectContext _context;

        public RecommendedViewComponent(Musebox_Web_ProjectContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Product> allProducts = _context.Products.Include(p => p.Brand).ToList<Product>();
            User user = await _context.Users.FirstAsync(u => u.Email.Equals(UserClaimsPrincipal.FindFirstValue(ClaimTypes.Email)));

            var productsOrderedByUser = from p in _context.Products.Include(b => b.Brand)
                                        from o in _context.Order.Where(o => o.UserId == user.UserId)
                                        where o.OrderProducts.Any(op => op.ProductId == p.ProductId)
                                        select p;

            if (allProducts.Count() < 3)
                return await Task.FromResult((IViewComponentResult)View("Default", new List<Product>()));

            if (productsOrderedByUser.Count() == 0 && allProducts.Count() >= 3)
                return await Task.FromResult((IViewComponentResult)View("Default", allProducts.OrderBy(x => x.ProductPrice).Take(3)));


            double minPrice = productsOrderedByUser.Min(p => p.ProductPrice);
            double maxPrice = productsOrderedByUser.Max(p => p.ProductPrice);
            string mostFrequentBrand = productsOrderedByUser.ToList()
                .GroupBy(q => q.Brand.BrandName)
                .OrderByDescending(gp => gp.Count())
                .First()
                .Select(g => g.Brand.BrandName).First().ToString();

            List<string> orderedTypes = productsOrderedByUser
                .GroupBy(q => q.ProductType)
                .Select(p => p.Key).ToList<string>();

            List<Product> recommendedProducts = new List<Product>();

            foreach(Product item in allProducts)
            {
                if (item.ProductPrice >= minPrice 
                    && item.ProductPrice <= maxPrice 
                    && item.Brand.Equals(mostFrequentBrand)
                    && !orderedTypes.Contains(item.ProductType)){

                    recommendedProducts.Add(item);
                    if (recommendedProducts.Count() == 3)
                        return await Task.FromResult((IViewComponentResult)View("Default", recommendedProducts));
                }
            }

            foreach (Product item in allProducts)
            {
                if (item.Brand.Equals(mostFrequentBrand) 
                    && !orderedTypes.Contains(item.ProductType)
                    && !recommendedProducts.Contains(item)
                    && !productsOrderedByUser.Contains(item))
                {
                    recommendedProducts.Add(item);
                    if (recommendedProducts.Count() == 3)
                        return await Task.FromResult((IViewComponentResult)View("Default", recommendedProducts));
                }
            }


            if (recommendedProducts.Count() < 3)
            {

                foreach (Product item in allProducts)
                {
                    if (!orderedTypes.Contains(item.ProductType)
                        && !recommendedProducts.Contains(item)
                        && !productsOrderedByUser.Contains(item))
                    {
                        recommendedProducts.Add(item);
                        if (recommendedProducts.Count() == 3)
                            return await Task.FromResult((IViewComponentResult)View("Default", recommendedProducts));
                    }
                }

                foreach (Product item in allProducts)
                {
                    if(!recommendedProducts.Contains(item)
                        && !productsOrderedByUser.Contains(item)
                        && item.Brand.Equals(mostFrequentBrand))
                    {
                        recommendedProducts.Add(item);
                        if(recommendedProducts.Count() == 3)
                            return await Task.FromResult((IViewComponentResult)View("Default", recommendedProducts));
                    }
                }

                foreach (Product item in allProducts)
                    if (!recommendedProducts.Contains(item)
                        && !productsOrderedByUser.Contains(item))
                    {
                        recommendedProducts.Add(item);
                        if (recommendedProducts.Count() == 3)
                            return await Task.FromResult((IViewComponentResult)View("Default", recommendedProducts));
                    }
            }

            foreach (Product item in allProducts)
            {
                recommendedProducts.Add(item);
                if (recommendedProducts.Count() == 3)
                    return await Task.FromResult((IViewComponentResult)View("Default", recommendedProducts));
            }

            return await Task.FromResult((IViewComponentResult)View("Default", recommendedProducts));
        }

    }
}
