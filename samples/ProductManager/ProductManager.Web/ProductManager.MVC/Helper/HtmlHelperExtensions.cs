using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductManager.MVC
{
    public static class HtmlHelperExtensions
    {
        public static string SimplePager(this HtmlHelper helper, int currentPage, int pageCount, string urlTemplate, string pagerClass)
        {
            if (currentPage < 0) currentPage = 1;
            if (pageCount < 0) pageCount = 0;

            var pager = new PagerBuilder(urlTemplate);
            pager.PagerClass = pagerClass;

            if (currentPage > 1)
            {
                pager.AddPage("&lt;&lt;", 1);
                pager.AddPage("&lt;", currentPage - 1);
            }

            var start = Math.Max(currentPage - 2, 1);
            var end = Math.Min(pageCount, currentPage + 2);

            for (var i = start; i <= end; i++)
            {
                if (i == currentPage)
                {
                    pager.AddPage(i.ToString(), i, "current", true);
                }
                else
                {
                    pager.AddPage(i.ToString(), i);
                }
            }

            if (currentPage < pageCount)
            {
                pager.AddPage("&gt;", currentPage + 1);
                pager.AddPage("&gt;&gt;", pageCount);
            }

            return pager.ToString();
        }
    }
}