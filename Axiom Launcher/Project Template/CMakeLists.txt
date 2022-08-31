
cmake_minimum_required(VERSION 3.22)
project(GAME_NAME)

set(CMAKE_POLICY_DEFAULT_CMP0077 NEW) # set() overrides option()
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

if (MSVC)
	add_link_options(/DEBUG:FULL)
	add_compile_options(/Oi /GR- /D_HAS_EXCEPTIONS=0)
endif()

add_subdirectory(lib)

file(GLOB_RECURSE GAME_NAME_SOURCES src/*.hpp src/*.cpp)
file(GLOB_RECURSE LIBS libs/*.lib)

add_executable(GAME_NAME ${GAME_SOURCES})

target_include_directories(GAME_NAME PUBLIC CPPSource)
target_include_directories(GAME_NAME PUBLIC "include")

target_link_libraries(GAME_NAME PUBLIC)

target_compile_definitions(GAME_NAME PUBLIC NOMINMAX)

