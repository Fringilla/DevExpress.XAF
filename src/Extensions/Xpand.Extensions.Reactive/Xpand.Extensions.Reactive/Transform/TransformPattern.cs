﻿using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Xpand.Extensions.Reactive.Transform{
    public static partial class Transform{
        public static IObservable<(T sender, TEventArgs e)> TransformPattern<TEventArgs, T>(
            this IObservable<EventPattern<TEventArgs>> source) where TEventArgs : EventArgs{
            return source.Select(pattern => ((T) pattern.Sender, pattern.EventArgs));
        }
    }
}