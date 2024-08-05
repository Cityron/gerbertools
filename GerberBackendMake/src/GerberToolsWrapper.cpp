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

// Для системных вызовов для работы с директориями
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

void DetermineBoardSideAndLayer(const std::string& gerberfile, BoardSide& Side, BoardLayer& Layer) {
	Side = BoardSide::Unknown;
	Layer = BoardLayer::Unknown;
	std::string filename = gerberfile.substr(gerberfile.find_last_of("/\\") + 1);
	std::string ext = filename.substr(filename.find_last_of('.') + 1);
	std::transform(ext.begin(), ext.end(), ext.begin(), ::tolower);

	if (ext == "art") {
		std::string baseName = filename.substr(0, filename.find_last_of('.'));
		std::transform(baseName.begin(), baseName.end(), baseName.begin(), ::toupper);

		if (baseName == "PMT") { Side = BoardSide::Top; Layer = BoardLayer::Paste; }
		else if (baseName == "PMB") { Side = BoardSide::Bottom; Layer = BoardLayer::Paste; }
		else if (baseName == "TOP") { Side = BoardSide::Top; Layer = BoardLayer::Copper; }
		else if (baseName == "BOTTOM") { Side = BoardSide::Bottom; Layer = BoardLayer::Copper; }
		else if (baseName == "SMBOT") { Side = BoardSide::Bottom; Layer = BoardLayer::SolderMask; }
		else if (baseName == "SMTOP") { Side = BoardSide::Top; Layer = BoardLayer::SolderMask; }
		else if (baseName == "SSBOT") { Side = BoardSide::Bottom; Layer = BoardLayer::Silk; }
		else if (baseName == "SSTOP") { Side = BoardSide::Top; Layer = BoardLayer::Silk; }
		else if (baseName == "DRILLING") { Side = BoardSide::Both; Layer = BoardLayer::Drill; }
	}
	else if (ext == "slices") { Side = BoardSide::Both; Layer = BoardLayer::Utility; }
	else if (ext == "copper_bottom") { Side = BoardSide::Bottom; Layer = BoardLayer::Copper; }
	else if (ext == "copper_top") { Side = BoardSide::Top; Layer = BoardLayer::Copper; }
	else if (ext == "silk_bottom") { Side = BoardSide::Bottom; Layer = BoardLayer::Silk; }
	else if (ext == "silk_top") { Side = BoardSide::Top; Layer = BoardLayer::Silk; }
	else if (ext == "paste_bottom") { Side = BoardSide::Bottom; Layer = BoardLayer::Paste; }
	else if (ext == "paste_top") { Side = BoardSide::Top; Layer = BoardLayer::Paste; }
	else if (ext == "soldermask_bottom") { Side = BoardSide::Bottom; Layer = BoardLayer::SolderMask; }
	else if (ext == "soldermask_top") { Side = BoardSide::Top; Layer = BoardLayer::SolderMask; }
	else if (ext == "drill_both") { Side = BoardSide::Both; Layer = BoardLayer::Drill; }
	else if (ext == "outline_both") { Side = BoardSide::Both; Layer = BoardLayer::Outline; }
	else if (ext == "png") {
		Side = BoardSide::Both;
		Layer = BoardLayer::Silk;
	}
	else if (ext == "assemblytop") {
		Layer = BoardLayer::Assembly;
		Side = BoardSide::Top;
	}
	else if (ext == "assemblybottom") {
		Layer = BoardLayer::Assembly;
		Side = BoardSide::Bottom;
	}
	else if (ext == "gbr") {
		std::string baseName = filename.substr(0, filename.find_last_of('.'));
		std::transform(baseName.begin(), baseName.end(), baseName.begin(), ::tolower);

		if (baseName == "profile" || baseName == "boardoutline" || baseName == "outline" || baseName == "board") {
			Side = BoardSide::Both;
			Layer = BoardLayer::Outline;
		}
		else if (baseName == "copper_bottom" || baseName == "bottom") {
			Side = BoardSide::Bottom;
			Layer = BoardLayer::Copper;
		}
		else if (baseName == "soldermask_bottom" || baseName == "bottommask") {
			Side = BoardSide::Bottom;
			Layer = BoardLayer::SolderMask;
		}
		else if (baseName == "solderpaste_bottom" || baseName == "bottompaste") {
			Side = BoardSide::Bottom;
			Layer = BoardLayer::Paste;
		}
		else if (baseName == "silkscreen_bottom" || baseName == "bottomsilk") {
			Side = BoardSide::Bottom;
			Layer = BoardLayer::Silk;
		}
		else if (baseName == "copper_top" || baseName == "top") {
			Side = BoardSide::Top;
			Layer = BoardLayer::Copper;
		}
		else if (baseName == "soldermask_top" || baseName == "topmask") {
			Side = BoardSide::Top;
			Layer = BoardLayer::SolderMask;
		}
		else if (baseName == "solderpaste_top" || baseName == "toppaste") {
			Side = BoardSide::Top;
			Layer = BoardLayer::Paste;
		}
		else if (baseName == "silkscreen_top" || baseName == "topsilk") {
			Side = BoardSide::Top;
			Layer = BoardLayer::Silk;
		}
		else if (baseName == "inner1") {
			Side = BoardSide::Internal1;
			Layer = BoardLayer::Copper;
		}
		else if (baseName == "inner2") {
			Side = BoardSide::Internal2;
			Layer = BoardLayer::Copper;
		}
		else {
			std::string lcase = gerberfile;
			std::transform(lcase.begin(), lcase.end(), lcase.begin(), ::tolower);
			if (lcase.find("board outline") != std::string::npos) { Side = BoardSide::Both; Layer = BoardLayer::Outline; }
			if (lcase.find("copper bottom") != std::string::npos) { Side = BoardSide::Bottom; Layer = BoardLayer::Copper; }
			if (lcase.find("silkscreen bottom") != std::string::npos) { Side = BoardSide::Bottom; Layer = BoardLayer::Silk; }
			if (lcase.find("copper top") != std::string::npos) { Side = BoardSide::Top; Layer = BoardLayer::Copper; }
			if (lcase.find("silkscreen top") != std::string::npos) { Side = BoardSide::Top; Layer = BoardLayer::Silk; }
			if (lcase.find("solder mask bottom") != std::string::npos) { Side = BoardSide::Bottom; Layer = BoardLayer::SolderMask; }
			if (lcase.find("solder mask top") != std::string::npos) { Side = BoardSide::Top; Layer = BoardLayer::SolderMask; }
			if (lcase.find("drill-copper top-copper bottom") != std::string::npos) { Side = BoardSide::Both; Layer = BoardLayer::Drill; }
			if (lcase.find("outline") != std::string::npos) { Side = BoardSide::Both; Layer = BoardLayer::Outline; }
			if (lcase.find("-edge_cuts") != std::string::npos) { Side = BoardSide::Both; Layer = BoardLayer::Outline; }
			if (lcase.find("-b_cu") != std::string::npos) { Side = BoardSide::Bottom; Layer = BoardLayer::Copper; }
			if (lcase.find("-f_cu") != std::string::npos) { Side = BoardSide::Top; Layer = BoardLayer::Copper; }
			if (lcase.find("-b_silks") != std::string::npos) { Side = BoardSide::Bottom; Layer = BoardLayer::Silk; }
			if (lcase.find("-f_silks") != std::string::npos) { Side = BoardSide::Top; Layer = BoardLayer::Silk; }
			if (lcase.find("-b_mask") != std::string::npos) { Side = BoardSide::Bottom; Layer = BoardLayer::SolderMask; }
			if (lcase.find("-f_mask") != std::string::npos) { Side = BoardSide::Top; Layer = BoardLayer::SolderMask; }
			if (lcase.find("-b_paste") != std::string::npos) { Side = BoardSide::Bottom; Layer = BoardLayer::Paste; }
			if (lcase.find("-f_paste") != std::string::npos) { Side = BoardSide::Top; Layer = BoardLayer::Paste; }
		}
	}
	else if (ext == "ger") {
		std::string l = gerberfile;
		std::transform(l.begin(), l.end(), l.begin(), ::tolower);
		std::vector<BoardSet> bs = {
			{ ".topsoldermask", BoardSide::Top, BoardLayer::SolderMask },
			{ ".topsilkscreen", BoardSide::Top, BoardLayer::Silk },
			{ ".toplayer", BoardSide::Top, BoardLayer::Copper },
			{ ".tcream", BoardSide::Top, BoardLayer::Paste },
			{ ".boardoutline", BoardSide::Both, BoardLayer::Outline },
			{ ".bcream", BoardSide::Bottom, BoardLayer::Paste },
			{ ".bottomsoldermask", BoardSide::Bottom, BoardLayer::SolderMask },
			{ ".bottomsilkscreen", BoardSide::Bottom, BoardLayer::Silk },
			{ ".bottomlayer", BoardSide::Bottom, BoardLayer::Copper },
			{ ".internalplane1", BoardSide::Internal1, BoardLayer::Copper },
			{ ".internalplane2", BoardSide::Internal2, BoardLayer::Copper }
		};

		for (const auto& a : bs) {
			if (l.find(a.name) != std::string::npos) {
				Side = a.side;
				Layer = a.layer;
				break;
			}
		}
	}
	else if (ext == "gml") { Side = BoardSide::Both; Layer = BoardLayer::Mill; }
	else if (ext == "fabrd" || ext == "oln" || ext == "gko" || ext == "gm1") { Side = BoardSide::Both; Layer = BoardLayer::Outline; }
	else if (ext == "l2" || ext == "g1l" || ext == "gl1" || ext == "g1") { Side = BoardSide::Internal1; Layer = BoardLayer::Copper; }
	else if (ext == "adtop") { Side = BoardSide::Top; Layer = BoardLayer::Assembly; }
	else if (ext == "adbottom") { Side = BoardSide::Bottom; Layer = BoardLayer::Assembly; }
	else if (ext == "notes") { Side = BoardSide::Both; Layer = BoardLayer::Notes; }
	else if (ext == "l3" || ext == "gl2" || ext == "g2l" || ext == "g2") { Side = BoardSide::Internal2; Layer = BoardLayer::Copper; }
	else if (ext == "gl3" || ext == "g3l" || ext == "g3") { Side = BoardSide::Internal3; Layer = BoardLayer::Copper; }
	else if (ext == "gl4" || ext == "g4l" || ext == "g4") { Side = BoardSide::Internal4; Layer = BoardLayer::Copper; }
	else if (ext == "gl5" || ext == "g5l" || ext == "g5") { Side = BoardSide::Internal5; Layer = BoardLayer::Copper; }
	else if (ext == "gl6" || ext == "g6l" || ext == "g6") { Side = BoardSide::Internal6; Layer = BoardLayer::Copper; }
	else if (ext == "gl7" || ext == "g7l" || ext == "g7") { Side = BoardSide::Internal7; Layer = BoardLayer::Copper; }
	else if (ext == "gl8" || ext == "g8l" || ext == "g8") { Side = BoardSide::Internal8; Layer = BoardLayer::Copper; }
	else if (ext == "gl9" || ext == "g9l" || ext == "g9") { Side = BoardSide::Internal9; Layer = BoardLayer::Copper; }
	else if (ext == "gl10" || ext == "g10l" || ext == "g10") { Side = BoardSide::Internal10; Layer = BoardLayer::Copper; }
	else if (ext == "gl11" || ext == "g11l" || ext == "g11") { Side = BoardSide::Internal11; Layer = BoardLayer::Copper; }
	else if (ext == "gl12" || ext == "g12l" || ext == "g12") { Side = BoardSide::Internal12; Layer = BoardLayer::Copper; }
	else if (ext == "gl13" || ext == "g13l" || ext == "g13") { Side = BoardSide::Internal13; Layer = BoardLayer::Copper; }
	else if (ext == "gl14" || ext == "g14l" || ext == "g14") { Side = BoardSide::Internal14; Layer = BoardLayer::Copper; }
	else if (ext == "gl15" || ext == "g15l" || ext == "g15") { Side = BoardSide::Internal15; Layer = BoardLayer::Copper; }
	else if (ext == "gl16" || ext == "g16l" || ext == "g16") { Side = BoardSide::Internal16; Layer = BoardLayer::Copper; }
	else if (ext == "gl17" || ext == "g17l" || ext == "g17") { Side = BoardSide::Internal17; Layer = BoardLayer::Copper; }
	else if (ext == "gl18" || ext == "g18l" || ext == "g18") { Side = BoardSide::Internal18; Layer = BoardLayer::Copper; }
	else if (ext == "gl19" || ext == "g19l" || ext == "g19") { Side = BoardSide::Internal19; Layer = BoardLayer::Copper; }
	else if (ext == "gl20" || ext == "g20l" || ext == "g20") { Side = BoardSide::Internal20; Layer = BoardLayer::Copper; }
	else if (ext == "l4" || ext == "gbl" || ext == "l2m") { Side = BoardSide::Bottom; Layer = BoardLayer::Copper; }
	else if (ext == "l1" || ext == "l1m" || ext == "gtl") { Side = BoardSide::Top; Layer = BoardLayer::Copper; }
	else if (ext == "gbp" || ext == "spbottom") { Side = BoardSide::Bottom; Layer = BoardLayer::Paste; }
	else if (ext == "gtp" || ext == "sptop") { Side = BoardSide::Top; Layer = BoardLayer::Paste; }
	else if (ext == "gbo" || ext == "ss2" || ext == "ssbottom") { Side = BoardSide::Bottom; Layer = BoardLayer::Silk; }
	else if (ext == "gto" || ext == "ss1" || ext == "sstop") { Side = BoardSide::Top; Layer = BoardLayer::Silk; }
	else if (ext == "gbs" || ext == "sm2" || ext == "smbottom") { Side = BoardSide::Bottom; Layer = BoardLayer::SolderMask; }
	else if (ext == "gts" || ext == "sm1" || ext == "smtop") { Side = BoardSide::Top; Layer = BoardLayer::SolderMask; }
	else if (ext == "outline" || ext == "gb3") { Side = BoardSide::Both; Layer = BoardLayer::Outline; }
	else if (ext == "gt3") { Side = BoardSide::Both; Layer = BoardLayer::Outline; }
	else if (ext == "top") { Side = BoardSide::Top; Layer = BoardLayer::Copper; }
	else if (ext == "bottom" || ext == "bot") { Side = BoardSide::Bottom; Layer = BoardLayer::Copper; }
	else if (ext == "smb") { Side = BoardSide::Bottom; Layer = BoardLayer::SolderMask; }
	else if (ext == "smt") { Side = BoardSide::Top; Layer = BoardLayer::SolderMask; }
	else if (ext == "slk" || ext == "sst") { Side = BoardSide::Top; Layer = BoardLayer::Silk; }
	else if (ext == "bsk" || ext == "ssb") { Side = BoardSide::Bottom; Layer = BoardLayer::Silk; }
	else if (ext == "spt") { Side = BoardSide::Top; Layer = BoardLayer::Paste; }
	else if (ext == "spb") { Side = BoardSide::Bottom; Layer = BoardLayer::Paste; }
	else if (ext == "drill_top_bottom" || ext == "drl" || ext == "drill" || ext == "drillnpt" || ext == "rou" || ext == "sco") {
		Side = BoardSide::Both;
		Layer = BoardLayer::Drill;
	}
}

BoardFileType FindFileTypeFromStream(std::ifstream& file, std::string filename) {
	std::transform(filename.begin(), filename.end(), filename.begin(), ::tolower);
	std::vector<std::string> unsupported = { "config", "exe", "dll", "png", "zip", "gif", "jpeg", "doc", "docx", "jpg", "bmp" };
	std::string ext = filename.substr(filename.find_last_of(".") + 1);

	if (std::find(unsupported.begin(), unsupported.end(), ext) != unsupported.end()) {
		return BoardFileType::Unsupported;
	};

	std::string line;
	while (std::getline(file, line)) {
		if (line.find("%FS") != std::string::npos) return BoardFileType::Gerber;
		if (line.find("M48") != std::string::npos) return BoardFileType::Drill;
	}

	return BoardFileType::Unsupported;
}

bool CreateDirectory(const std::string& path) {
#if defined(_WIN32)
	int ret = _mkdir(path.c_str());
#else
	mode_t mode = 0755;
	int ret = mkdir(path.c_str(), mode);
#endif
	if (ret == 0 || errno == EEXIST) {
		return true;
	}
	else {
		std::cerr << "Не удалось создать директорию: " << path << std::endl;
		return false;
	}
}

bool UnzipFile(const std::string& zipFilePath, const std::string& outputDir) {
	unzFile zipfile = unzOpen(zipFilePath.c_str());
	if (!zipfile) {
		std::cerr << "Не удалось открыть zip архив." << std::endl;
		return false;
	}

	if (unzGoToFirstFile(zipfile) != UNZ_OK) {
		std::cerr << "Не удалось перейти к первому файлу в zip архиве." << std::endl;
		unzClose(zipfile);
		return false;
	}

	do {
		char filename_inzip[256];
		unz_file_info file_info;
		if (unzGetCurrentFileInfo(zipfile, &file_info, filename_inzip, sizeof(filename_inzip), NULL, 0, NULL, 0) != UNZ_OK) {
			std::cerr << "Не удалось получить информацию о файле." << std::endl;
			unzClose(zipfile);
			return false;
		}

		std::string full_path = outputDir + "/" + filename_inzip;
		const size_t last_slash_idx = full_path.rfind('/');
		if (std::string::npos != last_slash_idx) {
			std::string dir = full_path.substr(0, last_slash_idx);
			if (!CreateDirectory(dir)) {
				std::cerr << "Не удалось создать директорию: " << dir << std::endl;
				unzClose(zipfile);
				return false;
			}
		}

		if (unzOpenCurrentFile(zipfile) != UNZ_OK) {
			std::cerr << "Не удалось открыть текущий файл." << std::endl;
			unzClose(zipfile);
			return false;
		}

		FILE* fout;
		if (fopen_s(&fout, full_path.c_str(), "wb") != 0) {
			std::cerr << "Не удалось создать файл для записи." << std::endl;
			unzCloseCurrentFile(zipfile);
			unzClose(zipfile);
			return false;
		}

		int error = UNZ_OK;
		do {
			char buffer[8192];
			error = unzReadCurrentFile(zipfile, buffer, sizeof(buffer));
			if (error < 0) {
				std::cerr << "Ошибка при чтении файла." << std::endl;
				fclose(fout);
				unzCloseCurrentFile(zipfile);
				unzClose(zipfile);
				return false;
			}

			if (error > 0) {
				fwrite(buffer, error, 1, fout);
			}
		} while (error > 0);

		fclose(fout);
		unzCloseCurrentFile(zipfile);

	} while (unzGoToNextFile(zipfile) == UNZ_OK);

	unzClose(zipfile);
	return true;
}

std::vector<std::string> ListFiles(const std::string& directory) {
	std::vector<std::string> files;

	for (const auto& entry : std::filesystem::recursive_directory_iterator(directory)) {
		if (entry.is_regular_file()) {
			files.push_back(std::filesystem::absolute(entry.path()).string());
		}
	}

	return files;
}

std::map<std::string, std::vector<std::string>> IdentifyGerberFiles(const std::string& directory) {
	std::map<std::string, std::vector<std::string>> gerberFiles;
	std::vector<std::string> files = ListFiles(directory);


	for (const auto& file : files) {
		BoardFileType gerber = BoardFileType::Unsupported;
		std::ifstream f1(file);
		gerber = FindFileTypeFromStream(f1, file);
		if (BoardFileType::Gerber == gerber) {
			BoardSide Side;
			BoardLayer Layer;
			DetermineBoardSideAndLayer(file, Side, Layer);

			size_t pos = file.rfind('\\');
			pos = file.rfind('\\', pos - 1);
			std::string result = file.substr(pos);

			if (BoardLayer::Copper == Layer && BoardSide::Top == Side) {
				gerberFiles["topCopper"].push_back(result);
			}
			else if (BoardLayer::Copper == Layer && BoardSide::Bottom == Side) {
				gerberFiles["bottomCopper"].push_back(result);
			}
			else if (BoardLayer::SolderMask == Layer && BoardSide::Top == Side) {
				gerberFiles["topMask"].push_back(result);
			}
			else if (BoardLayer::SolderMask == Layer && BoardSide::Bottom == Side) {
				gerberFiles["bottomMask"].push_back(result);
			}
			else if (BoardLayer::Silk == Layer && BoardSide::Top == Side) {
				gerberFiles["topSilk"].push_back(result);
			}
			else if (BoardLayer::Silk == Layer && BoardSide::Bottom == Side) {
				gerberFiles["bottomSilk"].push_back(result);
			}
			else if (BoardLayer::Drill == Layer && BoardSide::Both == Side) {
				gerberFiles["drill"].push_back(result);
			}
			else if (BoardLayer::Mill == Layer && BoardSide::Both == Side) {
				gerberFiles["mill"].push_back(result);
			}
			else if (BoardLayer::Outline == Layer && BoardSide::Both == Side) {
				gerberFiles["outline"].push_back(result);
			}
		}
		else if (BoardFileType::Drill == gerber) {
			size_t pos = file.rfind('\\');
			pos = file.rfind('\\', pos - 1);
			std::string result = file.substr(pos);
			gerberFiles["drill"].push_back(result);
		}
		f1.close();
	}
	return gerberFiles;

}

color::Color CreateColor(float r, float g, float b, float a) {
	color::Color color;
	color.r = r;
	color.g = g;
	color.b = b;
	color.a = a;
	return color;
}


pcb::CircuitBoard LoadPCB(const std::string& directory) {
	auto start = std::chrono::system_clock::now();


	std::cout
		<< "elapsed time: " << start.max << "s"
		<< std::endl;
	auto gerberFiles = IdentifyGerberFiles(directory);
	if (gerberFiles.empty()) {
		throw std::runtime_error("No gerber file");
	}
	auto end = std::chrono::system_clock::now();
	std::chrono::duration<double> elapsed_seconds1 = end - start;

	std::cout
		<< "elapsed time1: " << elapsed_seconds1.count() << "s"
		<< std::endl;

	std::string basename = directory;
	std::string outline = gerberFiles["outline"].front();
	std::vector<std::string> drill = gerberFiles["drill"];
	std::string drill_nonplated = "";
	std::string mill = "";
	double plating_thickness = 0.5 * pcb::COPPER_OZ;
	// Создание объекта CircuitBoard
	pcb::CircuitBoard board(basename, outline, drill, drill_nonplated, mill, plating_thickness);

	// Добавление слоев

	if (!gerberFiles["bottomMask"].empty()) {
		board.add_mask_layer(gerberFiles["bottomMask"].front(), gerberFiles["bottomSilk"].front());
	}
	if (!gerberFiles["bottomCopper"].empty()) {
		board.add_copper_layer(gerberFiles["bottomCopper"].front(), pcb::COPPER_OZ);
	}
	board.add_substrate_layer(1.5);
	if (!gerberFiles["topCopper"].empty()) {
		board.add_copper_layer(gerberFiles["topCopper"].front(), pcb::COPPER_OZ);
	}
	if (!gerberFiles["topMask"].empty()) {
		board.add_mask_layer(gerberFiles["topMask"].front(), gerberFiles["topSilk"].front());
	}

	board.add_surface_finish();
	gerberFiles.clear();


	return board;
}

int main(int argc, char* argv[]) {
	auto start = std::chrono::system_clock::now();
	auto end = std::chrono::system_clock::now();

	std::chrono::duration<double> elapsed_seconds = end - start;
	std::time_t end_time = std::chrono::system_clock::to_time_t(end);

	std::cout
		<< "elapsed time: " << start.max << "s"
		<< std::endl;

	std::string zipFilePath = argv[1];
	std::string outputDir = argv[2];

	// Создаем директорию для выходных данных, если она не существует
	if (!CreateDirectory(outputDir)) {
		throw std::runtime_error("Не удалось создать директорию для выходных данных.");
	}

	// Распаковка zip архива
	if (!UnzipFile(zipFilePath, outputDir)) {
		throw std::runtime_error("Не удалось распаковать zip архив.");
	}

	auto end1 = std::chrono::system_clock::now();
	std::chrono::duration<double> elapsed_seconds1 = end1 - start;

	std::cout
		<< "zip: " << elapsed_seconds1.count() << "s"
		<< std::endl;

	// Рендеринг SVG для передней и задней стороны платы
	std::string frontSVGPath = outputDir + "/front.svg";
	std::string backSVGPath = outputDir + "/back.svg";
	pcb::CircuitBoard pcb = LoadPCB(outputDir);
	std::ostringstream strm_front;
	std::ostringstream strm_back;

	auto end2 = std::chrono::system_clock::now();
	std::chrono::duration<double> elapsed_seconds2 = end2 - end1;

	std::cout
		<< "LoadPCB: " << elapsed_seconds2.count() << "s"
		<< std::endl;

	auto coordinates = pcb.get_bounds();

	std::cout << std::fixed << std::setprecision(3)
		<< round(coord::Format::to_mm(coordinates.right) - coord::Format::to_mm(coordinates.left)) << ","
		<< round(coord::Format::to_mm(coordinates.top) - coord::Format::to_mm(coordinates.bottom)) << std::endl;

	// Запись SVG файла
	std::string result_write_svg_front = pcb.write_svg(strm_front, false, 2.0);

	std::ofstream file_front(frontSVGPath);
	if (file_front.is_open()) {
		file_front << result_write_svg_front;
		file_front.close();
	}

	strm_front.clear();

	auto end3 = std::chrono::system_clock::now();
	std::chrono::duration<double> elapsed_seconds3 = end3 - end2;

	std::cout
		<< "frontSVGPath: " << elapsed_seconds3.count() << "s"
		<< std::endl;
	std::string result_write_svg_back = pcb.write_svg(strm_back, true, 2.0);

	std::ofstream file_back(backSVGPath);
	if (file_back.is_open()) {
		file_back << result_write_svg_back;
		file_back.close();
	}

	strm_back.clear();

	auto end4 = std::chrono::system_clock::now();
	std::chrono::duration<double> elapsed_seconds4 = end4 - end3;

	std::cout
		<< "backSVGPath: " << elapsed_seconds4.count() << "s"
		<< std::endl;
	pcb.write_obj(outputDir + "/output.obj");
	auto end5 = std::chrono::system_clock::now();
	std::chrono::duration<double> elapsed_seconds5 = end5 - end4;

	std::cout
		<< "obj: " << elapsed_seconds5.count() << "s"
		<< std::endl;

	auto end6 = std::chrono::system_clock::now();
	std::chrono::duration<double> elapsed_seconds6 = end6 - start;

	std::cout
		<< "obj: " << elapsed_seconds6.count() << "s"
		<< std::endl;
	std::cout << "All task complited" << std::endl;

	return 0;
}


