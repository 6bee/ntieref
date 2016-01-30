using C1.Silverlight;
using C1.Silverlight.DataGrid;
using NTier.Client.Domain;
using ProductManager.Common.Domain.Model.ProductManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace ProductManager.Silverlight.Controls
{
    //public class FilterDescriptor
    //{
    //    public string PropertyName { get; set; }
    //    public object Value { get; set; }
    //}

    public class DataGridFilterRow : DataGridRow
    {
        //private readonly Dictionary<DataGridColumn, SearchBox> _cells = new Dictionary<DataGridColumn,SearchBox>();
        ////public ReadOnlyCollection<Tuple<C1SearchBox, DataGridColumn>> Cells
        ////{
        ////    get
        ////    {
        ////        return _cells.AsReadOnly();
        ////    }
        ////}
        //public ReadOnlyCollection<FilterDescriptor> Filters
        //{
        //    get
        //    {
        //        return _cells.Where(e => !string.IsNullOrEmpty(e.Value.Text)).Select(e => new FilterDescriptor { PropertyName = e.Key.FilterMemberPath, Value = e.Value.Text }).ToList().AsReadOnly();
        //    }
        //}

        protected override void OnLoaded()
        {
            base.OnLoaded();

            CellPadding = new Thickness(4);
            IsEditable = false;
            RowHeaderStyle = filterRowHeaderStyle;
        }

        public Thickness CellPadding { get; set; }

        protected override object GetRowPresenterRecyclingKey()
        {
            return typeof(DataGridFilterRow);
        }

        protected override object GetCellContentRecyclingKey(DataGridColumn column)
        {
            return typeof(SearchBox);
        }

        protected override FrameworkElement CreateCellContent(DataGridColumn column)
        {
            // Add filter to the container
            var filterTextBox = CreateTextBoxFilter(column);
            return filterTextBox;
        }

        private SearchBox CreateTextBoxFilter(DataGridColumn column)
        {
            var filterTextBox = new SearchBox();
            filterTextBox.Tag = column;
            filterTextBox.Text = TextFilterHelper.GetTextFromState(column);
            filterTextBox.SetBinding(SearchBox.BackgroundProperty, column.DataGrid, dg => dg.RowBackground);
            filterTextBox.SetBinding(SearchBox.ForegroundProperty, column.DataGrid, dg => dg.RowForeground);
            filterTextBox.SetBinding(SearchBox.BorderBrushProperty, column.DataGrid, dg => dg.BorderBrush);
            filterTextBox.SetBinding(SearchBox.FocusBrushProperty, column.DataGrid, dg => dg.SelectedBackground);
            filterTextBox.SetBinding(SearchBox.MouseOverBrushProperty, column.DataGrid, dg => dg.MouseOverBrush);
            return filterTextBox;
        }

        protected override void BindCellContent(FrameworkElement cellContent, DataGridColumn column)
        {
            var filterTextBox = (SearchBox)cellContent;
            
            //_cells.Add(column,filterTextBox);
            
            filterTextBox.Margin = CellPadding;
            filterTextBox.Tag = column;
            if (string.IsNullOrEmpty(column.FilterMemberPath))
            {
                filterTextBox.IsEnabled = false;
                filterTextBox.Text = "Not available";
            }
            else
            {
                filterTextBox.Text = "";
                filterTextBox.IsEnabled = true;
            }

            UpdateText(column, filterTextBox);
            // handle TextChanged to apply the filter
            filterTextBox.TextChanged += new System.EventHandler<System.Windows.Controls.TextChangedEventArgs>(filterTextBox_TextChanged);
            //column.FilterStateChanged += new System.EventHandler<PropertyChangedEventArgs<DataGridFilterState>>(column_FilterStateChanged);
            column.DataGrid.FilterChanged += new EventHandler<DataGridFilterChangedEventArgs>(DataGrid_FilterChanged);
        }

        private void OnFilterChanged(DataGridColumn column, string value)
        {
            if (FilterChanged != null)
            {
                //FilterChanged(column, value);
                FilterChanged(this, EventArgs.Empty);
            }
        }
        //public event Action<DataGridColumn, string> FilterChanged;
        public event Action<object, EventArgs> FilterChanged;

        protected override void UnbindCellContent(FrameworkElement cellContent, DataGridColumn column)
        {
            var filterTextBox = (SearchBox)cellContent;

            //_cells.Remove(column);
            
            filterTextBox.TextChanged -= new System.EventHandler<System.Windows.Controls.TextChangedEventArgs>(filterTextBox_TextChanged);
            //column.FilterStateChanged -= new System.EventHandler<PropertyChangedEventArgs<DataGridFilterState>>(column_FilterStateChanged);
            column.DataGrid.FilterChanged -= new EventHandler<DataGridFilterChangedEventArgs>(DataGrid_FilterChanged);
            filterTextBox.Tag = null;
            filterTextBox.Text = "";
            if (filterTextBox.IsFocused)
            {
                DataGrid.Focus();
            }
        }


        void DataGrid_FilterChanged(object sender, DataGridFilterChangedEventArgs e)
        //void column_FilterStateChanged(object sender, PropertyChangedEventArgs<DataGridFilterState> e)
        {
            if (Presenter != null)
            {
                var column = sender as DataGridColumn;
                if (column != null)
                {
                    var cellPresenter = Presenter[column];
                    if (cellPresenter != null)
                    {
                        var filterTextBox = cellPresenter.Content as SearchBox;
                        if (filterTextBox != null)
                        {
                            UpdateText(column, filterTextBox);
                        }
                    }
                }

                //var dataGrid = sender as C1DataGrid;
                //if (dataGrid != null)
                //{
                //    foreach (var filterCollumn in e.NewFilteredColumns)
                //    {
                //        //filterCollumn.Column
                //        foreach (var filter in filterCollumn.Value.FilterInfo)
                //        {
                //            //filter.FilterType
                //            //filter.Value;
                //        }
                //    }
                //}
            }
        }

        private static void UpdateText(DataGridColumn column, SearchBox filterTextBox)
        {
            if (!filterTextBox.IsFocused)
            {
                filterTextBox.Text = TextFilterHelper.GetTextFromState(column);
            }
        }

        void filterTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var filterTextBox = sender as SearchBox;
            var column = filterTextBox.Tag as DataGridColumn;
            if (column != null && filterTextBox.IsEnabled)
            {
                if (column.DataGrid.DataSourceView.CanFilter)
                {
                    var state = TextFilterHelper.BuildFilterState(column, filterTextBox.Text);
                    // apply filter
                    column.DataGrid.FilterBy(column, state, true);
                }

                if (column.DataGrid.DataSourceView is IFilteredCollectionView)
                {
                    var filters = (column.DataGrid.DataSourceView as IFilteredCollectionView).FilterExpressions;

                    var expression = column.Tag as System.Linq.Expressions.Expression<Func<Product, bool>>;
                    if (expression != null)
                    {
                        filters.Remove(expression);
                    }

                    var memberName = column.FilterMemberPath;
                    var filterValue = filterTextBox.Text;

                    if (column is DataGridDateTimeColumn)
                    {
                        expression = _ => (
                            (((DateTime)_[memberName]).Day < 10 ? "0" : "") + ((DateTime)_[memberName]).Day +
                            (((DateTime)_[memberName]).Month < 10 ? ".0" : ".") + ((DateTime)_[memberName]).Month +
                            "." + ((DateTime)_[memberName]).Year
                            ).Contains(filterValue);
                    }
                    else
                    {
                        expression = _ => _[memberName].ToString().Contains(filterValue);
                    }

                    column.Tag = expression;
                    filters.Add(expression);
                }

                OnFilterChanged(column, filterTextBox.Text);
            }
        }

        private static Style filterRowHeaderStyle = null;

        static DataGridFilterRow()
        {
            string rowHeaderStyle = @"<Style xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
                                 xmlns:vsm=""clr-namespace:System.Windows;assembly=System.Windows""
                                 xmlns:c1=""http://schemas.componentone.com/winfx/2006/xaml""
                                 TargetType=""c1:DataGridRowHeaderPresenter"">
            <Setter Property=""Template"">
                <Setter.Value>
                    <ControlTemplate TargetType=""c1:DataGridRowHeaderPresenter"">
                        <Grid x:Name=""RootElement"">
                            <VisualStateManager.VisualStateGroups>
                                <VisualStateGroup x:Name=""CommonStates"">
                                    <VisualState x:Name=""Normal"">
                                        <Storyboard/>
                                    </VisualState>
                                    <VisualState x:Name=""Disabled"">
                                        <Storyboard/>
                                    </VisualState>
                                    <VisualState x:Name=""MouseOver"">
                                        <Storyboard>
                                            <ObjectAnimationUsingKeyFrames Duration=""0"" Storyboard.TargetName=""MouseOverElement"" Storyboard.TargetProperty=""(UIElement.Visibility)"">
                                                <DiscreteObjectKeyFrame KeyTime=""00:00:00"" Value=""Visible""/>
                                            </ObjectAnimationUsingKeyFrames>
                                        </Storyboard>
                                    </VisualState>
                                </VisualStateGroup>
                                <VisualStateGroup x:Name=""SelectionStates"">
                                    <VisualState x:Name=""Selected"">
                                        <Storyboard>
                                            <ObjectAnimationUsingKeyFrames Duration=""0"" Storyboard.TargetName=""SelectedElement"" Storyboard.TargetProperty=""(UIElement.Visibility)"">
                                                <DiscreteObjectKeyFrame KeyTime=""00:00:00"" Value=""Visible""/>
                                            </ObjectAnimationUsingKeyFrames>
                                        </Storyboard>
                                    </VisualState>
                                    <VisualState x:Name=""Editing""/>
                                    <VisualState x:Name=""Unselected""/>
                                </VisualStateGroup>
                                <VisualStateGroup x:Name=""ValidationStates"">
                                    <VisualState x:Name=""Valid""/>
                                    <VisualState x:Name=""InvalidUnfocused""/>
                                    <VisualState x:Name=""InvalidFocused""/>
                                </VisualStateGroup>
                            </VisualStateManager.VisualStateGroups>
                            <c1:C1BrushBuilder x:Name=""BackgroundBrush"" Input=""{TemplateBinding Background}"">
                                <c1:C1BrushBuilder.DesignBrush>
                                    <LinearGradientBrush StartPoint=""0,0.5"" EndPoint=""1,0.5"">
                                        <GradientStop Color=""#FFEEF2F6"" Offset=""0.2""/>
                                        <GradientStop Color=""#FFD0DBE7"" Offset=""1""/>
                                    </LinearGradientBrush>
                                </c1:C1BrushBuilder.DesignBrush>
                                <c1:C1BrushBuilder.DesignColor>
                                    <Color>#FFD1DCE8</Color>
                                </c1:C1BrushBuilder.DesignColor>
                            </c1:C1BrushBuilder>
                            <c1:C1BrushBuilder x:Name=""MouseOverBrush"" Input=""{TemplateBinding MouseOverBrush}"">
                                <c1:C1BrushBuilder.DesignBrush>
                                    <LinearGradientBrush StartPoint=""0,0.5"" EndPoint=""1,0.5"">
                                        <GradientStop Color=""#FFF1FBFF"" Offset=""0.2""/>
                                        <GradientStop Color=""#FFC5E3ED"" Offset=""1""/>
                                    </LinearGradientBrush>
                                </c1:C1BrushBuilder.DesignBrush>
                                <c1:C1BrushBuilder.DesignColor>
                                    <Color>#FFBFE1EA</Color>
                                </c1:C1BrushBuilder.DesignColor>
                            </c1:C1BrushBuilder>
                            <c1:C1BrushBuilder x:Name=""SelectedBackground"" Input=""{TemplateBinding SelectedBackground}"">
                                <c1:C1BrushBuilder.DesignBrush>
                                    <LinearGradientBrush StartPoint=""0,0.5"" EndPoint=""1,0.5"">
                                        <GradientStop Color=""#FFDFF7FF"" Offset=""0.2""/>
                                        <GradientStop Color=""#FFA6D4E4"" Offset=""1""/>
                                    </LinearGradientBrush>
                                </c1:C1BrushBuilder.DesignBrush>
                                <c1:C1BrushBuilder.DesignColor>
                                    <Color>#FF8ED1E2</Color>
                                </c1:C1BrushBuilder.DesignColor>
                            </c1:C1BrushBuilder>
                            <Border Background=""{Binding Output, ElementName=BackgroundBrush}"" BorderBrush=""{TemplateBinding BorderBrush}"" BorderThickness=""0 0 1 1""/>
                            <Border x:Name=""SelectedElement"" Visibility=""Collapsed"" Background=""{Binding Output, ElementName=SelectedBackground}"" BorderThickness=""0 0 1 1""/>
                            <Border x:Name=""MouseOverElement"" Visibility=""Collapsed"" Background=""{Binding Output, ElementName=MouseOverBrush}"" BorderThickness=""0 0 1 1""/>
                            <c1:C1BrushBuilder x:Name=""FilterFillBrush"" Input=""{TemplateBinding Foreground}"" DesignColor=""Black"" >
                                <c1:C1BrushBuilder.DesignBrush>
                                    <LinearGradientBrush EndPoint=""1,0"" StartPoint=""0,0"">
                                        <GradientStop Color=""#FF9D9D9D"" Offset=""0""/>
                                        <GradientStop Color=""#FF878787"" Offset=""0.5""/>
                                        <GradientStop Color=""#FFADADAD"" Offset=""0.5""/>
                                        <GradientStop Color=""#FFAFAFAF"" Offset=""1""/>
                                    </LinearGradientBrush>
                                </c1:C1BrushBuilder.DesignBrush>
                            </c1:C1BrushBuilder>
                            <c1:C1BrushBuilder x:Name=""FilterStrokeBrush"" Input=""{TemplateBinding Foreground}"" DesignColor=""Black"">
                                <c1:C1BrushBuilder.DesignBrush>
                                    <LinearGradientBrush EndPoint=""1,0"" StartPoint=""0,0"">
                                        <GradientStop Color=""#FF353535"" Offset=""0""/>
                                        <GradientStop Color=""#FF474747"" Offset=""0.5""/>
                                        <GradientStop Color=""#FF5E5E5E"" Offset=""0.5""/>
                                        <GradientStop Color=""#FF525252"" Offset=""1""/>
                                    </LinearGradientBrush>
                                </c1:C1BrushBuilder.DesignBrush>
                            </c1:C1BrushBuilder>
                            <Path x:Name=""Filter"" 
                                  Width=""8"" Height=""8"" Stretch=""Fill"" Grid.Column=""1"" Grid.Row=""1"" Grid.RowSpan=""2"" Data=""M3.5,0.5 L4.5,0.5 L4.5,0.5 L7.5,0.5 L7.5,1.5 L7.5,1.5 L4.5,4.3 L4.5,7.5 L3.5,7.5 L3.5,4.3 L0.5,1.5 L0.5,0.5 L3.5,0.5 z""
                                  Fill=""{Binding Output, ElementName=FilterFillBrush}""
                                  Stroke=""{Binding Output, ElementName=FilterStrokeBrush}""
                                  StrokeThickness=""1""
                                  Opacity=""0.6""/>
                            <Rectangle x:Name=""Resize"" Fill=""White"" Cursor=""SizeNS"" Height=""4"" Margin=""0,0,0,-2"" VerticalAlignment=""Bottom"" Opacity=""0"" Grid.ColumnSpan=""3"" Grid.RowSpan=""3""/>
                        </Grid>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>";
            filterRowHeaderStyle = (Style)XamlReader.Load(rowHeaderStyle);
        }
    }
}
