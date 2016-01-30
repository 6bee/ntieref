using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManager.MVC
{
    public class PagerBuilder
    {
        public static string SimplePager(int currentPage, int pageCount, string urlTemplate, string pagerClass)
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

        private class PagerLink
        {
            public string Title { get; set; }
            public int PageNo { get; set; }
            public string Class { get; set; }
            public bool IsCurrent { get; set; }
        }

        private readonly string _urlTemplate;
        private readonly List<PagerLink> _pagerLinks = new List<PagerLink>();

        public PagerBuilder(string urlTemplate)
        {
            _urlTemplate = urlTemplate;
        }

        public string PagerClass { get; set; }

        public void AddPage(string title, int pageNo, bool isCurrent = false)
        {
            AddPage(title, pageNo, string.Empty, isCurrent);
        }

        public void AddPage(string title, int pageNo, string itemClass, bool isCurrent = false)
        {
            var link = new PagerLink
            {
                PageNo = pageNo,
                Title = title,
                Class = itemClass,
                IsCurrent = isCurrent
            };
            _pagerLinks.Add(link);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("<div");

            if (!string.IsNullOrEmpty(PagerClass))
            {
                builder.Append(" class=\"");
                builder.Append(PagerClass);
                builder.Append("\"");
            }
            builder.Append(">");

            foreach (var link in _pagerLinks)
            {
                builder.Append("<a");

                if (!link.IsCurrent)
                {
                    builder.Append(" href=\"");
                    builder.AppendFormat(_urlTemplate, link.PageNo);
                    builder.Append("\"");
                }

                if (!string.IsNullOrEmpty(link.Class))
                {
                    builder.Append(" class=\"");
                    builder.Append(link.Class);
                    builder.Append("\"");
                }
                builder.Append(">");
                builder.Append(link.Title);
                builder.Append("</a>");
            }
            builder.Append("</div>");
            return builder.ToString();
        }
    }
}