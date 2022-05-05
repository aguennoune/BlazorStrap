﻿using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace BlazorStrap
{
    public partial class BSDataTable<TValue> : BSTable
    {
        
        [Parameter] public IEnumerable<TValue>? Items { get; set; }
      
        [Parameter] public EventCallback<DataRequest> OnChange { get; set; }
        [Parameter] public Func<DataRequest, Task<(IEnumerable<TValue>,int)>>? FetchItems { get; set; }

        [Parameter] public int TotalItems { get; set; }

        [Parameter] public RenderFragment<TValue>? Body { get; set; }
        [Parameter] public RenderFragment? Header { get; set; }
        [Parameter] public RenderFragment? Footer { get; set; }
        [Parameter, AllowNull] public RenderFragment? NoData { get; set; }
        [Parameter] public bool PaginationTop { get; set; }
        [Parameter] public bool PaginationBottom { get; set; } = true;
        [Parameter] public int RowsPerPage { get; set; } = 20;

        [Parameter] public int StartPage { get; set; } = 1;
        internal Func<string, bool, Task>? OnSort;
        internal Func<string, Task>? OnFilter;
        public int Page { get; set; } = 1;
        private string _sortColumn = "";
        private string _filterColumn = "";
        private string _filterValue = "";
        private bool _desc;

        protected override async Task OnInitializedAsync()
        {
            Page = StartPage;
            //Might be Prerendered
            try
            {
                if (FetchItems != null)
                {
                    var data = await FetchItems.Invoke(DataRequest(Page - 1));
                    Items = data.Item1;
                    TotalItems = data.Item2;
                    await InvokeAsync(StateHasChanged);
                }
            }catch{}
        }

        private DataRequest DataRequest(int page)
        {
            return new DataRequest
            {
                Page = page,
                Descending = _desc,
                Filter = _filterValue,
                FilterColumn = _filterColumn,
                FilterColumnProperty = TypeDescriptor.GetProperties(typeof(TValue)).Find(_filterColumn, false),
                SortColumn = _sortColumn,
                SortColumnProperty = TypeDescriptor.GetProperties(typeof(TValue)).Find(_sortColumn, false)
            };
        }
        private async Task ChangePage(int page)
        {
            if(FetchItems != null)
            {
                var data = await FetchItems.Invoke(DataRequest(page - 1));
                Items = data.Item1;
                TotalItems = data.Item2;
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                await OnChange.InvokeAsync(DataRequest(page - 1));
            }
            
            Page = page;
        }
        private int GetPages()
        {
            var value = 0;
                value = (int)Math.Ceiling(((float)TotalItems / RowsPerPage));
            return value < 1 ? 1 : value;
        }

        public async Task Refresh()
        {
            if (FetchItems != null)
            {
                var data = await FetchItems.Invoke(DataRequest(Page - 1));
                Items = data.Item1;
                TotalItems = data.Item2;
                await InvokeAsync(StateHasChanged);
            }
            await InvokeAsync(StateHasChanged);
        }
        public async Task FilterAsync(string name, string value)
        {
            Page = 1;
            _filterValue = value;
            _filterColumn = name;
            OnFilter?.Invoke(name);
            if (FetchItems != null)
            {
                var data = await FetchItems.Invoke(DataRequest(Page - 1));
                Items = data.Item1;
                TotalItems = data.Item2;
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                await OnChange.InvokeAsync(DataRequest(Page -1));
            }
        }
        public async Task SortAsync(string name)
        {
            _desc = _sortColumn != name ? false : !_desc;
            _sortColumn = name;

            if (FetchItems != null)
            {
                var data = await FetchItems.Invoke(DataRequest(Page - 1));
                Items = data.Item1;
                TotalItems = data.Item2;
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                await OnChange.InvokeAsync(DataRequest(Page - 1));
            }
            
            OnSort?.Invoke(name, _desc);
        }
    }
}