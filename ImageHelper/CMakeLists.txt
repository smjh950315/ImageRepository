cmake_minimum_required(VERSION 3.22)
project(ImageHelper)
set(GCC_COVERAGE_COMPILE_FLAGS "-fpermissive")
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} ${GCC_COVERAGE_COMPILE_FLAGS}")
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -O3")

# add include dir
include_directories(
    "${CMAKE_CURRENT_SOURCE_DIR}"
)
set(CMAKE_SHARED_LINKER_FLAGS
    "${CMAKE_SHARED_LINKER_FLAGS} -Wl,-rpath -Wl,$ORIGIN -Wl,-fPIC"
)

add_library(ImageHelper SHARED ImageHelper.cpp)
set_target_properties(ImageHelper PROPERTIES OUTPUT_NAME ImageHelper)
target_compile_features(
    ImageHelper
    PUBLIC
    cxx_std_20
)
add_library(ImageHelper_static STATIC ImageHelper.cpp)
set_target_properties(ImageHelper_static PROPERTIES OUTPUT_NAME ImageHelpers)
target_compile_features(
    ImageHelper_static
    PUBLIC
    cxx_std_20
)
