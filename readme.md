DS4_PSO2
===

This is a utility that maps a DualShock 4 controller (plugged in via USB) to a virtual joystick (provided by the vJoy driver + library) designed to interact properly with Phantasy Star Online 2 (it has very shoddy support for joysticks/gamepads, so we have to give it a little help).

This repository also contains a reusable library for interacting with DualShock 4 controllers (LibDualShock4) suitable for use in games and multimedia software - perhaps as an alternative to DirectInput or XInput backends.

Requirements
===

Joystick emulation is done using the [vJoy driver](http://vjoystick.sourceforge.net), so you'll need to install that and enable at least one joystick. The vJoy joystick can be configured by pressing the 'Configure' button in the DS4_PSO2 main window.

Download
===

See the [Downloads](https://github.com/kg/DS4_PSO2/wiki/Downloads) page.

Features
===

* Maps all PS4 controller buttons
* Maps directional swipes on the touch panel to virtual button presses
* Maps analog sticks, triggers and dpad
* Maps analog sticks to virtual buttons for games without analog trigger support
* Can automatically apply an appropriate vJoy joystick configuration

ToDo
===

* Bluetooth support is not implemented (though, to be fair, the PS4 controller's bluetooth works terribly with PCs anyway, and the battery life is poor, so it's not as big of a missing feature as it is with 360 controllers)
* No configuration UI is available for button/axis mappings or for the touch panel swipes (though it is relatively straightforward to change the code)
* Only one DualShock 4 controller is supported. It might be nice to support multiple for use in multiplayer games.
* While the LibDualShock4 code properly exposes the accelerometers and gyroscopes built into the controller, they are not mapped to Windows axes. Adding this might be useful for games that support 3D input.
