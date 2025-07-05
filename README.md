# More Customizations

![Thumbnail](https://github.com/Creta5164/peak-more-customizations/raw/main/icon.png)

This mod manages to allow adding more customization options by utilizing AssetBundle.

Most parts are inspired from [CustomHats](https://github.com/radsi/PEAK-CustomHats).

# Features

- Adds customization options from mod directory
- Supported various customization type
  - Accessory (Glasses, hair, etc.)
  - Mouth
  - Eye
  - Hat

# Limitation

- Skin and wear is not supported at the moment. (contribution are welcome!)
- It'll only show correctly if other players has exactly same customization files.
  - This is caused by save data are order-dependent.

# Dependencies

- [Passport Pagination](https://github.com/radsi/PEAK-PassportPagination)
  - This allows to navigate more then passport page's option count so it's must be required.

# Creating your customization

## Prerequisites

- **Unity Editor** *(6.0)*
  - Required for creating asset bundle.
- **Image editing tool** *(Photoshop, Affinity, Krita, etc.)*
  - Required if you want to add face related types.
- **Modeling tool** *(Recommended to use Blender)*
  - Required if you want to add hat types.

## Table of contents

- [Accessory](https://github.com/creta5164/peak-more-customizations/tree/main/docs/accessory.md)
- [Mouth](https://github.com/creta5164/peak-more-customizations/tree/main/docs/mouth.md)
- [Eyes](https://github.com/creta5164/peak-more-customizations/tree/main/docs/eyes.md)
- [Hat](https://github.com/creta5164/peak-more-customizations/tree/main/docs/hat.md)
- [Build asset bundle](https://github.com/creta5164/peak-more-customizations/tree/main/docs/build-asset-bundle.md)
- [Create customization](https://github.com/creta5164/peak-more-customizations/tree/main/docs/create-customization.md)

# Build from source

You can build from source with [.NET SDK](https://dot.net), this project requires .NET 9.0.

## Configure csproj file

Open `MoreCustomizations.csproj` file, then find `GameDir` and `DestDir` property.

You need to change it for your environment.

![How it should be](https://github.com/creta5164/peak-more-customizations/tree/main/docs/img/build-from-source-1.png)

- `GameDir` : Installation path of PEAK. (i.e. it should be ended with `...steamapps/common/PEAK`)
- `DestDir` : Distribution path of compiled results. (i.e. parent directory of `BepInEx`.)

## Testing

Open this repository in terminal, run below command will compile and distribute result to your `BepInEx` plugins path.

```
dotnet build ./src
```

You can create [ThunderStore](https://thunderstore.io/c/peak) package with below command.

```
dotnet publish ./src --configuration Release
```

This will produces package files to `src/bin/ThunderStore`.