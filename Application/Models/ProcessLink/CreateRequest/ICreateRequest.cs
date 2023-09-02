using BocchiTracker.ModelEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ProcessLink.CreateRequest
{
    public interface ICreateRequest
    {
        byte[]? Create(RequestEventParameterBase inRequest);
    }
}
