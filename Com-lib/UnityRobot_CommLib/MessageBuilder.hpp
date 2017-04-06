#pragma once
#include <array>

#include <IPresentationProtocol.hpp>
#include <message.pb.h>

namespace Networking
{

class MessageBuilder
{
using Msg = Communication::Message;
using Vec3 = Communication::Transform::Vector3_;
using Shape = Communication::Messages::Shape_;
using array3 = std::array<float, 3>;

public:
	static Msg create(Communication::MessageType_ type, Communication::MessageTarget_ tgt, int32_t id);

	static void addStringData(Msg& msg, const std::string& str);
	//robot-Unity
	static void addIdentificationRes(Msg& msg, const std::string& str);
	static void addErrorLog(Msg& msg, const std::string& str);
	static void addCustomMsg(Msg& msg, const std::string& key, const std::string& data = "");
	//ShapeUpdate
	static void addShapeUpdateInfo(Msg& msg);
	static void addChangedShape(Msg& msg, int32_t id, Vec3 center, Vec3 rotation = createVec3(array3{0, 0, 0}));
	static void addChangedShape(Msg& msg, int32_t id, std::vector<array3> vertices);
	static void addNewShape(Msg& msg, int32_t id, std::initializer_list<Vec3> vertices);
	static void addNewShape(Msg& msg, int32_t id, std::vector<array3> vertices);
	static void addNewShape(Msg& msg, int32_t id, std::initializer_list<array3> vertices);
	static void addDelShape(Msg& msg, int32_t id);
	//Velocity
	static void setVelocity(Msg& msg, array3 linear, array3 angular);
	static void setLinVelocity(Msg& msg, array3 linear);
	static void setAngVelocity(Msg& msg, array3 angular);

	static Vec3 createVec3(float x, float y, float z);
	static Vec3 createVec3(array3 vertices);
private:
	static void addVerticesToShape(Shape* sh, int32_t id, std::initializer_list<Vec3> vertices);
	static void addVerticesToShape(Shape* sh, int32_t id, std::vector<array3> vertices);
	static void addVerticesToShape(Shape* sh, int32_t id, std::initializer_list<array3> vertices);

	static void addTransformToShape(Shape* sh, int32_t id, Vec3 center, Vec3 rotation = createVec3(array3{ 0, 0, 0 }));

	static void addVerticesToShape(Shape* shape, const Vec3& vec);
	static void addVerticesToShape(Shape* shape, array3 values);
	static void setVec3(Vec3* vel, array3 values);
};

}
