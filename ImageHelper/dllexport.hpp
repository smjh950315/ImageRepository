#pragma once
#include <iostream>
#include <type_traits>
#define DLL_EXPORTS
#if defined(_WIN32) || defined(_WIN64)
#ifdef DLL_EXPORTS
#define __EXPORT __declspec(dllexport)
#else
#define __EXPORT __declspec(dllimport)
#endif // DLL_EXPORTS
#else
#define __EXPORT
#endif

#define _CDECL(x) x __cdecl

using intptr = std::intptr_t;
using uintptr = std::uintptr_t;

template<class T>
struct alignas(8) wrapper {
	T value;
};

struct alignas(8) ImSize2D {
	int width;
	int height;
};

struct alignas(8) ImInfo {
	ImSize2D size;
	wrapper<int> channels;
	char* format;
};

#ifdef __cplusplus
extern "C" {
#endif
	__EXPORT _CDECL(int) stb_resize(void* data, int length, void* imsize, int channels, void** result, int fix_ratio);
	__EXPORT _CDECL(void) stb_free(void* data);
	__EXPORT _CDECL(int) stb_getinfo(void* data, int length, void* iminfo);


	__EXPORT _CDECL(void*) cv_get_matrix(char* data, int length);
	__EXPORT _CDECL(void) cv_free_matrix(void* matPtr);

	__EXPORT _CDECL(double) cv_get_differential_by_mse(void* lmatptr, void* rmatptr);
	__EXPORT _CDECL(double) cv_get_differential_by_ssim(void* lmatptr, void* rmatptr);
	__EXPORT _CDECL(void*) cv_get_differential_bfmatch(void* lmatptr, void* rmatptr);

#ifdef __cplusplus
};
#endif


