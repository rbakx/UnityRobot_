#include "ShapesTracker.hpp"

using namespace std;
using namespace robotmapping;

ShapesTracker::ShapesTracker()
{
	
}

ShapesTracker::~ShapesTracker()
{
	
}

void ShapesTracker::SignalNewFrame(const ShapeDetectorBase& detector) noexcept
{
	//std::cout << "[ShapesTracker] FRAME NEW" << std::endl;
}

void ShapesTracker::SignalEndFrame(const ShapeDetectorBase& detector) noexcept
{	
	vector<Shape>::size_type cf_s = _new_frame_shapes.size();
	vector<Shape>::size_type pf_s = _tracked_shapes.size();

	//std::cout << "[ShapesTracker] FRAME d END " << cf_s << ", " << pf_s << std::endl;
	
	double distance = 0.0;
	double tollerance = 65.0;
	
	bool matched = false;
	
	std::vector<bool> matched_tracks(_tracked_shapes.size());
	
	Shape::coordinate_type center_previous;
	Shape::coordinate_type center_new;
	
	for (std::vector<Shape>::reverse_iterator rev_new_it = _new_frame_shapes.rbegin() ; rev_new_it != _new_frame_shapes.rend(); ++rev_new_it)
	{
		center_new = rev_new_it->Center();
		matched = false;
		
		for (std::vector<Shape>::reverse_iterator rev_prev_it = _tracked_shapes.rbegin() ; rev_prev_it != _tracked_shapes.rend(); ++rev_prev_it)
		{
			center_previous = rev_prev_it->Center();
			
			distance = sqrt(pow(center_previous.x - center_new.x, 2.0F) + pow(center_previous.y - center_new.y, 2.0F));
			
			if(distance < tollerance)
			{
				rev_prev_it->SetCenter(center_new);
				
				matched = true;
				
				cout << "[ShapesTracker] POSITION UPDATE: (" << rev_new_it->GetTrackerId() << ") " << rev_prev_it->ToString() << std::endl;
	
				*(matched_tracks.begin() + (rev_new_it - _new_frame_shapes.rbegin())) = true;			
				break;
			}
		}
		
		if(!matched)
		{
			rev_new_it->SetTrackerId(++tracker_id_top);
			_tracked_shapes.push_back(*rev_new_it);
			
			*(matched_tracks.begin() + (rev_new_it - _new_frame_shapes.rbegin())) = true;
			
			cout << "[ShapesTracker] NEW MATCH: (" << rev_new_it->GetTrackerId() << ") " << rev_new_it->ToString() << std::endl;
		}
	}
	
	std::vector<Shape>::reverse_iterator track_it_obj = _tracked_shapes.rbegin();
	
	for (std::vector<bool>::reverse_iterator track_it = matched_tracks.rbegin() ; track_it != matched_tracks.rend(); ++track_it)
	{
		if(!(*track_it))
		{
			std::cout << "[ShapesTracker] I LOST IT, BRO! (" << track_it_obj->GetTrackerId() << ") " << track_it_obj->ToString() << std::endl;
			_tracked_shapes.erase(--(track_it_obj.base()));
		}
		
		track_it_obj++;
	}
	
	_new_frame_shapes.clear();
}

void ShapesTracker::ShapeDetected(const ShapeDetectorBase& detector, Shape& shape) noexcept
{
	//std::cout << "[ShapesTracker] NEW d SHAPE: " << shape.ToString() << std::endl;
	
	_new_frame_shapes.push_back(shape);
	
}