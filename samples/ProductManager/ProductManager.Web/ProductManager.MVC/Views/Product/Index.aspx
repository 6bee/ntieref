<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<ProductManager.Common.Domain.Model.ProductManager.Product>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Products
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Products</h2>

    <table>
        <tr>
            <th></th>
            <th>
                <%: Html.ActionLink("ID", "Index", new { sort = "ProductID", sortDirection = (("ProductID" == (string)ViewData["SortColumn"] && ViewData["SortDirection"]=="asc") ? "desc":"asc") })%>
            </th>
            <th>
                <%: Html.ActionLink("Name", "Index", new { sort = "Name", sortDirection = (("Name" == (string)ViewData["SortColumn"] && ViewData["SortDirection"]=="asc") ? "desc":"asc") })%>
            </th>
            <th>
                <%: Html.ActionLink("Color", "Index", new { sort = "Color", sortDirection = (("Color" == (string)ViewData["SortColumn"] && ViewData["SortDirection"]=="asc") ? "desc":"asc") })%>
            </th>
            <th>
                <%: Html.ActionLink("Price", "Index", new { sort = "ListPrice", sortDirection = (("ListPrice" == (string)ViewData["SortColumn"] && ViewData["SortDirection"] == "asc") ? "desc" : "asc") })%>                
            </th>
        </tr>
        <% foreach (var item in Model) { %>
        <tr>
            <td>
                <%: Html.ActionLink("Edit", "Edit", new { id = item.ProductID })%> |
                <%: Html.ActionLink("Details", "Details", new { id = item.ProductID })%> |
                <%: Html.ActionLink("Delete", "Delete", new { id = item.ProductID })%>
            </td>
            <td>
                <%: item.ProductID%>
            </td>
            <td>
                <%: item.Name%>
            </td>
            <td>
                <%: item.Color%>
            </td>
            <td>
                <%: item.ListPrice%>
            </td>
        </tr>
        <% } %>
    </table>
    
    <%= ProductManager.MVC.PagerBuilder.SimplePager((int)ViewData["CurrentPage"], (int)ViewData["PageCount"], 
        "/Product/Index?page={0}&sort=" + ViewData["SortColumn"] + "&sortDirection=" + ViewData["SortDirection"], "pager") %>
    
    <p>
        <%: Html.ActionLink("Create New", "Create") %>
    </p>
</asp:Content>
