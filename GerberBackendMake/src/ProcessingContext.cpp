#include <iostream>
#include <fstream>
#include <sstream>
#include <vector>
#include <string>
#include <map>
#include <regex>
#include <stdexcept>
#include <iomanip>
#include <sys/stat.h>
#include <sys/types.h>
#include "zlib.h" 
#include "unzip.h" 
#include "aperture.hpp"
#include "aperture_macro.hpp"
#include "clipper.hpp"
#include "color.hpp"
#include "coord.hpp"
#include "earcut.hpp"
#include "gerber.hpp"
#include "ncdrill.hpp"
#include "netlist.hpp"
#include "obj.hpp"
#include "path.hpp"
#include "pcb.hpp"
#include "plot.hpp"
#include "svg.hpp"
#include "version.hpp"
#include "color_utils.hpp"
#include <stack>
#include <filesystem>
#include "BoardEnumTypes.h"

#if defined(_WIN32)
#include <direct.h>
#include <io.h>
#define GetCurrentDir _getcwd
#define ChangeDir _chdir
#define MakeDir _mkdir
#else
#include <dirent.h>
#include <unistd.h>
#define GetCurrentDir getcwd
#define ChangeDir chdir
#define MakeDir mkdir
#endif

using namespace gerbertools;

