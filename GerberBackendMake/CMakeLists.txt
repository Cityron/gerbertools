cmake_minimum_required(VERSION 3.8)
set(CMAKE_SUPPRESS_REGENERATION TRUE)
project(GerberToolsWrapper LANGUAGES CXX C)

# Установить стандарт C++
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

# Указать пути к исходным и заголовочным файлам
set(GERBERTOOLS_SRC_DIR "${CMAKE_SOURCE_DIR}/gerbertools/src")
set(GERBERTOOLS_INCLUDE_DIR "${CMAKE_SOURCE_DIR}/gerbertools/include/gerbertools")

# Указать пути к заголовочным файлам для сборки проекта
include_directories("${GERBERTOOLS_INCLUDE_DIR}")

# Найти все исходные файлы и заголовочные файлы
file(GLOB GERBERTOOLS_SRC_FILES "${GERBERTOOLS_SRC_DIR}/*.cpp")
file(GLOB GERBERTOOLS_HEADER_FILES "${GERBERTOOLS_INCLUDE_DIR}/*.h" "${GERBERTOOLS_INCLUDE_DIR}/*.hpp")

# Найти все файлы в папке src
file(GLOB SRC_FILES "${CMAKE_SOURCE_DIR}/src/*.cpp" "${CMAKE_SOURCE_DIR}/src/*.c" "${CMAKE_SOURCE_DIR}/src/*.h" "${CMAKE_SOURCE_DIR}/src/*.hpp")

# Указать все исходные файлы
set(SOURCE_FILES
    ${SRC_FILES}
    ${GERBERTOOLS_SRC_FILES}
)

set(HEADER_FILES
    ${GERBERTOOLS_HEADER_FILES}
)

# Организация файлов в группы
source_group(TREE ${CMAKE_SOURCE_DIR}/src PREFIX "Source Files" FILES ${SRC_FILES})
source_group(TREE ${GERBERTOOLS_SRC_DIR} PREFIX "Include\\gerbertools" FILES ${GERBERTOOLS_SRC_FILES})

source_group(TREE ${GERBERTOOLS_INCLUDE_DIR} PREFIX "Header Files\\gerbertools" FILES ${GERBERTOOLS_HEADER_FILES})

# Создаем динамическую библиотеку (DLL)
add_library(GerberToolsWrapper SHARED
    ${SOURCE_FILES}
    ${HEADER_FILES}
)

# Установить флаги компилятора для Windows
if (WIN32)
    target_compile_definitions(GerberToolsWrapper PRIVATE -DWIN32)
    # Определить экспортируемые символы в DLL
    target_compile_definitions(GerberToolsWrapper PRIVATE GERBERTOOLSWRAPPER_EXPORTS)
endif()

# Установить флаги компилятора для Linux
if (UNIX)
    target_compile_definitions(GerberToolsWrapper PRIVATE -DLINUX)
endif()

# Настройка для Visual Studio
set_property(TARGET GerberToolsWrapper PROPERTY VS_DEBUGGER_WORKING_DIRECTORY "${CMAKE_BINARY_DIR}")

# Установить директорию для сборки
set(CMAKE_BUILD_TYPE Release)
set(CMAKE_EXPORT_COMPILE_COMMANDS ON)
if (WIN32)
    # Оптимизация для Windows с использованием компилятора MSVC:
    set(CMAKE_CXX_FLAGS_RELEASE "/O2 /GL /DNDEBUG /arch:AVX2")
    set(CMAKE_EXE_LINKER_FLAGS_RELEASE "/LTCG /OPT:REF /OPT:ICF")
elseif (UNIX)
    # Оптимизация для Linux с использованием компилятора GCC:
    set(CMAKE_CXX_FLAGS_RELEASE "-O3 -march=native -funroll-loops -fstrict-aliasing -ffast-math -flto -ffunction-sections -fdata-sections -Wl,--gc-sections")
endif()