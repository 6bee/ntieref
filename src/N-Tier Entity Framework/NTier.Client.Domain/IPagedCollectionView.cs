// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;


namespace System.ComponentModel
{
    // Summary:
    //     Provides data for the System.ComponentModel.IPagedCollectionView.PageChanging
    //     event.
    public sealed class PageChangingEventArgs : CancelEventArgs
    {
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.PageChangingEventArgs
        //     class.
        //
        // Parameters:
        //   newPageIndex:
        //     The index of the requested page.
        public PageChangingEventArgs(int newPageIndex)
        {
            NewPageIndex = newPageIndex;
        }

        // Summary:
        //     Gets the index of the requested page.
        //
        // Returns:
        //     The index of the requested page.
        public int NewPageIndex { get; private set; }
    }

    // Summary:
    //     Defines methods and properties that a collection view implements to provide
    //     paging capabilities to a collection.
    public interface IPagedCollectionView
    {
        // Summary:
        //     Gets a value that indicates whether the System.ComponentModel.IPagedCollectionView.PageIndex
        //     value can change.
        //
        // Returns:
        //     true if the System.ComponentModel.IPagedCollectionView.PageIndex value can
        //     change; otherwise, false.
        bool CanChangePage { get; }
        //
        // Summary:
        //     Gets a value that indicates whether the page index is changing.
        //
        // Returns:
        //     true if the page index is changing; otherwise, false.
        bool IsPageChanging { get; }
        //
        // Summary:
        //     Gets the number of known items in the view before paging is applied.
        //
        // Returns:
        //     The number of known items in the view before paging is applied.
        int ItemCount { get; }
        //
        // Summary:
        //     Gets the zero-based index of the current page.
        //
        // Returns:
        //     The zero-based index of the current page.
        int PageIndex { get; }
        //
        // Summary:
        //     Gets or sets the number of items to display on a page.
        //
        // Returns:
        //     The number of items to display on a page.
        int PageSize { get; set; }
        //
        // Summary:
        //     Gets the total number of items in the view before paging is applied.
        //
        // Returns:
        //     The total number of items in the view before paging is applied, or -1 if
        //     the total number is unknown.
        int TotalItemCount { get; }

        // Summary:
        //     When implementing this interface, raise this event after the System.ComponentModel.IPagedCollectionView.PageIndex
        //     has changed.
        event EventHandler<EventArgs> PageChanged;
        //
        // Summary:
        //     When implementing this interface, raise this event before changing the System.ComponentModel.IPagedCollectionView.PageIndex.
        //     The event handler can cancel this event.
        event EventHandler<PageChangingEventArgs> PageChanging;

        // Summary:
        //     Sets the first page as the current page.
        //
        // Returns:
        //     true if the operation was successful; otherwise, false.
        bool MoveToFirstPage();
        //
        // Summary:
        //     Sets the last page as the current page.
        //
        // Returns:
        //     true if the operation was successful; otherwise, false.
        bool MoveToLastPage();
        //
        // Summary:
        //     Moves to the page after the current page.
        //
        // Returns:
        //     true if the operation was successful; otherwise, false.
        bool MoveToNextPage();
        //
        // Summary:
        //     Moves to the page at the specified index.
        //
        // Parameters:
        //   pageIndex:
        //     The index of the page to move to.
        //
        // Returns:
        //     true if the operation was successful; otherwise, false.
        bool MoveToPage(int pageIndex);
        //
        // Summary:
        //     Moves to the page before the current page.
        //
        // Returns:
        //     true if the operation was successful; otherwise, false.
        bool MoveToPreviousPage();
    }
}
