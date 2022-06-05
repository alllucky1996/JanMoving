using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smartstore.Core.Configuration;

namespace Smartstore.Moving.Extensions
{
    public static class CoreExtentions
    {
        public static string DebugString(this Setting setting)
        {
            if (setting == null)
                return null;
            return $"{setting.Name}: {setting.Value}";
        }
    }
}
