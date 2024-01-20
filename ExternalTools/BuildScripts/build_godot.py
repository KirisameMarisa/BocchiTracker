import subprocess
import config

def Build():
    subprocess.call(["python", "-m", "SConstruct"], cwd=config.cGodotPath)

if __name__ == '__main__':
    Build()