import requests
from datetime import datetime
import re


# check if the input file is in the correct format using regex
def check_input(filename, format):
    x = re.search("^.+\."+format+"$", filename)
    return bool(x)


# helper function to read file
def read_file(filename):
    file = open(filename, "rb")
    content = file.read()
    file.close()
    return content


# send 3 PDDL files and get back either vfg, png, gif, webm or mp4 file
def pddl_visualise(domain_file, problem_file, animation_profile, output_format):
    if check_input(domain_file, "pddl") is False or check_input(problem_file, "pddl") is False \
            or check_input(animation_profile, "pddl") is False:
        print("Error: incorrect input format; all input files should be in PDDL format")
        return None
    if output_format != "png" and output_format != "webm" and output_format != "gif" and output_format != "mp4" \
            and output_format != "vfg":
        print("Error: desired output format is not supported; only vfg, png, gif, webm and mp4 are supported")
        return None
    domain = read_file(domain_file)
    problem = read_file(problem_file)
    animation = read_file(animation_profile)
    files = ("domain", (None, domain)), ("problem", (None, problem)), ("animation", (None, animation)), \
            ("fileType", (None, output_format))
    r = requests.post("http://127.0.0.1:8000/upload/(?P<filename>[^/]+)$", files=files)
    if output_format == "png":
        output_format = "zip"
    output_name = "planimation " + datetime.now().strftime("%Y-%m-%d_%X.") + output_format
    with open(output_name, "wb") as output:
        for chunk in r.iter_content(chunk_size=None):
            output.write(chunk)
    return output_name


# send a vfg file and get back either png, gif, webm or mp4 file
def vfg_visualise(vfg_file, output_format):
    if check_input(vfg_file, "vfg") is False:
        print("Error: incorrect input format; input should be in vfg format")
        return None
    if output_format != "png" and output_format != "webm" and output_format != "gif" and output_format != "mp4":
        print("Error: desired output format is not supported; only png, gif, webm and mp4 are supported")
        return None
    vfg = read_file(vfg_file)
    files = ('vfg', (None, vfg)), ('fileType', (None, output_format))
    r = requests.post("http://127.0.0.1:8000/downloadVisualisation", files=files)
    if output_format == "png":
        output_format = "zip"
    output_name = "planimation " + datetime.now().strftime("%Y-%m-%d_%X.") + output_format
    with open(output_name, "wb") as output:
        for chunk in r.iter_content(chunk_size=None):
            output.write(chunk)
    return output_name
