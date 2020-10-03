#!/usr/bin/env python3
import sys,os
import planimation_api as api

USAGE_STRING = """
No command-line options given. Usage:

cli_planimation.py submitVFG 		[obsolute path of vfg file] [download option (png/webm/gif)]
cli_planimation.py submitPlan 		[obsolute path of domain file] [obsolute path of problem file] [obsolute path of animation file] [download option (png/webm/gif)]
"""


if __name__ == "__main__":
	# give the usage of command line
	if len(sys.argv) == 1:
		print(USAGE_STRING)
		exit(0)
	i=1
	while i< len(sys.argv):
		#check submitVFG function
		if sys.argv[i] == "submitVFG":
			i+=1
			if len(sys.argv) == 4:
				vfg = sys.argv[i].strip()

				if vfg.endswith('.vfg'):
					i+=1
					downloadtype = sys.argv[i].strip()
					
					if downloadtype!='png' and downloadtype!='webm' and downloadtype!='gif':
						print("invalid download type")
						exit(1)
					else:
						print(vfg)
						print(downloadtype)
						api.vfg_visualise(vfg,downloadtype)
						exit(0)
				else:
					print("Invalid vfg file type")
					exit(1)
			else:
				print("Invalid argument entered for submitVFG")
				exit(1)
		# check submitPlan function 		
		elif sys.argv[i] == "submitPlan":
			i+=1
			if len(sys.argv) == 6:
				domainFile = sys.argv[i].strip()
				i+=1
				problemFile = sys.argv[i].strip()
				i+=1
				animationFile = sys.argv[i].strip()
				if domainFile.endswith('.pddl') and problemFile.endswith('.pddl') and animationFile.endswith('.pddl'):
					i+=1
					downloadtype = sys.argv[i].strip()
					if downloadtype!='png' and downloadtype!='webm' and downloadtype!='gif':
						print("invalid download type")
						exit(1)
					else:
						api.pddl_visualise(domainFile,problemFile,animationFile,downloadtype)
						exit(0)

				else:
					print("invalid input for pddls,please check your pddl files")
					exit(1)


			else:
				print("Invalid argument numbers entered for submitPlan")
				exit(1)

		else:
			print("no such argument input!!!")
			exit(1)