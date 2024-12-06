ZeroDayToolKit - Build-Your-Own Community DLC
===

https://github.com/user-attachments/assets/0c2d327e-6e0c-4b74-893f-b83644e8917e

a [Hacknet](https://store.steampowered.com/app/365450/Hacknet/) [Pathfinder](https://github.com/Arkhist/Hacknet-Pathfinder) mod that aims to provide tools for better immersion and tighter gameplay.

Please DM or mention **@pr_d** on discord if you encounter any bugs or weirdness using the mod.

## New Commands
Some commands are DISABLED BY DEFAULT as it can either ruin the immersion, or vastly change how one approaches cracking, and can be considered cheating if used in a non-0DTK extension / basegame. you may enable these features via `<EnableCommand>` explained below.
- `alias <cmd> <expression...>` / `unalias <cmd>`: sets an alias for this command. You can se `$0`~`$9`, `$#` and `$*` to pass parameters. (DISABLED BY DEFAULT)
- `btoa <file>` / `atob <file>`: Encode/Decode files to Base64.
- `binary <-e/-d> <file>`: Encode/Decode files to Binary.
- `cp <file> <path>`: copies a file to another folder.
- `date`: Prints current time to console. (DISABLED BY DEFAULT)
- `echo [content...]`: Prints content to console.
- `expr <expression>`: Resolve a math expression.
- `head <file> [line]` / `tail <file> [line]`: Prints first/last (default: 10) lines of the file to console.
- `history`: Prints command history to console.
- `hostname [-i]`: Prints the name (or IP address if -i is provided) of the connected device.
- `last`: prints the history of user connections based on logs.
  - alias: `w`
- `man <command>`: Prints help for that command.
- `mkdir <foldername>`: Create a folder.
- `ping <ip>`: Checks for the computer's existence without connecting.
- `pwd`: Prints the current working directory.
- `radio <file>`: (IF [STUXNET](https://github.com/AutumnRivers/StuxnetHN/releases/latest) IS INSTALLED) adds the contents of the audio file in `RadioV3`, similar to code redemption.
- `rmdir <foldername>`: Remove a folder. (DISABLED BY DEFAULT)
- `send <message...>`: Send chat in IRC/DHS. use `\n` to send newlines. (DISABLED BY DEFAULT)
  - aliases: `/ <message>`, `> <message>`, `irc <message>`
- `shutdown <time in seconds OR "now">`: reboots this pc after a given time has passed.
- `source <file>`: execute the file as a newline-delimited list of commands. You can se `$0`~`$9`, `$#` and `$*` to pass parameters. (DISABLED BY DEFAULT)
- `touch <filename> [content...]`: Create a file.
- `wc <file>`: prints line, word and byte count for this file.
- `who`: prints the current active account in the connected computer.
  - alias: `whoami`
- `zip <folder>` / `unzip <folder>`: Zip/Unzip folder via gZip.
- new aliases: `netstat` for `probe`, `ip addr show` / `ip address show` for `scan`, `ip addr add` / `ip address add` for `connect`.

## Chaining and Piping
- You are able to type multiple commands delimited by special set of characters.
- `<command1>; <command2>`: execute the two commands in order.
- `<command1> && <command2>`: executes command1, then only execute command2 if command1 succeeded.
- `<command1> || <command2>`: opposite of `&&`, only executes command2 if command1 fails.
- `<command1> | <command2>`: executes command1, and then executes command2 with the output of command1 appened to it as an argument.
- `<command1> >> <file>`: executes command1, and then stores the output of the command in the file.
- some commands (such as `touch`) ignore chaining characters so that they can be typed in.

## New Executables
Every executables except for the ones labelled feature custom-made Hacknet-style graphics.
- `#SSH_SWIFT#` / `>S>.SSH.$w!f7.>S>`: Opens port `22 (SSH)`.
  - Counterpart to `FTPSprint`, Solve speed is random, speeds up SSH break-in compared to vanilla.
  - Maximum 2s, Minimum 8s (same as SSHCrack)
- `#TELE_SMOOTH#` / `teleSMOOTH` : Opens port `23 (Telnet)`.
  - Upon initiation, 5 processes spawn with decreasing size and solve time.
  - When a finished smallest process is `kill`ed or ended, all siblings process time is halved.
  - When a process is `kill`ed out of order, all processes fail and you must start from beginning
  - Maximum 180s (no `kill` used), Minimum 16.5s (`kill` as soon as each process finishes)
- `#PACKET_HEADER_INJECTION#` / `:pkthdr::`: Opens port `80 (HTTP WebServer)`.
  - Takes more time, but vastly less RAM compared to vanilla. Useful for packing in remaining spaces.
- `#CLOCK_V3#`: ??? `123 (NTP)`
- `#SQL_TX_CRASHER#` / `THE_LAST_RESORT`: Opens port `1433 (SQL Server)`.
  - This program will **re-enable** firewall and proxy if the target has any.
  - This program stays for long after the process is completed, encouraging players to manually `kill` it.
- `#MQTT_INTERCEPTOR` / `mqttpwn:r63`: Opens port `1883 (MQTT Protocol)`. (NO GRAPHICS YET)
  - if MQTT port is present, you must connect to it before you can open SSH or FTP ports. (NOT IMPLEMENTED)
- `#SANDBOX_ESCAPE#`: ??? `2375 (Docker)`
- `#EOS_ROOTKIT#`: ??? `3659 (eOS)`
- `#GIT_TUNNEL#` / `GitTunnel For Hacknet BETA`: Opens port `9418 (GitTunnel)`.
  - deletes the `/log` folder upon successful hack. MAY LOSE INFORMATION!
- `#ANTI_ANTIVIRUS#`: ??? `16324 (Amos Updater)`
- `#PORT_BACKDOOR#` / `thanks from /el/ <3`: Opens port `0 (Backdoor Connection)`.
  - Port `0 (Backdoor Connection)` can be opened on **ANY OS**, essentially working as a free port.
  - This process takes a long, **long** time.
- `#VPN_BYPASS#`: ???
- `#NET_SPOOF#`: ???
- `#SOFT_SNOOZE#`: ???
- `#TRACE_FREEZER#`: ???

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

Once the `head` of the Network is shut off, `Track` timer will change to a gray `Retrace` timer, letting you close connections and forkbomb the rest of the OSes, delete the logs, etc.

Once the `Retrace` period is over and everything is good, it is considered that the player has won.

See [This Video](https://youtu.be/uQaObgut0p0) for a visual showcase of Trace V2 and other things this mod adds.

## File System
- Images and (IF [STUXNET](https://github.com/AutumnRivers/StuxnetHN/releases/latest) IS INSTALLED) Audio files can be stored in a file now.
  - To add an image located at `ExtensionName/Images/logo.png`: `<AddAsset FileName="logo.png" FileContents="#0DTK_IMAGE:Images/logo.png#" TargetComp="playerComp" TargetFolderpath="home" />`;
  - To add it as an IRC chat: `<AddIRCMessage Author="author" TargetComp="comp" Delay="0.0">!ATTACHMENT:image#%#logo.png#%#Images/logo.png</AddIRCMessage>`
  - Images will be visible upon click/`cat` and also in the IRC when it is posted.
  - You are also able to download the images from `MemForensics` results.
  - Clicking the "+" button on the image attachment downloads the image.
  - To add a song file located at `ExtensionName/Music/1.ogg` with a song ID `music1`: `<AddAsset FileName="1.ogg" FileContents="#STUXNET_RADIO:music1#" TargetComp="playerComp" TargetFolderpath="home" />`;
  - To add it as an IRC chat: `<AddIRCMessage Author="author" TargetComp="comp" Delay="0.0">!ATTACHMENT:file#%#1.ogg#%##STUXNET_RADIO:music1#</AddIRCMessage>`
  - Clicking the "+" button on the audio attachment automatically adds it to `RadioV3`.

## IRC Improvements
- You are able to talk in irc using the `send` command. it is disabled by default to avoid breaking immersion in non-0DTK extensions, but can be enabled by adding `<EnableCommand command="send">` on StartingAction.
- Conditions are available to detect the player messages and react accordingly.
  - `<OnIRCMessageAny [target="computer_id" user="string" notUser="string" minDelay="float" maxDelay="float" requiredFlags="string" doesNotHaveFlags="string"]>`: Triggered when a user sends a message to the IRC chat. set `user="#PLAYERNAME#` to have it trigger when a user sends something.
  - `<OnIRCMessage [target="computer_id" user="string" notUser="string" minDelay="float" maxDelay="float" requiredFlags="string" doesNotHaveFlags="string" word="string"]>`: Triggered when a specific word is said on IRC. Case and spacing insensitive.
    - You may use `&` and `|` to create AND / OR operation between words. for example, `what&this|that` would match `what is this` and `what is that` but not `this and that`.
  - `<OnIRCMessageTone [target="computer_id" user="string" notUser="string" minDelay="float" maxDelay="float" requiredFlags="string" doesNotHaveFlags="string" tone="string"]>`: Triggered when a user responds with a specific type of message.
    - Since 1.0, you are able to create custom tones using the Locales feature explained below.
    - Currently supported default tones are `no`, `yes`, `help`, `hey` and number `1` through `10`.
- You are able to also send attachments similar to the NPCs.
  - `send name@ip`: sends `LINK: name@ip`
  - `send id@pw@ip`: sends `ACCOUNT: ip : id@pw`
  - `send <path to local file>`: sends `FILE`, `IMAGE` or `AUDIO` depending on what it is. 
- Conditions are available for capturing attachments specifically as well.
  - `<OnIRCAttachment [target="computer_id" user="string" notUser="string" minDelay="float" maxDelay="float" requiredFlags="string" doesNotHaveFlags="string" name="string"]>`: Detects if the message sent is an attachment, and then optionally checks for its name (user ID for links and accounts).
  - `<OnIRCAttachmentLink [target="computer_id" user="string" notUser="string" minDelay="float" maxDelay="float" requiredFlags="string" doesNotHaveFlags="string" name="string" ip="string"]>`: Detects if the message sent is a link, and then optionally checks for name and IP.
  - `<OnIRCAttachmentAccount [target="computer_id" user="string" notUser="string" minDelay="float" maxDelay="float" requiredFlags="string" doesNotHaveFlags="string" name="string" ip="string" id="string" password="string"]>`: Detects if the message sent is a link, and then optionally checks for ip, id and password. `name` should be redundant for user input but account technically has 4 arguments and `name` checks for former and `id` latter.
  - `<OnIRCAttachmentFile [target="computer_id" user="string" notUser="string" minDelay="float" maxDelay="float" requiredFlags="string" doesNotHaveFlags="string" name="string" content="string"]>`: Detects if the message sent is a file, and then optionally checks for name and content. you can also put hash file data in the `content` to check for special files (such as images or executibles).

## New Extension Actions
- `<SetRAM ram="float">`: Sets the RAM amount of the player.
- `<RunCommand command="string">`: Runs a command on that OS.
- `<ResetIRCDelay target="computer_id">`: Resets the IRC Delay for that OS, useful for branching IRC logs. (see Conditions)
- `<SetNumberOfChoices choices="int">`: Sets the number of choices for interactive IRC logs. (see Conditions)
- `<DisableCommand command="string">` / `<EnableCommand command="string">`: Prohibits usage of certain commands, useful for blocking chat by disabling `/`. (see Conditions)
- `<EnableStrictLog targetComp="computer_id">` / `<DisableStrictLog targetComp="computer_id">`: Makes it so that this PC also detects (dis)connection logs. you MUST delete all logs and `ForkBomb` yourself out to be hidden if strict log is on.

## New Extension Conditions
- `<OnFileCreation [target="computer_id" path="path" name="string" content="string"]>` / `<OnFileDeletion [target="computer_id" path="path" name="string" content="string"]>`: Triggered when a specific file is created or deleted on an OS.
- `<OnCrash [requiredFlags="string" doesNotHaveFlags="string" targetComp="computer_id" targetNetwork="network_id"]>`: When that OS or Network's "Head" has crashed due to `ForkBomb` and such. Defaults to the current OS/Network if written within a `TraceV2` or `Computer` tag.
- `<OnReboot [requiredFlags="string" doesNotHaveFlags="string" targetComp="computer_id" targetNetwork="network_id"]>`: When that OS or Network's "Head" has rebooted after a crash.
- `<OnRebootCompleted [requiredFlags="string" doesNotHaveFlags="string" targetNetwork="network_id" RequireLogsOnSource="bool" RequireSourceIntact="bool"]`: When that Network has finished its Retrace timer. only fires `RequireLogsOnSource` if any of the Network's devices have logs on them, and only fires `RequireSourceIntact` if `sys/netcfgx.dll` is still on any of the devices.
- `<OnHostileActionTaken [requiredFlags="string" doesNotHaveFlags="string" targetComp="computer_id" targetNetwork="network_id"]>`: when a hostile action (hacking attempt, commonly tripping trace) is taken.

## Event System
Events are like flags but meant to operate on a simpler level.  
`<SendEvent signal="string">` sends an Event with the corresponding signal, and `<OnEvent signal="string">` recieves it. All events currently loaded recieve the signal and execute (no ordering nonsense) before the signal `dissipating away`, making room for repeat signal executions.

## Localization Support
You can set the extension's language to `dynamic` to enable multiple locales in a single extension.

Include `{{key}}` in place of the text to make it be influenced by locales.

create a `/Locales` folder in your extension, and put xml files in there.
here is an example of such a file.

setting the `exact` attribute to true makes it so that it does not partially replace letters.

```xml
<Locale>
  <en-us>
    <l key="back">Back</l>
    <l key="ok">OK</l>
    <l key="cancel" exact="true">Cancel</l>
  </en-us>
</Locale>
```

you may also put these files inside `.../Hacknet/locales/Custom` to apply the translation key for all extensions.

If downloaded via the installer, the mod also adds **complete font support** for Korean and unlocks use of IME for CJK inputs.

### Custom Tones Support
You can create custom tones for `<OnIRCMessageTone>` or edit existing tones using locale file as well. These are the vanilla tones xml file for reference.
```xml
<Locale>
    <en-us>
      <l key="0dtk::tone_no">no|na|eh|sorry|can't|cant|decline|no can|off|won't|wont|couldn't|couldnt|shouldn't|shouldnt|shant|shan't</l>
      <l key="0dtk::tone_yes">ye|yup|ok|alr|aight|lets|les|let's|leg|sure|wish me|got it|got this|cool|ready|rdy|can do|will|accept|bring|here i|here we|could|would|should|ought|shall</l>
      <l key="0dtk::tone_help">stuck|can't|cant|what|hm|huh|?|not|help|aid|idea|how|hint|clue|doesn|nudge</l>
      <l key="0dtk::tone_hey">guy|boy|girl|dude|folk|people|@channel|yall|y'all|ppl|everyone|someone|anyone|hey|sup</l>
      <l key="0dtk::tone_1">1|one|first|former</l>
      <l key="0dtk::tone_2">2|two|second</l>
      <l key="0dtk::tone_3">3|three|third</l>
      <l key="0dtk::tone_4">4|four|fourth</l>
      <l key="0dtk::tone_5">5|five|fifth</l>
      <l key="0dtk::tone_6">6|six|sixth</l>
      <l key="0dtk::tone_7">7|seven|seventh</l>
      <l key="0dtk::tone_8">8|eight|eighth</l>
      <l key="0dtk::tone_9">9|nine|ninth</l>
      <l key="0dtk::tone_10">10|ten|tenth</l>
  </en-us>
</Locale>
```

## Other, Miscellaneous Tweaks...
- **SFX Volume Slider**. adjustable in options.
- You can now use `Sequencer.exe -i` (also applies to Kaguya's Trial) to instantly start the challenge.
- You can now use `ComShell.exe -t` to trap all shells.
- A typo in usage message for `ComShell` is fixed.
- Tracking Logs are smarter. (does not count connect/disconnect logs UNLESS `<EnableStrictLog>` is enabled for that device.)

## Changelog
- 1.0.0:
  - File System (`0DTK_IMAGE`/`STUXNET_RADIO`)
  - new irc attachment types: file, image, radio
  - ability to send attachment to irc
  - new exe: `TeleSmoothTalker`
  - new commands: `alias`/`unalias`, `rmdir`, `cp`, `wc`, `who`/`whoami`, `w`, `last`, `>`/`send`/`irc`, `shutdown`, `ip addr(ess) show`/`add`, `netstat`, `ping`, `source`, `radio`
  - piping (`&&`,`||`,`|`,`>>`,`;`)
  - `SendEvent`/`OnEvent`, `OnIRCAttachment`, `OnIRCAttachmentLink`/`OnIRCAttachmentAccount`/`OnIRCAttachmentFile`
  - fixed `GitTunnel` visual on non-korean font
  - fixed 0DTK crash when it is installed both globally and extensionwise
  - `getExecutible()` now detects custom exes
  - fixed `SetRAM` value not saving
  - added `remline` and `reloadtheme` to help menu
  - fixed delete key causing issues in IME unlock
  - temporarily removed `MQTT_Interceptor` as it is not finished
  - updated C# version to latest
- 0.2.7: fixed onircmessage and onircmessagetone, custom locale on en-us
- 0.2.6: ability to type foreign letters on console (cjk IME support)
- 0.2.5: locale-based irc tone recognition and irc chat filter, korean irc tone recognition
- 0.2.4: parameters for any 0DTK action/conditions are valid for both all-lowercase and propercase form
- 0.2.3: added `pwd`, added `rdy` to `yes` tone, proper error messages on executables, fixed bug with disabling commands between save/load
- 0.2.2: Fixed command disables not being saved, added strict log functionality, added exact locale functionality, specified some things in README, fixed some korean locale things, changed `bindecode` and `binencode` to `binary [-d/-e]`, added `echo`, `date`, `expr`, `history`, `man`, `hostname`, `head` and `tail`, added `<EnableStrictLog>`/`<DisableStrictLog>`, `help` is properly sorted, autocomplete is properly disabled for disabled commands, added initial disabled command list 
- 0.2.1: Updated to PF 5.3.0, Fixed IRC condition bug, made dynamic locale read a lot faster (appearantly +=ing string a million times is not good for your OS), added custom command description, and change description based on disabled commands for `help` menu. Added proper README.
- 0.2.0: Added graphics for PortBackdoor, Dynamic locale support, Sound Effect Volume setting
- 0.0.2: Refactored half the mod
- 0.0.1: Initial release
