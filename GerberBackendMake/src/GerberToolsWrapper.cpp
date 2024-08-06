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
#include "ProcessingContext.cpp"

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


void createGzipFromVector(const std::vector<std::pair<std::string, std::string>>& dataVec, const std::string& filename) {
	gzFile gzfile = gzopen(filename.c_str(), "wb");
	if (!gzfile) {
		throw std::runtime_error("Не удалось открыть файл для записи gzip: " + filename);
	}

	for (const auto& [data, fileName] : dataVec) {
		gzprintf(gzfile, "%s\n", fileName.c_str());
		if (gzwrite(gzfile, data.data(), data.size()) == 0) {
			gzclose(gzfile);
			throw std::runtime_error("Ошибка записи в gzip файл: " + filename);
		}
	}
	gzclose(gzfile);
}

bool UnzipFile(const std::string& zipFilePath, std::vector<pcb::GerberFile>& filesGerber) {
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

	pcb::FileType type;

	do {
		char filename_inzip[256];
		unz_file_info file_info;
		if (unzGetCurrentFileInfo(zipfile, &file_info, filename_inzip, sizeof(filename_inzip), NULL, 0, NULL, 0) != UNZ_OK) {
			std::cerr << "Не удалось получить информацию о файле." << std::endl;
			unzClose(zipfile);
			return false;
		}

		if (unzOpenCurrentFile(zipfile) != UNZ_OK) {
			std::cerr << "Не удалось открыть текущий файл." << std::endl;
			unzClose(zipfile);
			return false;
		}

		std::string file_data;
		file_data.resize(file_info.uncompressed_size);

		int error = unzReadCurrentFile(zipfile, &file_data[0], file_info.uncompressed_size);
		if (error < 0) {
			std::cerr << "Ошибка при чтении файла." << std::endl;
			unzCloseCurrentFile(zipfile);
			unzClose(zipfile);
			return false;
		}

		std::string filename_str(filename_inzip);
		std::string file_name = filename_str.substr(filename_str.find_last_of("/\\") + 1);

		std::istringstream file_stream(file_data);
		auto filetype = type.FindFileTypeFromStream(file_stream, file_name);

		if (filetype == BoardFileType::Gerber) {
			pcb::GerberFile gerberFile(file_name, std::move(file_data), BoardFileType::Gerber);
			filesGerber.push_back(gerberFile);
		}
		else if (filetype == BoardFileType::Drill) {
			pcb::GerberFile gerberFile(file_name, std::move(file_data), BoardFileType::Drill);
			filesGerber.push_back(gerberFile);
		}

		unzCloseCurrentFile(zipfile);

	} while (unzGoToNextFile(zipfile) == UNZ_OK);

	unzClose(zipfile);

	return true;
}

void processPCBFiles(const std::string& zipFilePath, const std::string& outputDir) {
	std::vector<pcb::GerberFile> filesGerber;
	std::vector<std::pair<std::string, std::string>> svgData;
	std::string gzipFilename = outputDir + "/svg_data.gz";

	std::ostringstream strm_front;
	std::ostringstream strm_back;

	if (!UnzipFile(zipFilePath, filesGerber)) {
		throw std::runtime_error("Не удалось распаковать zip архив.");
	}

	auto pcb = pcb::CircuitBoard::LoadPCB(filesGerber);

	pcb.write_svg(strm_front, false, 2.0);
	pcb.write_svg(strm_back, true, 2.0);

	svgData.push_back({ strm_front.str(), "front.svg" });
	svgData.push_back({ strm_back.str(), "back.svg" });

	createGzipFromVector(svgData, gzipFilename);
}

void generateMTLAndOBJFiles(const std::string& zipFilePath, const std::string& outputDir) {
	std::vector<pcb::GerberFile> filesGerber;
	std::vector<std::pair<std::string, std::string>> objData;
	std::ostringstream strm_mtl;
	std::ostringstream strm_obj;

	if (!UnzipFile(zipFilePath, filesGerber)) {
		throw std::runtime_error("Не удалось распаковать zip архив.");
	}

	auto pcb = pcb::CircuitBoard::LoadPCB(filesGerber);

	pcb.generate_mtl_file(strm_mtl);
	pcb.write_obj(strm_obj);

	objData.push_back({ strm_mtl.str(), "output.mtl" });
	objData.push_back({ strm_obj.str(), "output.obj" });

	std::string gzipFileName = outputDir + "/output_files.gz";
	createGzipFromVector(objData, gzipFileName);
}

int main(int argc, char* argv[]) {
	if (argc < 3) {
		std::cerr << "Usage: " << argv[0] << " <zipFilePath> <outputDir>" << std::endl;
		return 1;
	}

	processPCBFiles(argv[1], argv[2]);

	generateMTLAndOBJFiles(argv[1], argv[2]);


	std::cout << "All tasks completed" << std::endl;

	return 0;
}

