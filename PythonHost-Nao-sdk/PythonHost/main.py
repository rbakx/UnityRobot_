# First, import some modules
from StringParser import de_serialiser
from NaoBroker import nao_service

# Standard libraries
import thread
import time

# Import the ctypes library which allows for loading c-ABI defined shared libraries
import ctypes
import sys

from controller import xinput

####################

# Some definitions

library_file_path = "nao_broker"

continueProgram = True

####################

# The C# plugin will run its own main loop. Python is in charge of spawning the loop and calling _main function of the library
# library_main_loop is the python function that calls the _main method from the library
def library_main_loop( lib_space):

	print sys.argv

	startArgs = '|'.join(str(s) for s in sys.argv);
	

	try:
		lib_space._main(startArgs)
		print 'library _main returned'
	except:
		print "Unexpected error: ", sys.exc_info()[1], " - " , sys.exc_info()[2]
		time.sleep(0.010)
		global continueProgram
		continueProgram = False
		exit()
		
# Get the name of the digital-state buttons
switcher =
{
	0: "[UNKNOWN]",
	1: "DPAD-UP",
	2: "DPAD-DOWN",
	3: "DPAD-LEFT",
	4: "DPAD-RIGHT",
	5: "START",
	6: "BACK",
	7: "LEFT_THUMBSTICK_PRESS",
	8: "RIGHT_THUMBSTICK_PRESS",
	9: "LEFT_BUMPER",
	10: "RIGHT_BUMPER",
	11: "[UNKNOWN]",
	12: "[UNKNOWN]",
	13: "A",
	14: "B",
	15: "X",
	16: "Y",
	17: "[UNKNOWN]",
	18: "[UNKNOWN]",
}
		
def controller_input_loop(lib_space):

	while(continueProgram):
	
		# Get controllers, assume they are xbox 360 controllers (due to mapping)
	
		joysticks = xinput.XInputJoystick.enumerate_devices()
		device_numbers = list(map(xinput.attrgetter('device_number'), joysticks))

		lj = len(joysticks)
		
		if lj < 0: continue
		
		print('found %d devices: %s' % (len(joysticks), device_numbers))
		
		for stick in joysticks:
			
			print('Using controller %d' % stick.device_number)
		
			@stick.event
			def on_button(button, pressed):
				
				lib_space._controller_propagate_digital(j.device_number, switcher.get(button), pressed);

			left_speed = 0
			right_speed = 0

			@stick.event
			def on_axis(axis, value):
			
				intval = int(value*32767);
				lib_space._controller_propagate_analog(j.device_number, axis, intval);

		while True:
			
			for stick in joysticks:
			
				j = stick
				j.dispatch_events()
				
			time.sleep(.01)
			
		else:
			time.sleep(0.010)
	
			#try:
				
			#except:
			#	print "Unexpected error: ", sys.exc_info()[1], " - " , sys.exc_info()[2]
			#	exit()*/

###################	

print ("Loading library...")

# Get the library space of this plugin
library_space = ctypes.cdll.LoadLibrary(library_file_path)

# Library space available; try to run a loop for the library _main
try:
   thread.start_new_thread( library_main_loop, (library_space, ) )
except:
   print ("Error: unable to start thread for library _main: ", library_file_path)
   exit()
   
   
 
#
try:
   thread.start_new_thread( controller_input_loop, (library_space, ) )
except:
   print ("Error: unable to start thread for controller_input_loop")
   exit()
   
# Spawn a de-serialiser to process string commands
string_parser = de_serialiser.de_serialiser()

# Register nao commands to deserialiser
nao_service.register_service_base(string_parser)

# Loop always
while(continueProgram):

	cmd_str = "-"
	
	# While there are commands queued by the library
	while(continueProgram):
	
		# Get the command
		cmd_str = ctypes.c_char_p(library_space.CallCycle()).value
	
		# Does it have any content or no commands buffered?
		if len(cmd_str) > 0:
			
			# Process string as possible command
			result = string_parser.process(cmd_str)
		
			if isinstance(result, basestring):
				library_space.RequestCompleted(result);
			
		else:
			# No more commands!
			break

	# No commands in queue; wait 2 ms; this is justified because it is well within the real-time domain
	time.sleep(0.002)