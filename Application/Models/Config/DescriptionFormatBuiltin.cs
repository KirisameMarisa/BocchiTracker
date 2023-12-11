using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Config
{
    public class DescriptionFormatBuiltin
    {
        public static readonly string Simple = @"{Description}";

        public static readonly string Chat   = @"<{TicketType}:{Summary}>
{Description}

{Assign}";

        public static readonly string Detail = @"{Description}

Application Information:
- {AppBasicInfo.platform}
- {AppBasicInfo.app_name}
- {AppBasicInfo.args}

PlayerPosition:
- {PlayerPosition.x}
- {PlayerPosition.y}
- {PlayerPosition.z}
- {PlayerPosition.stage}";
    }
}
