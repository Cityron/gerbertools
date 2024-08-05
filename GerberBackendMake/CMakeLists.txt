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
set(ZLIB_SRC_DIR "${CMAKE_SOURCE_DIR}/zlib-1.2.11")
set(MINIZIP_SRC_DIR "${ZLIB_SRC_DIR}/contrib/minizip")
set(ZLIB_INCLUDE_DIR "${CMAKE_SOURCE_DIR}/zlib-1.2.11")
set(MINIZIP_INCLUDE_DIR "${ZLIB_INCLUDE_DIR}/contrib/minizip")
set(MINIZIP_INCLUDE_DIR_BUILD "${ZLIB_INCLUDE_DIR}/build")

# Указать пути для сборки проекта
set(PROJECT_SRC_DIR "${CMAKE_BINARY_DIR}/Source_Files")
set(PROJECT_INCLUDE_DIR "${CMAKE_BINARY_DIR}/Source_Headers")

# Создать необходимые директории в проекте
file(MAKE_DIRECTORY "${PROJECT_SRC_DIR}/gerbertools")
file(MAKE_DIRECTORY "${PROJECT_SRC_DIR}/zlib")
file(MAKE_DIRECTORY "${PROJECT_SRC_DIR}/minizip")
file(MAKE_DIRECTORY "${PROJECT_INCLUDE_DIR}/gerbertools")
file(MAKE_DIRECTORY "${PROJECT_INCLUDE_DIR}/zlib")
file(MAKE_DIRECTORY "${PROJECT_INCLUDE_DIR}/minizip")

# Скопировать файлы в проектные директории
file(GLOB GERBERTOOLS_SRC_FILES "${GERBERTOOLS_SRC_DIR}/*.cpp")
file(GLOB GERBERTOOLS_HEADER_FILES "${GERBERTOOLS_INCLUDE_DIR}/*.h" "${GERBERTOOLS_INCLUDE_DIR}/*.hpp")
file(GLOB ZLIB_SRC_FILES "${ZLIB_SRC_DIR}/*.c")
file(GLOB ZLIB_HEADER_FILES "${ZLIB_INCLUDE_DIR}/*.h" "${ZLIB_INCLUDE_DIR}/*.hpp")
file(GLOB MINIZIP_SRC_FILES "${MINIZIP_SRC_DIR}/*.c")
file(GLOB MINIZIP_HEADER_FILES "${MINIZIP_INCLUDE_DIR}/*.h" "${MINIZIP_INCLUDE_DIR_BUILD}/*.h" "${MINIZIP_INCLUDE_DIR}/*.hpp")

foreach(file ${GERBERTOOLS_SRC_FILES})
    file(COPY ${file} DESTINATION "${PROJECT_SRC_DIR}/gerbertools")
endforeach()

foreach(file ${GERBERTOOLS_HEADER_FILES})
    file(COPY ${file} DESTINATION "${PROJECT_INCLUDE_DIR}/gerbertools")
endforeach()

foreach(file ${ZLIB_SRC_FILES})
    file(COPY ${file} DESTINATION "${PROJECT_SRC_DIR}/zlib")
endforeach()

foreach(file ${ZLIB_HEADER_FILES})
    file(COPY ${file} DESTINATION "${PROJECT_INCLUDE_DIR}/zlib")
endforeach()

foreach(file ${MINIZIP_SRC_FILES})
    file(COPY ${file} DESTINATION "${PROJECT_SRC_DIR}/minizip")
endforeach()

foreach(file ${MINIZIP_HEADER_FILES})
    file(COPY ${file} DESTINATION "${PROJECT_INCLUDE_DIR}/minizip")
endforeach()

# Указать пути к заголовочным файлам для сборки проекта
include_directories("${PROJECT_INCLUDE_DIR}/gerbertools")
include_directories("${PROJECT_INCLUDE_DIR}/zlib")
include_directories("${PROJECT_INCLUDE_DIR}/minizip")

# Найти все исходные файлы в проектных директориях
file(GLOB PROJECT_GERBERTOOLS_SRC_FILES "${PROJECT_SRC_DIR}/gerbertools/*.cpp")
file(GLOB PROJECT_ZLIB_SRC_FILES "${PROJECT_SRC_DIR}/zlib/*.c")
file(GLOB PROJECT_MINIZIP_SRC_FILES "${PROJECT_SRC_DIR}/minizip/*.c")
file(GLOB PROJECT_GERBERTOOLS_HEADER_FILES "${PROJECT_INCLUDE_DIR}/gerbertools/*.h" "${PROJECT_INCLUDE_DIR}/gerbertools/*.hpp")
file(GLOB PROJECT_ZLIB_HEADER_FILES "${PROJECT_INCLUDE_DIR}/zlib/*.h" "${PROJECT_INCLUDE_DIR}/zlib/*.hpp")
file(GLOB PROJECT_MINIZIP_HEADER_FILES "${PROJECT_INCLUDE_DIR}/minizip/*.h" "${PROJECT_INCLUDE_DIR}/minizip/*.hpp")

# Указать все исходные файлы
set(SRC_FILES
    src/GerberToolsWrapper.cpp
    ${PROJECT_GERBERTOOLS_SRC_FILES}
    ${PROJECT_ZLIB_SRC_FILES}
    ${PROJECT_MINIZIP_SRC_FILES}
)

set(HEADER_FILES
    ${PROJECT_GERBERTOOLS_HEADER_FILES}
    ${PROJECT_ZLIB_HEADER_FILES}
    ${PROJECT_MINIZIP_HEADER_FILES}
)

# Организация файлов в группы
source_group(TREE ${PROJECT_SRC_DIR}/gerbertools PREFIX "Source Files\\gerbertools" FILES ${PROJECT_GERBERTOOLS_SRC_FILES})
source_group(TREE ${PROJECT_SRC_DIR}/zlib PREFIX "Source Files\\zlib" FILES ${PROJECT_ZLIB_SRC_FILES})
source_group(TREE ${PROJECT_SRC_DIR}/minizip PREFIX "Source Files\\minizip" FILES ${PROJECT_MINIZIP_SRC_FILES})
source_group(TREE ${PROJECT_INCLUDE_DIR}/gerbertools PREFIX "Header Files\\gerbertools" FILES ${PROJECT_GERBERTOOLS_HEADER_FILES})
source_group(TREE ${PROJECT_INCLUDE_DIR}/zlib PREFIX "Header Files\\zlib" FILES ${PROJECT_ZLIB_HEADER_FILES})
source_group(TREE ${PROJECT_INCLUDE_DIR}/minizip PREFIX "Header Files\\minizip" FILES ${PROJECT_MINIZIP_HEADER_FILES})

# Создаем исполняемый файл
add_executable(GerberToolsWrapper
    ${SRC_FILES}
    ${HEADER_FILES}
)

# Установить флаги компилятора для Windows
if (WIN32)
    target_compile_definitions(GerberToolsWrapper PRIVATE -DWIN32)
endif()

# Установить флаги компилятора для Linux
if (UNIX)
    target_compile_definitions(GerberToolsWrapper PRIVATE -DLINUX)
    target_link_libraries(GerberToolsWrapper PRIVATE pthread)
endif()

# Настройка для Visual Studio
set_property(TARGET GerberToolsWrapper PROPERTY VS_DEBUGGER_WORKING_DIRECTORY "${CMAKE_BINARY_DIR}")

# Установить директорию для сборки
set(CMAKE_BUILD_TYPE Release)
set(CMAKE_EXPORT_COMPILE_COMMANDS ON)
if (WIN32)
    # Оптимизация для Windows с использованием компилятора MSVC:
    set(CMAKE_CXX_FLAGS_RELEASE "/O2 /GL /DNDEBUG /arch:AVX2")
    # /O2: Включает максимальную оптимизацию для скорости.
    # /GL: Включает межмодульную оптимизацию (Whole Program Optimization).
    # /DNDEBUG: Отключает макрос assert для релизной сборки.
    # /arch:AVX2: Включает использование инструкций AVX2, если процессор их поддерживает.

    set(CMAKE_EXE_LINKER_FLAGS_RELEASE "/LTCG /OPT:REF /OPT:ICF")
    # /LTCG: Включает глобальную оптимизацию на этапе компоновки (Link Time Code Generation).
    # /OPT:REF: Удаляет неиспользуемые функции и данные.
    # /OPT:ICF: Объединяет функции с одинаковым кодом для уменьшения размера.
elseif (UNIX)
    # Оптимизация для Linux с использованием компилятора GCC:
    set(CMAKE_CXX_FLAGS_RELEASE "-O3 -march=native -funroll-loops -fstrict-aliasing -ffast-math -flto -ffunction-sections -fdata-sections -Wl,--gc-sections")
    # -O3: Включает максимальную оптимизацию для скорости.
    # -march=native: Использует инструкции процессора, на котором выполняется компиляция.
    # -funroll-loops: Включает разворачивание циклов.
    # -fstrict-aliasing: Включает строгий алиасинг для оптимизации.
    # -ffast-math: Включает агрессивные математические оптимизации.
    # -flto: Включает межмодульную оптимизацию (Link Time Optimization).
    # -ffunction-sections и -fdata-sections: Размещает функции и данные в отдельных секциях.
    # -Wl,--gc-sections: Удаляет неиспользуемые секции на этапе компоновки.
endif()