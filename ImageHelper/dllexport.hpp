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

#ifdef __cplusplus
extern "C" {
#endif
	__EXPORT _CDECL(int) stb_resize(void* data, int length, int width, int height, void** result, int fix_ratio);
	__EXPORT _CDECL(void) stb_free(void* data);
	__EXPORT _CDECL(int) stb_getinfo(void* data, int length, int* x,int* y, int* comp, char* format);
#ifdef __cplusplus
};
#endif


