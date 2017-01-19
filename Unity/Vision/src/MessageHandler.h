#pragma once
#include "framereaders/robotmapping/IWorldMappingEventSubscriber.hpp"
#include <MessageBuilder.h>

using namespace robotmapping;
namespace UnityRobot 
{
	class Communicator;
}

class MessageHandler : public robotmapping::IWorldMappingEventSubscriber
{
public:
	MessageHandler() noexcept;
	explicit MessageHandler(UnityRobot::Communicator* comm) noexcept;

	void OnRecognise(const ShapesTracker& tracker, Shape& shape) noexcept override;
	void OnLost(const ShapesTracker& tracker, Shape& shape) noexcept override;
	void OnMove(const ShapesTracker& tracker, Shape& shape) noexcept override;
	void OnVerticesChanged(const ShapesTracker& tracker, Shape& shape) noexcept override;

	void SignalNewFrame(const ShapesTracker& tracker) noexcept override;
	void SignalEndFrame(const ShapesTracker& tracker) noexcept override;
	void ShapeDetected(const ShapesTracker& tracker, Shape& shape) noexcept override;

	void setCommunicator(UnityRobot::Communicator* comm) noexcept;
private:
	int m_msgId;
	Communication::Message m_toSend;
	UnityRobot::Communicator* m_communicator;
};
