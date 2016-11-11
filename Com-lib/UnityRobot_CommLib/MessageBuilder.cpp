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

void MessageBuilder::addIdentificationRes(Communication::Message& msg, const std::string& str)
{
	auto idRes = msg.mutable_identificationresponse();
	idRes->set_robottype(str);
}

void MessageBuilder::addErrorLog(Communication::Message& msg, const std::string& str)
{
	auto errorLog = msg.mutable_error();
	errorLog->set_message(str);
}

void MessageBuilder::addCustomMsg(Communication::Message& msg, const std::string& key, const std::string& data)
{
	auto customMsg = msg.mutable_custommessage();
	customMsg->set_key(key);

	if(!data.empty())
		customMsg->set_data(data);
}

void MessageBuilder::addShapeUpdateInfo(Communication::Message& msg)
{
	//is that even useful? Who knows!
}

void MessageBuilder::addChangedShape(Communication::Message& msg, int32_t id, std::initializer_list<Communication::Transform::Vector3_> vertices)
{
	auto shapeUpdate = msg.mutable_shapeupdate();
	addShape(shapeUpdate->add_changedshapes(), id, vertices);
}

void MessageBuilder::addChangedShape(Communication::Message& msg, int32_t id, std::initializer_list<std::array<float, 3>> vertices)
{
	auto shapeUpdate = msg.mutable_shapeupdate();
	addShape(shapeUpdate->add_changedshapes(), id, vertices);
}

void MessageBuilder::addNewShape(Communication::Message& msg, int32_t id, std::initializer_list<Communication::Transform::Vector3_> vertices)
{
	auto shapeUpdate = msg.mutable_shapeupdate();
	addShape(shapeUpdate->add_newshapes(), id, vertices);
}

void MessageBuilder::addNewShape(Communication::Message& msg, int32_t id, std::initializer_list<std::array<float, 3>> vertices)
{
	auto shapeUpdate = msg.mutable_shapeupdate();
	addShape(shapeUpdate->add_newshapes(), id, vertices);
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

void MessageBuilder::addShape(Communication::Messages::Shape_* sh, int32_t id, std::initializer_list<std::array<float, 3>> vertices)
{
	sh->set_id(id);

	for (const auto& v : vertices)
		addVerticesToShape(sh, v);
}

void MessageBuilder::addShape(Communication::Messages::Shape_* sh, int32_t id, std::initializer_list<Communication::Transform::Vector3_> vertices)
{
	sh->set_id(id);

	for (const auto& v : vertices)
		addVerticesToShape(sh, v);
}

void MessageBuilder::addVerticesToShape(Communication::Messages::Shape_* shape, const Communication::Transform::Vector3_& vec)
{
	auto newVec = shape->add_vertices();
	newVec->CopyFrom(vec);
}

void MessageBuilder::addVerticesToShape(Communication::Messages::Shape_* shape, std::array<float, 3> values)
{
	auto newVec = shape->add_vertices();
	setVec3(newVec, values);
}

void MessageBuilder::setVec3(Communication::Transform::Vector3_* vec, std::array<float, 3> values)
{
	vec->set_x(values[0]);
	vec->set_y(values[1]);
	vec->set_z(values[2]);
}

Communication::Transform::Vector3_ MessageBuilder::createVec3(std::array<float, 3> vertices)
{
	Communication::Transform::Vector3_ result;

	result.set_x(vertices[0]);
	result.set_y(vertices[1]);
	result.set_z(vertices[2]);

	return result;
}
}
