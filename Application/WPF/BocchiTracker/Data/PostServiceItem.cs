using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Data
{
    public class PostServiceItem
    {
        public ReactiveProperty<string> Name { get; set; }

        public ReactiveProperty<bool> IsSelected { get; set; }
    }
}
