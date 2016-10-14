#pragma once
#include <message.pb.h>
namespace Networking
{

class MessageBuilder
{

public:
	static Communication::Message create(Communication::MessageType_ type, Communication::MessageTarget_ tgt, int32_t id);

	static void addStringData(Communication::Message& msg, const std::string& str);

	//ShapeUpdate
	static void addShapeUpdateInfo(Communication::Message& msg);
	static void addChangedShape(Communication::Message& msg, std::initializer_list<Communication::Messages::Shape_> init);
	static void addNewShape(Communication::Message& msg, int32_t id, std::initializer_list<Communication::Transform::Vector3_> vertices);
	//Velocity
	static void setVelocity(Communication::Message& msg, std::array<float, 3> linear, std::array<float, 3> angular);
	static void setLinVelocity(Communication::Message& msg, std::array<float, 3> linear);
	static void setAngVelocity(Communication::Message& msg, std::array<float, 3> angular);
private:
	static void setVec3(Communication::Transform::Vector3_* vel, std::array<float, 3> values);
};

}
