#include "MessageBuilder.h"
#include <algorithm>

namespace
{
	bool operator==(const Communication::Transform::Vector3_& l, const Communication::Transform::Vector3_& r)
	{
		return l.x() == r.x() && l.y() == r.y() && l.z() == r.z();
	}
}

namespace Networking
{
using Msg = Communication::Message;
using Vec3 = Communication::Transform::Vector3_;
using Shape = Communication::Messages::Shape_;
using array3 = std::array<float, 3>;

Msg MessageBuilder::create(Communication::MessageType_ type, Communication::MessageTarget_ tgt, int32_t id)
{
	Msg result;

	result.set_messagetype(type);
	result.set_messagetarget(tgt);
	result.set_id(id);

	return result;
}

void MessageBuilder::addStringData(Msg& msg, const std::string& str)
{
	auto strdata = msg.mutable_stringdata();
	strdata->append(str);
}

void MessageBuilder::addIdentificationRes(Msg& msg, const std::string& str)
{
	auto idRes = msg.mutable_identificationresponse();
	idRes->set_robottype(str);
}

void MessageBuilder::addErrorLog(Msg& msg, const std::string& str)
{
	auto errorLog = msg.mutable_error();
	errorLog->set_message(str);
}

void MessageBuilder::addCustomMsg(Msg& msg, const std::string& key, const std::string& data)
{
	auto customMsg = msg.mutable_custommessage();
	customMsg->set_key(key);

	if(!data.empty())
		customMsg->set_data(data);
}

void MessageBuilder::addShapeUpdateInfo(Msg& msg)
{
	//is that even useful? Who knows!
}

void MessageBuilder::addChangedShape(Msg& msg, int32_t id, Vec3 center, Vec3 rotation)
{
	auto shapeUpdate = msg.mutable_shapeupdateinfo();
	addTransformToShape(shapeUpdate->add_changedshapes(), id ,center, rotation);
}

void MessageBuilder::addNewShape(Msg& msg, int32_t id, std::initializer_list<Vec3> vertices)
{
	auto shapeUpdate = msg.mutable_shapeupdateinfo();
	addVerticesToShape(shapeUpdate->add_newshapes(), id, vertices);
}

void MessageBuilder::addNewShape(Msg& msg, int32_t id, std::vector<array3> vertices)
{
	auto shapeUpdate = msg.mutable_shapeupdateinfo();
	addVerticesToShape(shapeUpdate->add_newshapes(), id, vertices);
}

void MessageBuilder::addNewShape(Msg& msg, int32_t id, std::initializer_list<array3> vertices)
{
	auto shapeUpdate = msg.mutable_shapeupdateinfo();
	addVerticesToShape(shapeUpdate->add_newshapes(), id, vertices);
}

void MessageBuilder::addDelShape(Msg& msg, int32_t id)
{
	auto shapeUpdate = msg.mutable_shapeupdateinfo();
	addVerticesToShape(shapeUpdate->add_delshapes(), id, std::initializer_list<array3>());
}

void MessageBuilder::setVelocity(Msg& msg, array3 linear, array3 angular)
{
	setLinVelocity(msg, linear);
	setAngVelocity(msg, angular);
}

void MessageBuilder::setLinVelocity(Msg& msg, array3 linear)
{
	setVec3(msg.mutable_robotvelocity()->mutable_linearvelocity(), linear);
}

void MessageBuilder::setAngVelocity(Msg& msg, array3 angular)
{
	setVec3(msg.mutable_robotvelocity()->mutable_angularvelocity(), angular);
}

void MessageBuilder::addVerticesToShape(Shape* sh, int32_t id, std::initializer_list<array3> vertices)
{
	sh->set_id(id);

	for (const auto& v : vertices)
		addVerticesToShape(sh, v);
}

void MessageBuilder::addVerticesToShape(Shape* sh, int32_t id, std::vector<array3> vertices)
{
	sh->set_id(id);

	for (const auto& v : vertices)
		addVerticesToShape(sh, {v[0], v[1], v[2]});
}

void MessageBuilder::addVerticesToShape(Shape* sh, int32_t id, std::initializer_list<Vec3> vertices)
{
	sh->set_id(id);

	for (const auto& v : vertices)
		addVerticesToShape(sh, v);
}

void MessageBuilder::addTransformToShape(Shape* sh, int32_t id, Vec3 center, Vec3 rotation)
{
	sh->set_id(id);
	sh->mutable_transform()->mutable_position()->CopyFrom(center);
	if (rotation == createVec3(array3{ 0, 0, 0 }))
		sh->mutable_transform()->mutable_rotation()->CopyFrom(rotation);
}

void MessageBuilder::addVerticesToShape(Shape* shape, const Vec3& vec)
{
	auto newVec = shape->add_vertices();
	newVec->CopyFrom(vec);
}

void MessageBuilder::addVerticesToShape(Shape* shape, array3 values)
{
	auto newVec = shape->add_vertices();
	setVec3(newVec, values);
}

void MessageBuilder::setVec3(Vec3* vec, array3 values)
{
	vec->set_x(values[0]);
	vec->set_y(values[1]);
	vec->set_z(values[2]);
}

Vec3 MessageBuilder::createVec3(float x, float y, float z)
{
	return createVec3({ x, y, z });
}

Vec3 MessageBuilder::createVec3(array3 vertices)
{
	Vec3 result;

	result.set_x(vertices[0]);
	result.set_y(vertices[1]);
	result.set_z(vertices[2]);

	return result;
}
}
