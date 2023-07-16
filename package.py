#!/usr/bin/env python
import xml.etree.ElementTree as ET
import json
import re
import subprocess
import shutil
from datetime import datetime
from pathlib import Path
from hashlib import md5

project_name = "JellyJav"
dotnet_version = "net6.0"
maintainer = "Eysus"

tree = ET.parse(f"{project_name}/{project_name}.csproj")
version = tree.find("./PropertyGroup/AssemblyVersion").text
targetAbi = tree.find("./ItemGroup/*[@Include='Jellyfin.Model']").attrib["Version"]
targetAbi = re.sub("-\w+", "", targetAbi) # Remove trailing release candidate version.
timestamp = datetime.now().strftime("%Y-%m-%dT%H:%M:%SZ")

meta = {
    "category": "Metadata",
    "guid": "5a771ee2-cec0-4313-b02b-733453b1ba5b",
    "name": project_name,
    "description": "JAV metadata providers for Jellyfin.",
    "owner": maintainer,
    "overview": "JAV metadata providers for Jellyfin.",
    "targetAbi": f"{targetAbi}.0",
    "timestamp": timestamp,
    "version": version
}

Path(f"release/{version}").mkdir(parents=True, exist_ok=True)
print(json.dumps(meta, indent=4), file=open(f"release/{version}/meta.json", "w"))

subprocess.run([
    "dotnet",
    "build",
    f"{project_name}/{project_name}.csproj",
    "--configuration",
    "Release"
])

shutil.copy(f"{project_name}/bin/Release/{dotnet_version}/{project_name}.dll", f"release/{version}/")
shutil.copy(f"{Path.home()}/.nuget/packages/anglesharp/0.14.0/lib/netstandard2.0/AngleSharp.dll", f"release/{version}/")

shutil.make_archive(f"release/jellyjav_{version}", "zip", f"release/{version}/")

entry = {
    "checksum": md5(open(f"release/jellyjav_{version}.zip", "rb").read()).hexdigest(),
    "changelog": "",
    "targetAbi": f"{targetAbi}.0",
    "sourceUrl": f"https://github.com/Eysus/jelly-jav/releases/download/{version}/jellyjav_{version}.zip",
    "timestamp": timestamp,
    "version": version
}

manifest = json.loads(open("manifest.json", "r").read())

if len(manifest[0]["versions"]) > 0:
    if manifest[0]["versions"][0]["version"] == version:
        del manifest[0]["versions"][0]

manifest[0]["versions"].insert(0, entry)
print(json.dumps(manifest, indent=4), file=open("manifest.json", "w"))