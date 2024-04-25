# Cloak Obfuscator
Cloak is a .NET obfuscator written in C#. It's primary goal is to be
more of a learning tool than a good obfuscator, but it can double as both :smile:

## Features
- [X] String Encryption
- [X] Control Flow
- [ ] Call Proxying
- [ ] Renaming
- [X] Int Encryption

## TODO
- [ ] Add a settings system to the protections
- [ ] Make a graphical interface
- [ ] Incorporate GitHub Actions for automatic builds
- [ ] Test and improve control flow, it hasn't had testing and may break large functions
- [ ] Thoroughly test the obfuscator as a whole

## Project Structure
- `Cloak.Cli` - The command line interface for cloak
- `Cloak.Gui` - An in-progress graphical interface for cloak
- `Cloak.Core` - The core functionality for Cloak, contains all transformations and the obfuscation pipeline
- `Cloak.Runtime` - This runtime gets injected into obfuscated applications
- `TestFile` - A basic file used for obfuscation testing and demonstration

## Credits
- [Washi1337](https://github.com/Washi1337): His amazing contributions in [AsmResolver](https://github.com/Washi1337/AsmResolver) and [Echo](https://github.com/Washi1337/Echo) have made this possible. Lots of love to him.
- [AvaloniaUI](https://www.avaloniaui.net): An amazing .NET ui framework that powers Cloak's GUI