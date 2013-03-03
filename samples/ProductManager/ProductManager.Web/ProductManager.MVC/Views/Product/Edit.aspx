<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ProductManager.Common.Domain.Model.ProductManager.Product>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit</h2>
    
    <script src="~/Scripts/MicrosoftAjax.js" type="text/javascript"></script>
    <script src="~/Scripts/MicrosoftMvcValidation.js" type="text/javascript"></script>
    <% Html.EnableClientValidation(); %>
    
    <% using (Html.BeginForm()) { %>
        
        <%: Html.ValidationSummary(true) %>
        
        <fieldset>
            <legend>Fields</legend>
            <%--<% var readonlyAttribute = new Dictionary<string, object>() { { "readonly", "true" } }; %>--%>
            <% var readonlyAttribute = new Dictionary<string, object>() { { "disabled", "disabled" } }; %>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.ProductID) %>
            </div>
            <div class="display-field">
                <%: Html.TextBoxFor(model => model.ProductID, readonlyAttribute)%>
            </div>
            
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

            <div class="hidden">
                <!-- readonly fields and foreign key fields are placed here 
                     to allow asp.net mvc to submit original values rather than null or empty values -->
                <%: Html.TextBoxFor(model => model.ProductID)%>
                <%: Html.TextBoxFor(model => model.ProductCategoryID)%>
                <%: Html.TextBoxFor(model => model.ModifiedDate)%>
                <%: Html.TextBoxFor(model => model.rowguid)%>
            </div>
            
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%: Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>
