#include "MessageBuilder.h"
#include <array>

namespace Networking
{

Communication::Message MessageBuilder::create(Communication::MessageType_ type, Communication::MessageTarget_ tgt, int32_t id)
{
	Communication::Message result;

	result.set_messagetype(type);
	result.set_messagetarget(tgt);
	result.set_id(id);

	return result;
}

void MessageBuilder::addStringData(Communication::Message& msg, const std::string& str)
{
	auto strdata = msg.mutable_stringdata();
	strdata->append(str);
}

void MessageBuilder::addChangedShape(Communication::Message& msg, std::initializer_list<Communication::Messages::Shape_> init)
{
	for(auto shape : init)
	{
		
	}
}

void MessageBuilder::addNewShape(Communication::Message& msg, int32_t id, std::initializer_list<Communication::Transform::Vector3_> vertices)
{
	auto shapeUpdate = msg.mutable_shapeupdate();
	auto shape = shapeUpdate->add_newshapes();
	shape->set_id(id);

	for (const auto& v : vertices)
	{
		auto ver = shape->add_vertices();
		ver->set_x(v.x());
		ver->set_y(v.y());
		ver->set_z(v.z());
	}
}

void MessageBuilder::setVelocity(Communication::Message& msg, std::array<float, 3> linear, std::array<float, 3> angular)
{
	setLinVelocity(msg, linear);
	setAngVelocity(msg, angular);
}

void MessageBuilder::setLinVelocity(Communication::Message& msg, std::array<float, 3> linear)
{
	setVec3(msg.mutable_robotvelocity()->mutable_linearvelocity(), linear);
}

void MessageBuilder::setAngVelocity(Communication::Message& msg, std::array<float, 3> angular)
{
	setVec3(msg.mutable_robotvelocity()->mutable_angularvelocity(), angular);
}

void MessageBuilder::setVec3(Communication::Transform::Vector3_* vec, std::array<float, 3> values)
{
	vec->set_x(values[0]);
	vec->set_y(values[1]);
	vec->set_z(values[2]);
}
}
