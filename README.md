# ModernCamera
Makes the camera more like an action MMO camera

<details>
<summary>Examples</summary>
<ul>
<li><img src="https://i.imgur.com/bQVtdqg.jpg" alt="example 1"></img></li>
<li><a href="https://www.youtube.com/embed/tMhuAOtTez0">YouTubeVideo</a></li>
</ul>
</details>

### Features
- Option and keybinding to enable/disable ModernCamera
- Option to use default build mode camera settings
- Option to always show a crosshair
- Option for field of view
- Keybinding to hide UI
- First person view
    - Option to enable/disable first person view
    - Alway locks camera rotation
    - Offsets camera height when shapeshifted
- Third person view
    - Keybinding for action mode to lock camera rotation
    - Option for crosshair in action mode
    - Option to lock pitch angle
    - Option to lock zoom distance
    - Options to adjust min/max pitch angles
    - Options to adjust min/max zoom
    - Option for over the shoulder offsets
    - Option for aiming offsets
    - Option to lock aim mode forward


### Installation
- Install [BepInEx](https://v-rising.thunderstore.io/package/BepInEx/BepInExPack_V_Rising/)
- Extract _Silkworm.dll_ into _(VRising folder)/BepInEx/plugins_
- Extract _ModernCamera.dll_ into _(VRising folder)/BepInEx/plugins_


### Configuration
All configuration is done with the in game options menu


### Known Issues
- Can see through floors/roofs from below (this cannot be fixed because objects are missing Mesh faces)
- Shadows flicker when looking directly horizontal (Due to fake cloud shadows)


### FAQ
**Q: Why is my characters name and healthbar always shown at top of screen?**

**A:** Because the mouse is locked over the character. Use an over-the-shoulder offset in options to move the mouse off-center so that the mouse is not over the character.

**Q: Why can I see objects popping (loading) in and out in the distance now?**

**A:** This is done for performance by the game. Normally you don't see this because you are looking down at your character. ModernCamera cannot change this.

**Q: Why can't I rotate the camera after hiding the UI?**

**A:** You may have pressed "Enter" which causes the game to lock the camera because the chatbox should normally be open. To fix this, toggle the UI back on, click in the chat box, and press "Enter" again.


### Support
Join the [modding discord](https://vrisingmods.com/discord) for support and tag `@iZastic#0365`

Submit a ticket on [GitHub](https://github.com/v-rising/ModernCamera/issues)


### Contributors
- iZastic: `@iZastic#0365` on Discord
- Dimentox: `@Dimentox#1154` on Discord
- Kunogi: `@牧瀬紅莉栖#1570` on Discord


### Silkworm
Source code for the Silkworm library [https://github.com/iZastic/vrising-silkworm](https://github.com/iZastic/vrising-silkworm).

I am not officially supporting this as a library for use in other mods, but I wanted to add a link to the source for those who are interested.


### Changelog
`1.5.1`
- Removed InvertY option (this is now supported by the game under Camera Settings)
- Removed ThirdPersonRoof option (no longer affective)
- Added Field of View option
- Added keybind to hide all UI elements
- Fixed crash after leaving and joining a server
- Fixed issue where mouse stayed locked after disabling ModernCamera

<details>

`1.5.0`
- Updated for Gloomrot

`1.4.1`
- Fixed no fading of wrong UI elements (like chat)
- Fixed bug causing game to crash when leaving game

`1.4.0`
- Added option and keybind for enabling/disabling ModernCamera
- Added options for third person aiming offsets
- Added option to always show cursor
- Added first person offsets when shapeshifted
- Added shapeshifted and mounted detection for offsetting camera
- Added public method to enabled/disable ModernCamera (devs, ModernCamera.Enable(bool))
- Added public method to enabled/disable ActionMode (devs, ModernCamera.ActionMode(bool))
- Disabled ZoomModifierSystem
    - Fix crashing when MaxZoom is to low
    - Fix interference with ModernCamera zooming
- Fixed bug when zooming in/out of first person mode
- Fixed zoom bug after mounting a horse
- Fix world space UI disappearing when zoomed in

`1.3.1`
- Fixed conflict with Wetstone
- Fixed bug when trying to leave first person
- Fixed bug with setting min/max pitch
- Added options for over the shoulder offsets
- Added option for crosshair in action mode
- Added option to lock zoom

`1.3.0`
- Added all config options to the in game options menu
- Added keybinding for action mode
- Added option for locking y axis angle
- Added options for adjusting min/max y axis angles
- Added option to use default build mode zoom/pitch
- Removed camera rotation toggle (use action mode now)
- Fixed mouse moving around when rotation is locked

`1.2.0`
- Fixed mouse lock issue cause by Unity bug since version 2019
- Fixed mouse staying locked when leaving game to main menu
- Fixed mouse staying locked in options menu
- Stopped using Unity cursor locking and now manually sets mouse position when locked
    - This may fixed compatability issues with the NoGameCursor mod
- Added a config option for inverting the camera Y axis
- Added a config option for camera max zoom
- Added a config option for showing roof in third person
- Added a config option for an over the shoulder view

`1.1.0`
- Added first person support
- Added option to allow toggle or held mode for camera rotation
- Join button is now disabled for official servers instead of just doing nothing

`1.0.1`
- Fixed zoom, now you can go completely into 1st person also.

`1.0.0`
- Initial mod upload

</details>
