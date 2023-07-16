
# Jelly JAV

~~Don't expect perfection.~~ Aim for the perfection.

Based on the archived repo of [JellyfinJav by imaginary-upside](https://github.com/imaginary-upside/JellyfinJav). As the original creator as no activities for 2 years, I prefered to create my own repo instead of just a fork.


## Usage

When adding the media library, make sure to select "Content type: movies".

### File names example 
- abp200.mkv
- ABP200.mkv
- ABP-200.mkv
- some random text abp-200 more random text.mkv

## Metadata Providers

- ~~R18 (Videos)~~
- JAVLibrary (Videos)
- AsianScreens (Actresses)
- Warashi Asian Pornstars (Actresses)


## Installation

~~Within your Jellyfin admin panel, go to `Plugins > Repositories`, and add the following repo :
```
https://raw.githubusercontent.com/eysus/jelly-jav/master/manifest.json
```
Then switch over to the Catalog tab and install the latest version.~~

## Development
### Requirements
- Docker
- Docker Compose
- Python
- .NET 6.0
- JAV files for testing purposes are stored in videos/


### Building
```bash
./build.sh
```
Visite http://localhost:8096

### Packaging
```bash
python package.py
```
The manifest.json will update, and the package will be zipped up in `release/`

## Authors

- [@Eysus](https://github.com/eysus)
- [@imaginary-upside](https://github.com/imaginary-upside/JellyfinJav)

## Screenshots

![Grid Example](screenshots/example-grid.jpg)
![Video Example](screenshots/example-video.jpg)
![Actress Example](screenshots/example-actress.jpg)

## License

[GNU AGPL-3](https://choosealicense.com/licenses/agpl-3.0/)

