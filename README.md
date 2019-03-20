# AGS SpeechSkip Tool
This program allows you to change a way speech lines in AGS games can be skipped.

Are you're tired of not being able to read subtitles in time or, perhaps,
you don't want to keep pressing buttons to get to the next speech line?
Maybe you hate skipping dialogs accidentally when your cat jumps on the keyboard out of nowhere?
Well, worry no more! Or do. Either way, this program might help you. Or it might not.

### Compilation
You will need .NET Framework 3.5 or above and Microsoft Visual Studio 2010 or above.
Open a solution (.sln) file and it should be able to build.

### Supported versions
Well... We're dealing with AGS, alright? It _will_ break. When it does feel free to open an issue.

### Tested AGS versions
* 3.4.1 P4
* 3.4.0
* 3.3.4
* 3.3.3
* 3.3.0
* 3.2.1

### Note on saves
Value of speech skip type variable is stored in savefile, so this program will _only_
have effect on a fresh new game start.

Since save file format is "great" and writing a parser for it is not a ridiculous task on people's lists,
it's much easier to load a savefile, modify value using a debugger and resave it than searching for 1 byte
in a middle of dozens of zeroes.

If you really need to use your savefile then copy a search pattern for SetSkipSpeech function
from "Patcher/SpeechSkipPatcher.cs" and scan for it in game executable or acwin.
Once you've found it, look for instruction that moves register value into a memory pointed to
by a constant pointer into data segment (e.g. mov dword ptr ds:[5CF610],eax).
That pointer (in this case to 0x5CF610) should point to a variable you have to change.
Set it to whatever you need and in case it works just make a new save and there you go.
Sometimes this instruction can be located inside of inner functions, so in that case jump
through call instructions.

### How it works
Adventure Game Studio has different ways to skip speech lines built-in (and, yeah, it's also bugged).
The idea is to change default value set by the game developer to the one we need.
This value sits inside of DTA (game data) file that is (always?) located in game executable.

The trick is that engine has a function (SetSkipSpeech) that can change that value at runtime exposed to script API,
which renders our "solution" useless if that function is used by the game (e.g. Blackwell Unbound).
So we just patch that function to return immediately and as a result it cannot change values anymore.
In theory it can break some games that rely on this function (or value) heavily, but realistically speaking
not sure if it's even possible.
