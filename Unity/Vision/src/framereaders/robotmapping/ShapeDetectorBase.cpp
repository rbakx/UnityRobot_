#include <stdexcept>

#include "ShapeDetectorBase.hpp"
#include "ObjectDetector.hpp"

#include "../../Settings.h"

using namespace std;
using namespace robotmapping;
using namespace frames;

ShapeDetectorBase::ShapeDetectorBase(const ShapeDetectorBase& base) noexcept
{
	_receiver = base._receiver;
}

ShapeDetectorBase::ShapeDetectorBase(IShapeDetectionEvents* receiver) noexcept
{
	if(receiver == nullptr)
	{
		throw invalid_argument("[ShapeDetectorBase] receiver is null, must be proper object");
	}
	
	_receiver = receiver;
}
		
ShapeDetectorBase::~ShapeDetectorBase()
{
	
}

vector<shared_ptr<VideoFeedFrameReceiver>> ShapeDetectorBase::createAndStartDetectorsFromSettings(IShapeDetectionEvents& receiver) noexcept
{
	vector<string> sampleNames = settings->getGeneralProperties().sampleNames;
	vector<shared_ptr<VideoFeedFrameReceiver>> detectors;

	
	for(const string& sampleName : sampleNames)
	{
		shared_ptr<VideoFeedFrameReceiver> detector = make_shared<ObjectDetector>(ObjectDetector(&receiver, sampleName));
		
		detectors.emplace_back(detector);
	}

	return detectors;
}