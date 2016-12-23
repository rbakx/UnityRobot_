#include "FrameMoveThreadHelper.hpp"

using namespace robotmapping;

FrameMoveThreadHelper::FrameMoveThreadHelper() noexcept : _ignore_new_till_accepted(false)
	{ Release(); }
	
FrameMoveThreadHelper::~FrameMoveThreadHelper()
	{ Release(); }

bool FrameMoveThreadHelper::HasFrame() noexcept
{ return !_frame.empty(); }

void FrameMoveThreadHelper::SetNewFrame(const cv::Mat& new_frame)
{
	std::lock_guard<std::mutex> frame_guard(_lock);
	
	if(!_ignore_new_till_accepted || !HasFrame())
	{
		_frame = new_frame.clone();
	}
}

void FrameMoveThreadHelper::IgnoreNewFramesIfPreviousNotAccepted(bool yes) noexcept
{
	_ignore_new_till_accepted = yes;
}

cv::Mat FrameMoveThreadHelper::AcceptFrame() noexcept
{
	std::lock_guard<std::mutex> frame_guard(_lock);

	return std::move(_frame);
}

void FrameMoveThreadHelper::Release() noexcept
{
	std::lock_guard<std::mutex> frame_guard(_lock);
	_frame.release();
}

cv::Mat& FrameMoveThreadHelper::GetCurrentFrameReference() noexcept
{
	std::lock_guard<std::mutex> frame_guard(_lock);
	
	return _frame;
}