using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProductManager.Client.Domain;
using ProductManager.Common.Domain.Model.ProductManager;
using NTier.Client.Domain;

namespace ProductManager.MVC.Controllers
{
    [HandleError]
    public class ProductController : Controller
    {
        #region DataContext
        private const int PageSize = 15;
        private ProductManagerDataContext DataContext
        {
            get
            {
                var dataContex = Session["ProductDataContext"] as ProductManagerDataContext;
                if (dataContex == null)
                {
                    dataContex = new ProductManagerDataContext() { MergeOption = MergeOption.NoTracking };
                    Session["ProductDataContext"] = dataContex;

                    // preload categories
                    //dataContex.ProductCategories.MergeOption = MergeOption.PreserveChanges;
                    //dataContex.ProductCategories.AsQueryable().ToList();
                }
                return dataContex;
            }
        }
        #endregion DataContext


        #region Index
        // GET: /Product/Index/[sort][Page]
        public ActionResult Index(string sort, string sortDirection, int? page)
        {
            var query = DataContext.Products.AsQueryable()
                .IncludeTotalCount()
                .OrderBy(p => p.ProductID)
                .Take(PageSize);

            bool desc = false;

            #region Sort
            if (!string.IsNullOrWhiteSpace(sort))
            {
                desc = sortDirection == "desc";

                // apply sorting
                query = desc ? query.OrderByDescending(p => p[sort]) : query.OrderBy(p => p[sort]);
            }
            #endregion Sort

            #region Paging
            if (page.HasValue)
            {
                query = query
                    .Skip((page.Value - 1) * PageSize)
                    .Take(PageSize);
            }
            #endregion Paging

            var result = query.Execute();

            #region View Parameters
            ViewData["CurrentPage"] = page ?? 1;
            ViewData["PageCount"] = (int)((result.EntitySet.TotalCount / PageSize) + (result.EntitySet.TotalCount % PageSize > 0 ? 1 : 0));
            ViewData["SortColumn"] = sort ?? "ProductID";
            ViewData["SortDirection"] = desc ? "desc" : "asc";
            #endregion View Parameters

            return View(result.ResultSet);
        }
        #endregion Index


        #region Edit
        // GET: /Product/Edit/[id]
        public ActionResult Edit(int id)
        {
            var product = DataContext.Products.AsQueryable().FirstOrDefault(p => p.ProductID == id);
            return View(product);
        }

        // POST: /Product/Edit/[id]
        [HttpPost]
        public ActionResult Edit([Bind] Product product)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = DataContext.Products.AsQueryable().FirstOrDefault(p => p.ProductID == product.ProductID);
                if (existingProduct != null)
                {
                    DataContext.AttachAsModified(product, existingProduct);
                    DataContext.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(product);
        }
        #endregion Edit


        #region Create
        // GET: /Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Product/Create/[id]
        [HttpPost]
        public ActionResult Create([Bind] Product product)
        {
            if (base.TryValidateModel(product))
            {
                DataContext.Add(product);
                DataContext.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(product);
        }
        #endregion Create


        #region Delete
        // GET: /Product/Delete/[id]
        public ActionResult Delete(int id)
        {
            var product = DataContext.Products.AsQueryable().FirstOrDefault(p => p.ProductID == id);
            if (product != null)
            {
                DataContext.Products.Delete(product);
                DataContext.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        #endregion Delete


        #region Details
        // GET: /Product/Details/[id]
        public ActionResult Details(int id)
        {
            var product = DataContext.Products.AsQueryable().FirstOrDefault(p => p.ProductID == id);
            return View(product);
        }
        #endregion Details
    }
}
