#include "ShapesTracker.hpp"

#include <stdexcept>

#include "IWorldMappingEventSubscriber.hpp"

using namespace std;
using namespace robotmapping;

ShapesTracker::ShapesTracker(IWorldMappingEventSubscriber* subscriber) : _subscriber(subscriber), _tracker_id_top(0)
{
	if(_subscriber == nullptr)
	{
		throw std::invalid_argument("[ShapesTracker] subscriber is null");
	}
}

ShapesTracker::~ShapesTracker()
{
	
}

void ShapesTracker::SignalNewFrame(const ShapeDetectorBase& detector) noexcept
{
	//std::cout << "[ShapesTracker] FRAME NEW" << std::endl;
	_subscriber->SignalNewFrame(*this);
}

void ShapesTracker::SignalEndFrame(const ShapeDetectorBase& detector) noexcept
{
	//std::cout << "[ShapesTracker] FRAME d END " << cf_s << ", " << pf_s << std::endl;

	bool matched = false;
	
	std::vector<bool> matched_tracks(_tracked_shapes.size());
	
	Shape::coordinate_type center_previous;
	Shape::coordinate_type center_new;

	for(auto new_it = _new_frame_shapes.begin(); new_it != _new_frame_shapes.end(); ++new_it)
	{
		center_new = std::move(new_it->Center());
		matched = false;
		
		for(auto prev_it = _tracked_shapes.begin(); prev_it != _tracked_shapes.end(); ++prev_it)
		{
			center_previous = std::move(prev_it->Center());

			double distance = sqrt(pow(center_previous.x - center_new.x, 2.0F) + pow(center_previous.y - center_new.y, 2.0F));
			
			if(distance < _TOLERANCE)
			{
				prev_it->SetCenter(center_new);
				
				matched = true;
				
				_subscriber->OnMove(*this, *prev_it);
	
				*(matched_tracks.begin() + (new_it - _new_frame_shapes.begin())) = true;
				break;
			}
		}
		
		if(!matched)
		{
			new_it->SetTrackerId(++_tracker_id_top);
			_tracked_shapes.push_back(*new_it);
			
			*(matched_tracks.begin() + (new_it - _new_frame_shapes.begin())) = true;
			
			_subscriber->OnRecognise(*this, *new_it);
		}
	}
	
	std::vector<Shape>::reverse_iterator track_it_obj = _tracked_shapes.rbegin();
	
	for (std::vector<bool>::reverse_iterator track_it = matched_tracks.rbegin() ; track_it != matched_tracks.rend(); ++track_it)
	{
		if(!(*track_it))
		{
			_subscriber->OnLost(*this, *track_it_obj);
			
			_tracked_shapes.erase(--(track_it_obj.base()));
		}
		
		track_it_obj++;
	}
	
	_new_frame_shapes.clear();
	
	_subscriber->SignalEndFrame(*this);
}

void ShapesTracker::ShapeDetected(const ShapeDetectorBase& detector, Shape& shape) noexcept
{
	//std::cout << "[ShapesTracker] NEW d SHAPE: " << shape.ToString() << std::endl;
	
	_subscriber->ShapeDetected(*this, shape);
	_new_frame_shapes.push_back(shape);
}