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
	std::string mtlPath = outputDir + "/output.mtl";
	std::string objPath = outputDir + "/output.obj";
	std::string obj = "output.mtl";
	pcb::CircuitBoard pcb = pcb::CircuitBoard::LoadPCB(outputDir);
	std::ostringstream strm_front;
	std::ostringstream strm_back;
	std::ostringstream strm_mtl;
	std::ostringstream strm_obj;

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
	pcb.write_svg(strm_front, false, 2.0);

	std::ofstream file_front(frontSVGPath);
	if (file_front.is_open()) {
		file_front << strm_front.str();
		file_front.close();
	}

	strm_front.clear();

	auto end3 = std::chrono::system_clock::now();
	std::chrono::duration<double> elapsed_seconds3 = end3 - end2;

	std::cout
		<< "frontSVGPath: " << elapsed_seconds3.count() << "s"
		<< std::endl;
	pcb.write_svg(strm_back, true, 2.0);

	std::ofstream file_back(backSVGPath);
	if (file_back.is_open()) {
		file_back << strm_back.str();
		file_back.close();
	}

	strm_back.clear();

	auto end4 = std::chrono::system_clock::now();
	std::chrono::duration<double> elapsed_seconds4 = end4 - end3;

	std::cout
		<< "backSVGPath: " << elapsed_seconds4.count() << "s"
		<< std::endl;

	pcb.generate_mtl_file(strm_mtl);
	std::ofstream file_mtl(mtlPath);
	if (file_mtl.is_open()) {
		file_mtl << strm_mtl.str();
		file_mtl.close();
	}


	pcb.write_obj(strm_obj);

	std::ofstream file_obj(objPath);
	if (file_obj.is_open()) {
		file_obj << "mtllib " << obj << "\n";
		file_obj << strm_obj.str();
		file_obj.close();
	}

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


