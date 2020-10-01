import getopt, sys
import requests
import json
import base64


# helper function to read vfg file
def readFile(VFGfile):
	vfg = open(VFGfile,"rb")
	allFile = vfg.read()
	vfg.close()
	return allFile
# API to download png file
def getPNG(VFGfile,URL):
	print("PNG : is downloading . . . .")
	vfgFile = readFile(VFGfile)
	files =(('vfg', (None,vfgFile)), ('fileType',(None,'png')))
	r = requests.post(URL,files=files)
	open('planimation.zip','wb').write(r.content)
	print("download is complete")
# API to download webm file
def getWebM(VFGfile,URL):
	print("WebM : is downloading . . . .")
	vfgFile = readFile(VFGfile)
	files =(('vfg', (None,vfgFile)), ('fileType',(None,'webm')))
	r = requests.post(URL,files=files)
	open('planimation.webm','wb').write(r.content)
	print("download is complete")
# API to dwonload zip file
def getGIF(VFGfile,URL):
	print("GIF : is downloading . . . .")
	vfgFile = readFile(VFGfile)
	files =(('vfg', (None,vfgFile)), ('fileType',(None,'gif')))
	r = requests.post(URL,files=files)
	open('planimation.gif','wb').write(r.content)
	print("download is complete")

# remove 1st argument
argumentL = sys.argv[1:]
options ="hi:o:"
l_options = ["help","inputfile =","outputmethod ="]
serverURL = "http://127.0.0.1:8000/downloadVisualisation"

try:
	inputfile = None
	# Parsing argument
	arguments, values = getopt.getopt(argumentL,options,l_options)

	if len(argumentL) != 4:
		print("Invalid argument, please check with -h or --help")

	#check argument
	for cArgument, cValue in arguments:
		if cArgument in ("-h","--help"):
			print ("There are two options in the command: -i and -o\n\n -i: input of your vfg file\n -o: format of your solution")

		elif cArgument in ("-i", "--inputfile"):
			print ("Displaying input value: " + cValue)
			inputfile = cValue
					

		elif cArgument in ("-o","--outputmethod"):
			if cValue=="png":
				getPNG(inputfile,serverURL)
			elif cValue=="webm":
				getWebM(inputfile,serverURL)
			elif cValue=="gif":
				getGIF(inputfile,serverURL)
			else:
				print("No such file input format")


except getopt.error as err:
	print(str(err))



