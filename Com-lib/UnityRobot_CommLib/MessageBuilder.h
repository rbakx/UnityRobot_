#pragma once
#include "proto/message.pb.h"

namespace Networking
{

class MessageBuilder
{

public:
	static Communication::Message create(Communication::MessageType_ type, Communication::MessageTarget_ tgt, int32_t id);

	static void addStringData(Communication::Message& msg, const std::string& str);
	//robot-Unity
	static void addIdentificationRes(Communication::Message& msg, const std::string& str);
	static void addErrorLog(Communication::Message& msg, const std::string& str);
	static void addCustomMsg(Communication::Message& msg, const std::string& key, const std::string& data = "");
	//ShapeUpdate
	static void addShapeUpdateInfo(Communication::Message& msg);
	static void addChangedShape(Communication::Message& msg, int32_t id, std::initializer_list<Communication::Transform::Vector3_> vertices);
	static void addChangedShape(Communication::Message& msg, int32_t id, std::initializer_list<std::array<float, 3>> vertices);
	static void addNewShape(Communication::Message& msg, int32_t id, std::initializer_list<Communication::Transform::Vector3_> vertices);
	static void addNewShape(Communication::Message& msg, int32_t id, std::initializer_list<std::array<float, 3>> vertices);
	//Velocity
	static void setVelocity(Communication::Message& msg, std::array<float, 3> linear, std::array<float, 3> angular);
	static void setLinVelocity(Communication::Message& msg, std::array<float, 3> linear);
	static void setAngVelocity(Communication::Message& msg, std::array<float, 3> angular);
private:
	static void addShape(Communication::Messages::Shape_* sh, int32_t id, std::initializer_list<Communication::Transform::Vector3_> vertices);
	static void addShape(Communication::Messages::Shape_* sh, int32_t id, std::initializer_list<std::array<float, 3>> vertices);

	static void addVerticesToShape(Communication::Messages::Shape_* shape, const Communication::Transform::Vector3_& vec);
	static void addVerticesToShape(Communication::Messages::Shape_* shape, std::array<float, 3> values);
	static void setVec3(Communication::Transform::Vector3_* vel, std::array<float, 3> values);
	static Communication::Transform::Vector3_ createVec3(std::array<float, 3> vertices);
};

}
