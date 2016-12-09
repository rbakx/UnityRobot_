# First, import some modules
from StringParser import de_serialiser
from NaoBroker import nao_service

# Standard libraries
import thread
import time

# Import the ctypes library which allows for loading c-ABI defined shared libraries
import ctypes
import sys

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

print ("Loading library...")

# Get the library space of this plugin
library_space = ctypes.cdll.LoadLibrary(library_file_path)

# Library space available; try to run a loop for the library _main
try:
   thread.start_new_thread( library_main_loop, (library_space, ) )
except:
   print ("Error: unable to start thread for library _main: ", library_file_path)
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