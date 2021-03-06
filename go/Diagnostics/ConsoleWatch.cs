// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;

namespace go.Diagnostics
{
    public class ConsoleWatch : IDisposable
    {
        private Stopwatch _watch;
        private Func<string> _doneMessage;

        public ConsoleWatch(string message, Func<string> doneMessage = null)
        {
            Console.WriteLine(message);
            _watch = Stopwatch.StartNew();
            _doneMessage = doneMessage ?? (() => "Done");
        }

        public void Dispose()
        {
            _watch.Stop();
            Console.WriteLine($"{_doneMessage()} in {_watch.Elapsed.TotalSeconds:n0}s");
        }
    }
}
