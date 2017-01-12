import inspect

def program_exit():
	exit()
	
def program_log(responseId, argument):
	print "[LOG-C#] ", argument
	
def program_exception(responseId, argument):
	print "[EXCEPTION-C#] ", argument
	exit(1)

class de_serialiser(object):
	
		def __init__(self):
			
			self.funcdict = {
				'exit': program_exit,
				'log': program_log,
				'except': program_exception
			}
			
		def add_method(self, method_name, method_pointer):
			self.funcdict[method_name] = method_pointer
			
		def process(self, toParseString):
		
			#print "[DEBUG] ", toParseString
		
			value = 0
		
			toParseString = toParseString.split('|')
			
			isDefinedMethod = False
			method = 0
			
			try:
				method = self.funcdict[toParseString[1]]
			
				isDefinedMethod = callable(method)
			
			except:
				isDefinedMethod = False
			
			if isDefinedMethod == True:
				
				toParseString.pop(1)
				
				numargssupplied = len(toParseString)
				
				numargs = numargssupplied - method.func_code.co_argcount
				
				while numargs > 0:
					numargs = numargs-1
					toParseString.pop(numargs)
				
				#print toParseString
				
				value = method(*toParseString)
				
			else:
				print "Unsupported command: ", toParseString
				
			return value