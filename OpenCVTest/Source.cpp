#include <opencv2/opencv.hpp>
using namespace cv;
using namespace std;

Mat imreadColor(const string& path) {
	return imread(path, IMREAD_COLOR);
}
Mat imReadGrayScale(const string& path) {
	return imread(path, IMREAD_GRAYSCALE);
}

double computeMSE(const cv::Mat& image1, const cv::Mat& image2) {
	cv::Mat s1;
	absdiff(image1, image2, s1); // Absolute differences
	s1.convertTo(s1, CV_32F);    // Convert to float
	s1 = s1.mul(s1);             // Square the differences

	cv::Scalar s = sum(s1);      // Sum all the elements
	double sse = s.val[0] + s.val[1] + s.val[2]; // Sum all the channels

	double mse = sse / (double)(image1.channels() * image1.total());
	return mse;
}

double computeSSIM(const cv::Mat& i1, const cv::Mat& i2) {
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

double showORB(const Mat& img1, const Mat& img2) {
	if (img1.empty() || img2.empty()) {
		std::cerr << "Could not open or find the image" << std::endl;
		return -1;
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
	cv::imshow("Matches", img_matches);
	cv::waitKey(0);

	return 0;
}

int main() {
	string file1 = R"(D:\dstShot.png)";
	string file2 = R"(D:\dstShot1.png)";

	auto mseVal = computeMSE(imreadColor(file1), imreadColor(file2));
	auto ssimVal = computeSSIM(imReadGrayScale(file1), imReadGrayScale(file2));
	auto orb = showORB(imReadGrayScale(file1), imReadGrayScale(file2));

	system("pause");
}


