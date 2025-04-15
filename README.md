# CocoR.Generator

CocoR.Generator is a tool that automatically generates C# code from Coco/R attributed grammar files (`.atg`) and template files (`.frame`) during the build process.

## Based On Coco/R

This project is based on the source code of the Coco/R compiler generator, available at: [https://ssw.jku.at/Research/Projects/Coco/](https://ssw.jku.at/Research/Projects/Coco/)

## License

Coco/R is distributed under the terms of the GNU General Public License (with slight extensions). See: [https://ssw.jku.at/Research/Projects/Coco/Doc/license.txt](https://ssw.jku.at/Research/Projects/Coco/Doc/license.txt)

## What CocoR.Generator Does

- Detects `.atg` attributed grammar files and `.frame` template files in your project
- Automatically generates the corresponding C# scanner and parser
- Includes the generated code in the build output
