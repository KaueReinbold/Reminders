using System;
using System.Collections.Generic;
using System.Text;

namespace Reminders.Domain.Framework
{
    public static class ExtensionMethods
    {
        public static bool IsNull(this object objectCompare)
        {
            return objectCompare == null;
        }
    }
}
