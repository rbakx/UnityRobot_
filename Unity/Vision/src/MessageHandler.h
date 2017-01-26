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
	explicit MessageHandler(std::unique_ptr<UnityRobot::Communicator> comm) noexcept;

	void OnRecognise(const IShapeTrackers& tracker, Shape& shape) noexcept override;
	void OnLost(const IShapeTrackers& tracker, Shape& shape) noexcept override;
	void OnMove(const IShapeTrackers& tracker, Shape& shape) noexcept override;
	void OnVerticesChanged(const IShapeTrackers& tracker, Shape& shape) noexcept override;

	void SignalNewFrame(const IShapeTrackers& tracker) noexcept override;
	void SignalEndFrame(const IShapeTrackers& tracker) noexcept override;
	void ShapeDetected(const IShapeTrackers& tracker, Shape& shape) noexcept override;
private:
	int m_msgId;
	Communication::Message m_toSend;
	const std::unique_ptr<UnityRobot::Communicator> m_communicator;
};
