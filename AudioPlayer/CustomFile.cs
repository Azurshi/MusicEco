using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPlayer; 
public static class CustomFile {
    public static bool Exists(string path) {
        return System.IO.File.Exists(path);
    }
}