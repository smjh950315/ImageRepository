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

struct life_time_handler {
	static constexpr void free_stbi_memory(void* block) {
		if (block != nullptr)
			STBI_FREE(block);
	}
	void* ptr{};
	void(*callback_free)(void*) = nullptr;
	life_time_handler(void* _ptr, void(*pfunc)(void*)) : ptr(_ptr), callback_free(pfunc) { }
	~life_time_handler() {
		if (ptr != nullptr && callback_free != nullptr) {
			callback_free(ptr);
		}
	}
};

#define TEST_AND_WRITE_FMTBUFFER(fmt, fmtstr_addr_ptr) \
if (stbi__##fmt##_info(stb_ctx_ptr, px,py,pc)) \
{ *fmtstr_addr_ptr = (char*)image_format_cstring::##fmt; return 1; }


__EXPORT _CDECL(void*) c_lang_malloc(size_t _blocksize)
{
	return malloc(_blocksize);
}

__EXPORT _CDECL(void) c_lang_free(void* _block)
{
	free(_block);
}

__EXPORT _CDECL(void*) c_lang_realloc(void* _block, size_t _size)
{
	return realloc(_block, _size);
}

__EXPORT _CDECL(int) stb_resize(void* data, int length, void* imsize, int channels, void** result, int fix_ratio)
{
	if (data == nullptr || length <= 0 || imsize == nullptr || result == nullptr) return -1;
	ImSize2D* psize = (ImSize2D*)imsize;

	int width = psize->width;
	int height = psize->height;
	int in_comp = channels;
	int out_comp;
	if (width <= 0 || height <= 0) return -1;

	int ori_width, ori_height, ori_comp;

	auto ori_data = stbi_load_from_memory((stbi_uc*)data, length, &ori_width, &ori_height, &ori_comp, channels);
	life_time_handler ori_data_handler{ ori_data, life_time_handler::free_stbi_memory };

	if (ori_width <= 0 || ori_height <= 0 || ori_comp <= 0 || ori_data == nullptr) return -1;

	if (in_comp == 0) {
		out_comp = ori_comp;
	} else {
		out_comp = in_comp;
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

	stbir_pixel_layout layout;
	switch (out_comp)
	{
		case 1:
			layout = STBIR_1CHANNEL;
			break;
		case 2:
			layout = STBIR_2CHANNEL;
			break;
		case 3:
			layout = STBIR_RGB;
			break;
		case 4:
			layout = STBIR_4CHANNEL;
			break;
		default:
			return -1;
	}

	void* metaResult = stbir_resize_uint8_linear(
		ori_data, ori_width, ori_height, ori_width * ori_comp,
		nullptr, width, height, width * ori_comp, layout
	);
	if (metaResult == nullptr) return -1;

	life_time_handler meta_result_handler{ metaResult, life_time_handler::free_stbi_memory };


	int byteLength;
	void* resultData = stbi_write_png_to_mem((unsigned char*)metaResult, width * ori_comp, width, height, ori_comp, &byteLength);

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

__EXPORT _CDECL(int) stb_getinfo(void* data, int length, void* iminfo_ptr)
{
	if (iminfo_ptr == nullptr) return -1;
	ImInfo* imInfoPtr = (ImInfo*)iminfo_ptr;
	ImInfo& imInfo = *imInfoPtr;
	int* px = &imInfo.size.width;
	int* py = &imInfo.size.height;
	int* pc = &imInfo.channels.value;
	char** pformat = &imInfo.format;

	stbi__context s;
	stbi__start_mem(&s, (stbi_uc*)data, length);
	stbi__context* stb_ctx_ptr = &s;
	TEST_AND_WRITE_FMTBUFFER(jpeg, pformat);
	TEST_AND_WRITE_FMTBUFFER(png, pformat);
	TEST_AND_WRITE_FMTBUFFER(gif, pformat);
	TEST_AND_WRITE_FMTBUFFER(bmp, pformat);
	TEST_AND_WRITE_FMTBUFFER(psd, pformat);
	TEST_AND_WRITE_FMTBUFFER(pic, pformat);
	TEST_AND_WRITE_FMTBUFFER(pnm, pformat);
	TEST_AND_WRITE_FMTBUFFER(hdr, pformat);
	TEST_AND_WRITE_FMTBUFFER(tga, pformat);
	return -1;
}
#include <opencv2/opencv.hpp>

static constexpr void delete_cv_mat(void* pmat) {
	if (pmat != nullptr) {
		delete ((cv::Mat*)pmat);
	}
}

static bool is_same_size(const cv::Mat& img1, const cv::Mat& img2) {
	return img1.size == img2.size;
}

static int get_cvtColor_flag_to_gray(int channel) {
	switch (channel)
	{
		case 3:
			return cv::COLOR_BGR2GRAY;
		case 4:
			return cv::COLOR_BGRA2GRAY;
		default:
			return -1;
	}
}

template<class T>
static T run_cv_gray_compute(void* lmatptr, void* rmatptr, T(*pfunc)(const cv::Mat&, const cv::Mat&), T failure_value, bool(*pvalidate)(const cv::Mat&, const cv::Mat&)) {
	if (lmatptr != nullptr && rmatptr != nullptr) {
		using Mat = cv::Mat;
		life_time_handler lhs_handler{ nullptr, delete_cv_mat };
		life_time_handler rhs_handler{ nullptr, delete_cv_mat };
		Mat* plhs = (Mat*)lmatptr;
		Mat* prhs = (Mat*)rmatptr;
		if (pvalidate) {
			if (!pvalidate(*plhs, *prhs)) return failure_value;
		}
		if (plhs->channels() != 1) {
			int flag = get_cvtColor_flag_to_gray(plhs->channels());
			if (flag == -1) return failure_value;
			Mat* l_ori = (Mat*)lmatptr;
			plhs = new Mat();
			lhs_handler.ptr = plhs;
			cv::cvtColor(*l_ori, *plhs, flag);
		}
		if (prhs->channels() != 1) {
			int flag = get_cvtColor_flag_to_gray(prhs->channels());
			if (flag == -1) return failure_value;
			Mat* r_ori = (Mat*)rmatptr;
			prhs = new Mat();
			rhs_handler.ptr = prhs;
			cv::cvtColor(*r_ori, *prhs, flag);
		}
		return pfunc(*plhs, *prhs);
	}
	return failure_value;
}

static double computeMSE(const cv::Mat& image1, const cv::Mat& image2) {
	cv::Mat s1;
	absdiff(image1, image2, s1); // Absolute differences
	s1.convertTo(s1, CV_32F);    // Convert to float
	s1 = s1.mul(s1);             // Square the differences

	cv::Scalar s = sum(s1);      // Sum all the elements
	double sse = s.val[0] + s.val[1] + s.val[2]; // Sum all the channels

	double mse = sse / (double)(image1.channels() * image1.total());
	return mse;
}

static double computeSSIM(const cv::Mat& i1, const cv::Mat& i2) {
	const double C1 = 6.5025, C2 = 58.5225;
	cv::Mat I1, I2;
	i1.convertTo(I1, CV_32F);
	i2.convertTo(I2, CV_32F);

	cv::Mat I2_2 = I2.mul(I2);        // I2^2
	cv::Mat I1_2 = I1.mul(I1);        // I1^2
	cv::Mat I1_I2 = I1.mul(I2);        // I1 * I2

	cv::Mat mu1, mu2;
	cv::GaussianBlur(I1, mu1, cv::Size(11, 11), 1.5);
	cv::GaussianBlur(I2, mu2, cv::Size(11, 11), 1.5);

	cv::Mat mu1_2 = mu1.mul(mu1);
	cv::Mat mu2_2 = mu2.mul(mu2);
	cv::Mat mu1_mu2 = mu1.mul(mu2);

	cv::Mat sigma1_2, sigma2_2, sigma12;

	cv::GaussianBlur(I1_2, sigma1_2, cv::Size(11, 11), 1.5);
	sigma1_2 -= mu1_2;

	cv::GaussianBlur(I2_2, sigma2_2, cv::Size(11, 11), 1.5);
	sigma2_2 -= mu2_2;

	cv::GaussianBlur(I1_I2, sigma12, cv::Size(11, 11), 1.5);
	sigma12 -= mu1_mu2;

	cv::Mat t1, t2, t3;

	t1 = 2 * mu1_mu2 + C1;
	t2 = 2 * sigma12 + C2;
	t3 = t1.mul(t2);  // t3 = ((2*mu1_mu2 + C1).*(2*sigma12 + C2))

	t1 = mu1_2 + mu2_2 + C1;
	t2 = sigma1_2 + sigma2_2 + C2;
	t1 = t1.mul(t2);  // t1 = ((mu1_2 + mu2_2 + C1).*(sigma1_2 + sigma2_2 + C2))

	cv::Mat ssim_map;
	divide(t3, t1, ssim_map); // ssim_map = t3./t1;

	cv::Scalar mssim = mean(ssim_map); // Average over the image
	return (mssim.val[0] + mssim.val[1] + mssim.val[2]) / 3;
}

static cv::Mat showORB(const cv::Mat& img1, const cv::Mat& img2) {
	if (img1.empty() || img2.empty()) {
		std::cerr << "Could not open or find the image" << std::endl;
		return cv::Mat();
	}

	// Initiate ORB detector
	cv::Ptr<cv::ORB> orb = cv::ORB::create();

	// Find keypoints and descriptors
	std::vector<cv::KeyPoint> keypoints1, keypoints2;
	cv::Mat descriptors1, descriptors2;
	orb->detectAndCompute(img1, cv::Mat(), keypoints1, descriptors1);
	orb->detectAndCompute(img2, cv::Mat(), keypoints2, descriptors2);

	// Match descriptors using BFMatcher
	cv::BFMatcher matcher(cv::NORM_HAMMING);
	std::vector<cv::DMatch> matches;
	matcher.match(descriptors1, descriptors2, matches);

	// Draw matches
	cv::Mat img_matches;
	cv::drawMatches(img1, keypoints1, img2, keypoints2, matches, img_matches);

	// Show detected matches
	return img_matches;
}

__EXPORT _CDECL(void*) cv_get_matrix(char* data, int length)
{
	cv::Mat* pmat = new cv::Mat;
	try {
		cv::imdecode(cv::InputArray{ (const char*)data, length }, -1, pmat);
	} catch (...) {
		delete pmat;
		pmat = nullptr;
	}
	return pmat;
}

__EXPORT _CDECL(void) cv_free_matrix(void* matptr)
{
	delete_cv_mat(matptr);
}

__EXPORT _CDECL(double) cv_get_differential_by_mse(void* lmatptr, void* rmatptr)
{
	return run_cv_gray_compute(lmatptr, rmatptr, computeMSE, -1.0, is_same_size);
}

__EXPORT _CDECL(double) cv_get_differential_by_ssim(void* lmatptr, void* rmatptr)
{
	return run_cv_gray_compute(lmatptr, rmatptr, computeSSIM, -1.0, is_same_size);
}

__EXPORT _CDECL(void*) cv_get_differential_bfmatch(void* lmatptr, void* rmatptr)
{
	if (lmatptr != nullptr && rmatptr != nullptr) {
		using Mat = cv::Mat;
		Mat& lmat = *((Mat*)lmatptr);
		Mat& rmat = *((Mat*)rmatptr);
		Mat* pmat = new Mat();
		*pmat = showORB(lmat, rmat);
		return pmat;
	}
	return nullptr;
}

__EXPORT _CDECL(int) cv_encode_png_to_c_lang_malloc(void* matptr, void** result)
{
	if (matptr != nullptr && result != nullptr) {
		cv::Mat* pmat = (cv::Mat*)matptr;
		std::vector<uchar> buffer;
		try {
			cv::imencode(".png", *pmat, buffer);
		} catch (const std::exception& ex){
			printf(ex.what());
			return 0;
		}
		void* ptr = nullptr;
		if (buffer.size() != 0) {
			ptr = c_lang_malloc(buffer.size());
		} 
		if (ptr == nullptr) return 0;
		*result = ptr;
		memcpy(*result, buffer.data(), buffer.size());
		return static_cast<int>(buffer.size());
	}
	return 0;
}

