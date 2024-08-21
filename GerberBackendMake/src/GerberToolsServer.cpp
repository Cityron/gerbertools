#include <iostream>
#include <fstream>
#include <sstream>
#include <vector>
#include "obj.hpp"
#include "path.hpp"
#include "pcb.hpp"
#include "plot.hpp"
#include "svg.hpp"
#include "platform.h" 

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
using namespace std;

using namespace gerbertools;

typedef struct {
	char* key;
	char** values;
	size_t value_count;
} KeyValue;

extern "C" {
	__declspec(dllexport) void processPCBFiles(KeyValue* data, size_t data_size, char*& frontSvg, char*& backSvg);
	__declspec(dllexport) void generateMTLAndOBJFiles(KeyValue* data, size_t data_size, char*& mtl, char*& obj);
}

static void FreeKeyValue(KeyValue* kv) {
	if (kv) {
		if (kv->key) {
			free(kv->key);
			kv->key = NULL;
		}

		if (kv->values) {
			for (size_t i = 0; i < kv->value_count; ++i) {
				if (kv->values[i]) {
					free(kv->values[i]);
					kv->values[i] = NULL;
				}
			}
			free(kv->values);
			kv->values = NULL;
		}

		free(kv);
	}
}

extern "C" {

	__declspec(dllexport)  void processPCBFiles(KeyValue* data, size_t data_size, char*& frontSvg, char*& backSvg) {
		std::map<std::string, std::vector<std::string>> files;

		for (size_t i = 0; i < data_size; ++i) {
			KeyValue kv = data[i];
			std::string key(kv.key);

			std::vector<std::string> values;
			for (size_t j = 0; j < kv.value_count; ++j) {
				values.push_back(kv.values[j]);
			}

			files[key] = values;
		}

		std::ostringstream strm_front;
		std::ostringstream strm_back;

		auto pcb = pcb::CircuitBoard::LoadPCB(files);

		pcb.write_svg(strm_front, false, 2.0);
		pcb.write_svg(strm_back, true, 2.0);

		std::string front_str = strm_front.str();
		std::string back_str = strm_back.str();

		frontSvg = _strdup(front_str.c_str());
		backSvg = _strdup(back_str.c_str());

		front_str.clear();
		back_str.clear();
		files.clear();
		strm_front.clear();
		strm_back.clear();
	}

	__declspec(dllexport)  void generateMTLAndOBJFiles(KeyValue* data, size_t data_size, char*& mtl, char*& obj) {
		std::map<std::string, std::vector<std::string>> files;

		for (size_t i = 0; i < data_size; ++i) {
			KeyValue kv = data[i];
			std::string key(kv.key);

			std::vector<std::string> values;
			for (size_t j = 0; j < kv.value_count; ++j) {
				values.push_back(kv.values[j]);
			}

			files[key] = values;
		}

		std::ostringstream strm_mtl;
		std::ostringstream strm_obj;

		auto pcb = pcb::CircuitBoard::LoadPCB(files);

		pcb.generate_mtl_file(strm_mtl);
		pcb.write_obj(strm_obj);

		std::string mtl_str = strm_mtl.str();
		std::string obj_str = strm_obj.str();

		mtl = _strdup(mtl_str.c_str());
		obj = _strdup(obj_str.c_str());

		strm_mtl.clear();
		strm_obj.clear();
		mtl_str.clear();
		obj_str.clear();
		files.clear();
	}
}








