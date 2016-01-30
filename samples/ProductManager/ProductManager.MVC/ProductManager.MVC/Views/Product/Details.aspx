<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ProductManager.Common.Domain.Model.ProductManager.Product>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Details</h2>

    <fieldset>
        <legend>Fields</legend>
        
            <% var readonlyAttribute = new Dictionary<string, object>() { { "disabled", "disabled" } }; %>
            
            <div class="display-label">
                <%: Html.LabelFor(model => model.ProductID) %>
            </div>
            <div class="display-field">
                <%: Html.TextBoxFor(model => model.ProductID, readonlyAttribute)%>
            </div>
            
            <div class="display-label">
                <%: Html.LabelFor(model => model.Name)%>
            </div>
            <div class="display-field">
                <%: Html.TextBoxFor(model => model.Name, readonlyAttribute)%>
            </div>
            
            <div class="display-label">
                <%: Html.LabelFor(model => model.ProductNumber)%>
            </div>
            <div class="display-field">
                <%: Html.TextBoxFor(model => model.ProductNumber, readonlyAttribute)%>
            </div>
            
            <div class="display-label">
                <%: Html.LabelFor(model => model.Color)%>
            </div>
            <div class="display-field">
                <%: Html.TextBoxFor(model => model.Color, readonlyAttribute)%>
            </div>
            
            <div class="display-label">
                <%: Html.LabelFor(model => model.StandardCost)%>
            </div>
            <div class="display-field">
                <%: Html.TextBoxFor(model => model.StandardCost, readonlyAttribute)%>
            </div>
            
            <div class="display-label">
                <%: Html.LabelFor(model => model.ListPrice)%>
            </div>
            <div class="display-field">
                <%: Html.TextBoxFor(model => model.ListPrice, readonlyAttribute)%>
            </div>
            
            <div class="display-label">
                <%: Html.LabelFor(model => model.Size)%>
            </div>
            <div class="display-field">
                <%: Html.TextBoxFor(model => model.Size, readonlyAttribute)%>
            </div>
            
            <div class="display-label">
                <%: Html.LabelFor(model => model.Weight)%>
            </div>
            <div class="display-field">
                <%: Html.TextBoxFor(model => model.Weight, readonlyAttribute)%>
            </div>
            
            <div class="display-label">
                <%: Html.LabelFor(model => model.SellStartDate)%>
            </div>
            <div class="display-field">
                <%: Html.TextBoxFor(model => model.SellStartDate, readonlyAttribute)%>
            </div>
            
            <div class="display-label">
                <%: Html.LabelFor(model => model.SellEndDate)%>
            </div>
            <div class="display-field">
                <%: Html.TextBoxFor(model => model.SellEndDate, readonlyAttribute)%>
            </div>
            
            <div class="display-label">
                <%: Html.LabelFor(model => model.DiscontinuedDate)%>
            </div>
            <div class="display-field">
                <%: Html.TextBoxFor(model => model.DiscontinuedDate, readonlyAttribute)%>
            </div>

    </fieldset>
    <p>
        <%: Html.ActionLink("Edit", "Edit", new { id = Model.ProductID })%> |
        <%: Html.ActionLink("Back to List", "Index") %>
    </p>

</asp:Content>

