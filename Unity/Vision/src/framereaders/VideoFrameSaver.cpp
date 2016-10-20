#include "VideoFrameSaver.hpp"
#include "../Settings.h"

using namespace std;
using namespace cv;
using namespace framereaders;

VideoFrameSaver::VideoFrameSaver() : _writer(VideoWriter()),
	width(settings->getRecordingProperties().width),
	height(settings->getRecordingProperties().height),
	fps(settings->getRecordingProperties().fps)
{
	
}

VideoFrameSaver::~VideoFrameSaver()
{
	_writer.release();
}

void VideoFrameSaver::StartSaving(const string& filename)
{
	//TODO: CHECK FILEPATH for stupidity
	
	_writer = VideoWriter(filename, CODEC, fps, Size(width, height));

    if (!_writer.isOpened())
	{
        throw runtime_error("[VideoFrameSaver] StartSaving - Could not open the output video for write!");
    }
}

void VideoFrameSaver::StopSaving() noexcept
{
	_writer.release();
}

void VideoFrameSaver::OnIncomingFrame(const Mat& frame) noexcept
{
	if(_writer.isOpened())
	{
		Mat resizedFrame;
		resize(frame, resizedFrame, Size(width, height));

		_writer.write(resizedFrame);
	}
}
