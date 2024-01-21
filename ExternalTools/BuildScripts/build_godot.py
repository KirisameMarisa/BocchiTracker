import subprocess
import config

def Build():
    subprocess.call(["python", "-m", "SCons"], cwd=config.cGodotPath)

if __name__ == '__main__':
    Build()
