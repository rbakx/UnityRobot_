#include "MessageHandler.h"
#include <MessageBuilder.h>
#include <RobotLogger.h>
#include <communicator.h>
using MsgBuilder = Networking::MessageBuilder;

namespace
{
using Vec3 = Communication::Transform::Vector3_;
using VecArray3 = std::vector<std::array<float, 3>>;

Vec3 cvPointToVec3(const cv::Point2f& p)
{
	return MsgBuilder::createVec3(p.x, p.y, 0.0f);
}

Vec3 cvPointToVec3(const cv::Point3f& p)
{
	return MsgBuilder::createVec3(p.x, p.y, p.z);
}	

VecArray3 cvPointsToVecArray3(std::vector<Shape::coordinate_type> v)
{
	if(v.empty())
		LogError("Attempting to recognise new shape with 0 vertices.");

	VecArray3 result;
	for (const auto& coord : v)
		result.push_back({ coord.x, coord.y, 0.0f });

	return result;
}
}

MessageHandler::MessageHandler() noexcept : m_msgId(0), m_communicator(nullptr)
{
}

MessageHandler::MessageHandler(UnityRobot::Communicator* comm) noexcept : m_msgId(0), m_communicator(comm)
{
}

void MessageHandler::OnRecognise(const ShapesTracker& , Shape& shape) noexcept
{
	MsgBuilder::addNewShape(m_toSend, shape.GetTrackingId(), cvPointsToVecArray3(shape.GetShapeData()));
}

void MessageHandler::OnLost(const ShapesTracker& , Shape& shape) noexcept
{
	MsgBuilder::addDelShape(m_toSend, shape.GetTrackingId());
}

void MessageHandler::OnMove(const ShapesTracker& , Shape& shape) noexcept
{
	MsgBuilder::addChangedShape(m_toSend, shape.GetTrackingId(), cvPointToVec3(shape.Center()), cvPointToVec3(shape.Orientation()));
}

void MessageHandler::OnVerticesChanged(const ShapesTracker& , Shape& shape) noexcept
{
	MsgBuilder::addChangedShape(m_toSend, shape.GetTrackingId(), cvPointsToVecArray3(shape.GetShapeData()));
}

void MessageHandler::ShapeDetected(const ShapesTracker& , Shape& ) noexcept
{
}

void MessageHandler::SignalNewFrame(const ShapesTracker& ) noexcept
{
	//m_toSend.Clear();
	m_toSend = MsgBuilder::create(Communication::MessageType_::ShapeUpdate, Communication::MessageTarget_::Unity, m_msgId++);
}

void MessageHandler::SignalEndFrame(const ShapesTracker& ) noexcept
{
	m_communicator->sendCommand(m_toSend);
}

void MessageHandler::setCommunicator(UnityRobot::Communicator* comm) noexcept
{
	m_communicator = comm;
}


