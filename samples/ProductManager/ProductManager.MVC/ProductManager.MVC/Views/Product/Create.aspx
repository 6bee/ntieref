<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ProductManager.Common.Domain.Model.ProductManager.Product>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create</h2>

    <% using (Html.BeginForm()) {%>
        <%: Html.ValidationSummary(true) %>

        <fieldset>
            <legend>Fields</legend>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Name)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Name)%>
                <%: Html.ValidationMessageFor(model => model.Name)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.ProductNumber)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.ProductNumber)%>
                <%: Html.ValidationMessageFor(model => model.ProductNumber)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Color)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Color)%>
                <%: Html.ValidationMessageFor(model => model.Color)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.StandardCost)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.StandardCost)%>
                <%: Html.ValidationMessageFor(model => model.StandardCost)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.ListPrice)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.ListPrice)%>
                <%: Html.ValidationMessageFor(model => model.ListPrice)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Size)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Size)%>
                <%: Html.ValidationMessageFor(model => model.Size)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Weight)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Weight)%>
                <%: Html.ValidationMessageFor(model => model.Weight)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.SellStartDate)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.SellStartDate)%>
                <%: Html.ValidationMessageFor(model => model.SellStartDate)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.SellEndDate)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.SellEndDate)%>
                <%: Html.ValidationMessageFor(model => model.SellEndDate)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.DiscontinuedDate)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.DiscontinuedDate)%>
                <%: Html.ValidationMessageFor(model => model.DiscontinuedDate)%>
            </div>
            
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%: Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>
