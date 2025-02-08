using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicEco.Common.Value;
public static class System {
    public static bool AppRunning = true;
    public static bool EventSystemBlockPublish = false;
    public static bool EventSystemBlockConnect = false;
    public static bool BlockUpdate = false;
}