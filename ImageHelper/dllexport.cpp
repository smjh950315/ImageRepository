#include "dllexport.hpp"
#define STB_IMAGE_IMPLEMENTATION
#define STB_IMAGE_WRITE_IMPLEMENTATION
#define STB_IMAGE_RESIZE_IMPLEMENTATION
#include "stb_image.h"
#include "stb_image_write.h"
#include "stb_image_resize2.h"
#define __CONSTEXPR_CSTR(x) static constexpr const char* x = #x
struct image_format_cstring {
	__CONSTEXPR_CSTR(jpeg);
	__CONSTEXPR_CSTR(png);
	__CONSTEXPR_CSTR(gif);
	__CONSTEXPR_CSTR(bmp);
	__CONSTEXPR_CSTR(psd);
	__CONSTEXPR_CSTR(pic);
	__CONSTEXPR_CSTR(pnm);
	__CONSTEXPR_CSTR(hdr);
	__CONSTEXPR_CSTR(tga);
};

#define TEST_AND_WRITE_FMTBUFFER(fmt, buff) \
if (stbi__##fmt##_info(stb_ctx_ptr, px,py,pc)) \
{ memcpy(buff, image_format_cstring::##fmt, strlen(image_format_cstring::##fmt) + 1); return 1; }


__EXPORT _CDECL(int) stb_resize(void* data, int length, int width, int height, void** result, int fix_ratio)
{
	if (data == nullptr || length <= 0 || width <= 0 || height <= 0 || result == nullptr) return -1;

	int ori_width, ori_height, ori_comp;

	auto ori_data = stbi_load_from_memory((stbi_uc*)data, length, &ori_width, &ori_height, &ori_comp, 0);

	if (ori_width <= 0 || ori_height <= 0 || ori_comp <= 0 || ori_data == nullptr) {
		if (ori_data != nullptr) {
			STBI_FREE(ori_data);
		}
		return -1;
	}

	if (fix_ratio) {
		double ratio_w = (double)width / (double)ori_width;
		double ratio_h = (double)height / (double)ori_height;
		if (ratio_w <= ratio_h) {
			height = static_cast<int>((double)ori_height * ratio_w);
		} else {
			width = static_cast<int>((double)ori_width * ratio_h);
		}
	}

	stbir_pixel_layout layout = STBIR_RGB;
	if (ori_comp == 4) layout = STBIR_RGBA;

	void* metaResult = stbir_resize_uint8_linear(
		ori_data, ori_width, ori_height, ori_width * ori_comp,
		nullptr, width, height, width * ori_comp, layout
	);

	if (metaResult == nullptr) return -1;

	int byteLength;
	void* resultData = stbi_write_png_to_mem((unsigned char*)metaResult, width * ori_comp, width, height, ori_comp, &byteLength);
	STBI_FREE(metaResult);
	if (byteLength <= 0 || resultData == nullptr) {
		if (resultData != nullptr) {
			stb_free(resultData);
		}
		return -1;
	}
	*result = resultData;
	return byteLength;
}

__EXPORT _CDECL(void) stb_free(void* data)
{
	if (data == nullptr) return;
	STBI_FREE(data);
}

__EXPORT _CDECL(int) stb_getinfo(void* data, int length, int* px, int* py, int* pc, char* format)
{
	stbi__context s;
	stbi__start_mem(&s, (stbi_uc*)data, length);
	stbi__context* stb_ctx_ptr = &s;
	TEST_AND_WRITE_FMTBUFFER(jpeg, format);
	TEST_AND_WRITE_FMTBUFFER(png, format);
	TEST_AND_WRITE_FMTBUFFER(gif, format);
	TEST_AND_WRITE_FMTBUFFER(bmp, format);
	TEST_AND_WRITE_FMTBUFFER(psd, format);
	TEST_AND_WRITE_FMTBUFFER(pic, format);
	TEST_AND_WRITE_FMTBUFFER(pnm, format);
	TEST_AND_WRITE_FMTBUFFER(hdr, format);
	TEST_AND_WRITE_FMTBUFFER(tga, format);
	return -1;
}
