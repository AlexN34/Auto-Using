# Auto-Using for C#
Auto-imports and provides intellisense for references that were not yet imported in a C# file. 

![Sample](demo.gif)

Gives priority to completions that were chosen before.

![Memory](memory.gif)



----


## Changelog

### 0.2.0
Removed unnecessary files

### 0.1.0
In an attempt to prevent this extension from cluttering Intellisense:
- Import completions you have chosen before will now get an highly increased priority
- Import completions you have never chosen will be prefixed (configurable) to highly reduce their priority.

### 0.0.3
Released