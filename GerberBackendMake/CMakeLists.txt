cmake_minimum_required(VERSION 3.8)
project(GerberToolsWrapper)

# Установить стандарт C++
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

# Указать пути к исходным и заголовочным файлам gerbertools
set(GERBERTOOLS_SRC_DIR "${CMAKE_SOURCE_DIR}/gerbertools/src")
set(GERBERTOOLS_INCLUDE_DIR "${CMAKE_SOURCE_DIR}/gerbertools/include/gerbertools")

# Указать пути к заголовочным файлам zlib и minizip
set(ZLIB_INCLUDE_DIR "${CMAKE_SOURCE_DIR}/zlib-1.2.11")
set(MINIZIP_INCLUDE_DIR "${ZLIB_INCLUDE_DIR}/contrib/minizip")
set(MINIZIP_INCLUDE_DIR_BUILD "${ZLIB_INCLUDE_DIR}/build")

# Указать пути к заголовочным файлам
include_directories(${GERBERTOOLS_INCLUDE_DIR})
include_directories(${ZLIB_INCLUDE_DIR})
include_directories(${MINIZIP_INCLUDE_DIR})
include_directories(${MINIZIP_INCLUDE_DIR_BUILD})

# Найти все исходные файлы .cpp и .hpp в директориях, исключая miniunz.c и minizip.c
file(GLOB GERBERTOOLS_SRC_FILES 
    ${GERBERTOOLS_SRC_DIR}/*.cpp
)
file(GLOB ZLIB_SRC_FILES 
    ${ZLIB_INCLUDE_DIR}/*.c
)
file(GLOB MINIZIP_SRC_FILES 
    ${MINIZIP_INCLUDE_DIR}/*.c
)

# Исключить файлы miniunz.c и minizip.c, так как они содержат функции main
list(REMOVE_ITEM MINIZIP_SRC_FILES 
    "${MINIZIP_INCLUDE_DIR}/miniunz.c"
    "${MINIZIP_INCLUDE_DIR}/minizip.c"
)

# Указать все исходные файлы
set(SRC_FILES
    src/GerberToolsWrapper.cpp
    ${GERBERTOOLS_SRC_FILES}
    ${ZLIB_SRC_FILES}
    ${MINIZIP_SRC_FILES}
)

# Создаем исполняемый файл
add_executable(GerberToolsWrapper
    ${SRC_FILES}
)

# Установить флаги компилятора для Windows
if (WIN32)
    target_compile_definitions(GerberToolsWrapper PRIVATE -DWIN32)
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

# Добавление дополнительных определений (если нужно)
add_definitions(-DGL_GLEXT_PROTOTYPES)
add_definitions(-DGLX_GLXEXT_PROTOTYPES)