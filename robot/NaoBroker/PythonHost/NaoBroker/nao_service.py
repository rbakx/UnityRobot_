import naoqi
from naoqi import ALProxy

from StringParser import de_serialiser

robots = []

robotsIdentityNumber = 1

class nao(object):
	
	def __init__(self):
		self.name = ""
		self.identity = -1
		
		self.module_tss = None
		self.module_motion = None
		self.module_posture = None
		self.module_autonomous = None
		
	def Connect(self, ip, port):
		
		success = False
		
		port = int(port)
	
		try:
		
			self.module_tss = ALProxy("ALTextToSpeech", ip, port)
			self.module_motion = ALProxy("ALMotion", ip, port)
			self.module_posture = ALProxy("ALRobotPosture", ip, port)
			self.module_autonomous = ALProxy("ALAutonomousLife", ip, port)
			
			# some basic settings
			self.module_motion.setFallManagerEnabled(True)
			self.module_motion.setTangentialSecurityDistance(0.1)
			self.module_motion.setOrthogonalSecurityDistance(0.2)
			self.module_motion.setMoveArmsEnabled(True, True)
			self.module_autonomous.stopAll()
			
			success = True
			
		except:
			self.module_motion = None
			self.module_tss = None
			self.module_posture = None
			self.module_autonomous = None
			
		return success
	
def resolveRobotFromResponseId(responseId):

	responseId = int(responseId)

	for robot in robots:	
		if robot.identity == responseId:
		
			return robot
		
	return None	
	
##=====================================================================

def nao_posture_goto(responseId, postureName = "StandInit"):

	robot = resolveRobotFromResponseId(responseId)
	if robot == None: return

	postureName = str(postureName)
	
	print "assuming posture: ", postureName
	
	robot.module_posture.goToPosture(postureName, 0.8)
	
	print "done assuming posture"
	
	return str(responseId) + "|done"

def nao_motion_movestop(responseId):

	robot = resolveRobotFromResponseId(responseId)
	if robot == None: return

	robot.module_motion.stopMove()
	
def nao_motion_sethandstate(responseId, handName = "RHand", openState = False ):
	
	robot = resolveRobotFromResponseId(responseId)
	if robot == None: return
	
	if openState:
	
		robot.module_motion.openHand(handName)
	
	else :
	
		robot.module_motion.closeHand(handName)
	
	return str(responseId)

def nao_motion_stiffness_interpolation(responseId, names="", stiffnessLists = 1.0, timeLists = 1.0):

	robot = resolveRobotFromResponseId(responseId)
	if robot == None: return

	stiffnessLists = float(str(stiffnessLists).replace(",", "."))
	timeLists = float(str(timeLists).replace(",", "."))
	
	robot.module_motion.setStiffnesses(names, stiffnessLists)
	
	return str(responseId) + "|done"
	
def nao_motion_angle_interpolation(responseId, names="", targetAngle = 0.0, targetTime = 1.0, isAbsolute = False):

	robot = resolveRobotFromResponseId(responseId)
	if robot == None: return
	
	print "nao debug: isAbsolute: ", isAbsolute
	
	wake = (isAbsolute == True or isAbsolute == 'True')
	
	
	targetAngle = float(str(targetAngle).replace(",", "."))
	targetTime = float(str(targetTime).replace(",", "."))

	robot.module_motion.setAngles(names, targetAngle, 1.0)
	
	return str(responseId) + "|done"
	
def nao_motion_moveforward(responseId, x=0.0, y=0.0, theta=0.0, frequency=1.0):

	robot = resolveRobotFromResponseId(responseId)
	if robot == None: return
	
	x = float(x.replace(",", "."))
	y =	float(y.replace(",", "."))
	theta = float(theta.replace(",", "."))
	frequency = float(frequency.replace(",", "."))
	
	robot.module_motion.moveToward(x, y, theta, [["Frequency", frequency],
												["MaxStepX", 0.065],
												["MaxStepY", 0.150],
												["MaxStepTheta", 0.36],
												["StepHeight", 0.030]])

def nao_motion_setwakestate(responseId, wake = True):

	robot = resolveRobotFromResponseId(responseId)
	
	if robot == None: return
	
	wake = (wake == True or wake == "True");
	
	if(wake):
		robot.module_motion.wakeUp()
	else:
		robot.module_motion.rest()
		
	return str(responseId)

##=======
def nao_tss_say(responseId, text=""):

	robot = resolveRobotFromResponseId(responseId)
	if robot == None: return
		
	robot.module_tss.say(text)

##=====================================================================		

def nao_connect(responseId, ip, port=9559):
	
	localRobotIdentifier = -1
	
	ip = str(ip)
	port = int(port)
	
	print "Request for nao connect: ", ip, port
	
	robot = nao()
	
	success = robot.Connect(ip, port)
	
	if success:
	
		print "Connected"
		
		robots.append(robot)
		global robotsIdentityNumber;
		
		localRobotIdentifier = robotsIdentityNumber
		robotsIdentityNumber = (localRobotIdentifier + 1)
		
		robot.identity = localRobotIdentifier
	else:
	
		print "Failed to connect"
	
	return str(responseId) + "|" + str(localRobotIdentifier)

def register_service_base(de_ser):
	
	de_ser.add_method('nao_connect', nao_connect)
	de_ser.add_method('nao_motion_angle_interpolation', nao_motion_angle_interpolation)
	de_ser.add_method('nao_motion_stiffness_interpolation', nao_motion_stiffness_interpolation)
	de_ser.add_method('nao_motion_sethandstate', nao_motion_sethandstate)
	de_ser.add_method('nao_motion_moveforward', nao_motion_moveforward)
	de_ser.add_method('nao_motion_movestop', nao_motion_movestop)
	de_ser.add_method('nao_motion_setwakestate', nao_motion_setwakestate)
	
	de_ser.add_method('nao_tss_say', nao_tss_say)
	de_ser.add_method('nao_posture_goto', nao_posture_goto)
