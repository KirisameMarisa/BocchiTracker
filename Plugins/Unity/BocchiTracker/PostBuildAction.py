import os
import shutil
import sys
from pathlib import Path

if len(sys.argv) != 3:
    print("Usage: python script.py $(OutDir) $(SolutionDir)")
    sys.exit(1)

cOutDir      = Path(sys.argv[1])
cSolutionDir = Path(sys.argv[2])
cVisualStudioArtifactFiles = ["BocchiTracker.dll", "package.json"]
cArtifactDir = cSolutionDir / ".." / "Artifact"

os.makedirs(cArtifactDir, exist_ok=True)
for file in cVisualStudioArtifactFiles:
    src_path = cOutDir / file
    dest_path = cArtifactDir / file
    shutil.copy2(src_path, dest_path)