﻿using System;

namespace UtilityWpf.ViewModel

{
    public class ViewContainer
    {
        public ViewContainer(string title, object content)
        {
            Title = title;
            Content = content;
        }

        public Guid Id { get; } = Guid.NewGuid();
        public string Title { get; }
        public object Content { get; }
    }
}