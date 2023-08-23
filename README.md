ZeroDayToolKit - Fanmade Hacknet Expansion
===
a [Hacknet](https://store.steampowered.com/app/365450/Hacknet/) [Pathfinder](https://github.com/Arkhist/Hacknet-Pathfinder) mod that aims to provide tools for better immersion and tighter gameplay.

Please DM or mention **@pr_d** on discord if you encounter any bugs or weirdness using the mod.

## New Commands
- `btoa <file>` / `atob <file>`: Encode/Decode files to Base64.
- `binencode <file>` / `bindecode <file>`: Encode/Decode files to Binary.
- `zip <folder>` / `unzip <folder>`: Zip/Unzip folder via gZip.
- `touch <filename> [content...]`: Create a file.
- `mkdir <foldername>`: Create a folder.
- `/ <message...>`: Chat in IRC/DHS.

## New Executables
Every executables except for the ones labelled feature custom-made Hacknet-style graphics.
- `#SSH_SWIFT#` / `>S>.SSH.$w!f7.>S>`: Opens port `22 (SSH)`.
  - Counterpart to `FTPSprint`, it vastly speeds up SSH break-in compared to vanilla.
- `#PACKET_HEADER_INJECTION#` / `:pkthdr::`: Opens port `80 (HTTP WebServer)`.
  - Takes more time, but vastly less RAM compared to vanilla. Useful for packing in remaining spaces.
- `#SQL_TX_CRASHER#` / `THE_LAST_RESORT`: Opens port `1433 (SQL Server)`.
  - This program will **re-enable** firewall and proxy if the target has any.
  - This program stays for long after the process is completed, encouraging players to manually `kill` it.
- `#PORT_BACKDOOR#` / `thanks from /el/ <3`: Opens port `0 (Backdoor Connection)`.
  - Port `0 (Backdoor Connection)` can be opened on **ANY OS**, essentially working as a free port.
  - This process takes a long, **long** time.
- `#GIT_TUNNEL#` / `GitTunnel For Hacknet BETA`: Opens port `9418 (GitTunnel)`. (NO GRAPHICS YET)
- `#MQTT_INTERCEPTOR` / `mqttpwn:r63`: Opens port `1883 (MQTT Protocol)`. (NO GRAPHICS YET)

## New Extension Actions
- `<SetRAM ram="float">`: Sets the RAM amount of the player.
- `<RunCommand command="string">`: Runs a command on that OS.
- `<ResetIRCDelay target="computer_id">`: Resets the IRC Delay for that OS, useful for branching IRC logs. (see Conditions)
- `<SetNumberOfChoices choices="int">`: Sets the number of choices for interactive IRC logs. (see Conditions)
- `<DisableCommand command="string">` / `<EnableCommand command="string">`: Prohibits usage of certain commands, useful for blocking chat by disabling `/`. (see Conditions)

## New Extension Conditions
- `<OnFileCreation [target="computer_id" path="path" name="string" content="string"]>` / `<OnFileDeletion [target="computer_id" path="path" name="string" content="string"]>`: Triggered when a specific file is created or deleted on an OS.
- `<OnIRCMessageAny [target="computer_id" user="string" notUser="string" minDelay="float" maxDelay="float" requiredFlags="string" doesNotHaveFlags="string"]>`: Triggered when a user sends a message to the IRC chat. set `user="#PLAYERNAME#` to have it trigger when a user sends something.
- `<OnIRCMessage [target="computer_id" user="string" notUser="string" minDelay="float" maxDelay="float" requiredFlags="string" doesNotHaveFlags="string" word="string"]>`: Triggered when a specific word is said on IRC. Case and spacing insensitive.
- `<OnIRCMessageTone [target="computer_id" user="string" notUser="string" minDelay="float" maxDelay="float" requiredFlags="string" doesNotHaveFlags="string" tone="string"]>`: Triggered when a user responds with a specific type of message. currently supported tones are `no`, `yes`, `help`, `hey` and number `1` through `10`.

The Following conditions are meant to be used alongside the TraceV2 system. see below for more information.
- `<OnCrash [requiredFlags="string" doesNotHaveFlags="string" targetComp="computer_id" targetNetwork="network_id"]>`: When that OS or Network's "Head" has crashed due to `ForkBomb` and such. Defaults to the current OS/Network if written within a `TraceV2` or `Computer` tag.
- `<OnReboot [requiredFlags="string" doesNotHaveFlags="string" targetComp="computer_id" targetNetwork="network_id"]>`: When that OS or Network's "Head" has rebooted after a crash.
- `<OnRebootCompleted [requiredFlags="string" doesNotHaveFlags="string" targetNetwork="network_id" RequireLogsOnSource="bool" RequireSourceIntact="bool"]`: When that Network has finished its Retrace timer. only fires `RequireLogsOnSource` if any of the Network's devices have logs on them, and only fires `RequireSourceIntact` if `sys/netcfgx.dll` is still on any of the devices.
- `<OnHostileActionTaken [requiredFlags="string" doesNotHaveFlags="string" targetComp="computer_id" targetNetwork="network_id"]>`: when a hostile action (hacking attempt, commonly tripping trace) is taken.

## TraceV2/`Track`: Brand new type of `Trace` timer
### Networks
Before I introduce TraceV2, we must create a `Network`. a Network is a group of nodes with a `head` node that functions as a group. To create a Network, create a `/Networks` folder in your Extension folder, and add an XML file [like such](https://github.com/prodzpod/ZeroDayToolKit/blob/main/TestExtension/Networks/test.xml).

Networks may have following properties alongside `Computer`s and aforementioned conditions:
- `<trace time="float">`: Determines the `Track` time.
- `<reboot time="float">`: Determines the `Retrace` time.
- `<onStart>`: shorthand for `OnHostileActionTaken`.
- `<onCrash>`: shorthand for `OnCrash`.
- `<onComplete>`: shorthand for `OnRebootCompleted`.
- `<afterComplete [requiredFlags="string" doesNotHaveFlags="string" RequireLogsOnSource="bool" RequireSourceIntact="bool"] every="int" offafter="int">`: triggers after `OnRebootCompleted`, counter increases every connection / `connect` you make. if its equal to every, `afterComplete` triggers and the counter resets. if it triggered `offafter` times, the counter shuts off. the counter shuts off prematurely if `RequireLogsOnSource` or `RequireSourceIntact` fails at any moment.
  - This is to keep haunting the player after they have failed to remove logs or something, so that they go back and remove logs, idk, go wild.

### Trace V2
Trace V2 is a Network-wide trace that does not go away when `connect`ed away from the OS. it acts similarly to screenbleed, but without all the effects so it can be used more liberally. 

Once any OS in the system detects hostile activity, a cyan `Track` timer turns on that fails similarly to Trace.

Once the `head` of the Network is `ForkBomb`ed, `Track` timer will change to a gray `Retrace` timer, letting you close connections and forkbomb the rest of the OSes, delete the logs, etc.

Once the `Retrace` period is over and everything is good, it is considered that the player has won.

See [This Video](https://youtu.be/uQaObgut0p0) for a visual showcase of Trace V2 and other things this mod adds.

## Localization Support
You can set the extension's language to `dynamic` to enable multiple locales in a single extension.

Include `{{key}}` in place of the text to make it be influenced by locales.

create a `/Locales` folder in your extension, and put xml files in there.
here is an example of such a file.

```xml
<Locale>
  <en-us>
    <l key="back">Back</l>
    <l key="ok">OK</l>
    <l key="cancel">Cancel</l>
  </en-us>
</Locale>
```

you may also put these files inside `.../Hacknet/locales/Custom` to apply the translation key for all extensions.

If downloaded via the installer, the mod also adds **complete font support** for Korean.

## Other, Miscellaneous Tweaks...
- **SFX Volume Slider**. adjustable in options.
- You can now use `Sequencer.exe -i` (also applies to Kaguya's Trial) to instantly start the challenge.
- You can now use `ComShell.exe -t` to trap all shells.
- A typo in usage message for `ComShell` is fixed.
- Tracking Logs are smarter. (does not count connect/disconnect logs)

## Changelog
- 0.2.1: Updated to PF 5.3.0, Fixed IRC condition bug, made dynamic locale read a lot faster (appearantly +=ing string a million times is not good for your OS), added custom command description, and change description based on disabled commands for `help` menu. Added proper README.
- 0.2.0: Added graphics for PortBackdoor, Dynamic locale support, Sound Effect Volume setting
- 0.0.2: Refactored half the mod
- 0.0.1: Initial release