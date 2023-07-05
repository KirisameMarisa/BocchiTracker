﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ServiceClientAdapters.Data
{
    public class IdentifierData
    {
        public string? Name { get; set; }

        public string? Id { get; set; }

        public override bool Equals(object? obj)
        {
            var x = obj as IdentifierData;
            if (x == null) return false;
            return x.Name == Name;
        }

        public override int GetHashCode()
        {
            if (Name == null) return 0;
            return Name.GetHashCode();
        }
    }
}
