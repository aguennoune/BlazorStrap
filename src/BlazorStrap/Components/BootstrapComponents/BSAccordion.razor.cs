﻿using BlazorComponentUtilities;
using Microsoft.AspNetCore.Components;

namespace BlazorStrap
{
    public partial class BSAccordion : BlazorStrapBase, IDisposable
    {
        
        [Parameter] public bool IsFlushed { get; set; }
        
        [CascadingParameter] public BSAccordionItem? Parent { get; set; }
        [CascadingParameter] public BSCollapse? CollapseParent { get; set; }


        private string? ClassBuilder => new CssBuilder("accordion")
            .AddClass("accordion-flush", IsFlushed)
            .AddClass(LayoutClass, !string.IsNullOrEmpty(LayoutClass))
            .AddClass(Class, !string.IsNullOrEmpty(Class))
            .Build().ToNullString();

        protected override void OnInitialized()
        {
            if(Parent != null)
                Parent.NestedHandler += NestedHandler;
            if(CollapseParent != null)
                CollapseParent.NestedHandler += NestedHandler;
        }

        private void NestedHandler()
        {
            ChildHandler?.Invoke(null);
        }

        public bool FirstChild()
        {
            return ChildHandler == null;
        }

        public void Invoke(BSAccordionItem sender)
        {
            if(ChildHandler != null)
                ChildHandler(sender);
            
        }

        public void Dispose()
        {
            if (Parent?.NestedHandler != null)
                Parent.NestedHandler -= NestedHandler;
            if (CollapseParent != null)
                CollapseParent.NestedHandler -= NestedHandler;
        }


        internal Action<BSAccordionItem?>? ChildHandler;
        
    }
}
