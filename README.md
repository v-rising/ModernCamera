# ModernCamera
Makes the camera more like a mmo camera and removes the limits.

<details>
<summary>Examples</summary>
<ul>
<li><img src="https://i.imgur.com/bQVtdqg.jpg" alt="example 1"></img></li>
<li><a href="https://www.youtube.com/embed/tMhuAOtTez0">YouTubeVideo</a></li>
</ul>
</details>

### Features
- First person camera view
- Third person camera view
- Camera rotation locking
    - Locked by default in first person
    - Toggleable in third person
- Invert Y axis

### Installation
- Install [BepInEx](https://v-rising.thunderstore.io/package/BepInEx/BepInExPack_V_Rising/)
- Extract _ModernCamera.dll_ into _(VRising folder)/BepInEx/plugins_

### Known Issues
- Zooming in to far causes some world space UI elements to become fully transparent
- Target info bar (top center) is always visible for player when locked in third person view
- After V Blood feeding it will always place you in third person view
- Exiting build mode will always place you in third person view

### Support
Join the [modding community](https://dev.il.gy) for support and tag `@iZastic#0365` or `@Dimentox#1154`

Submit a tickat on [GitHub](https://github.com/v-rising/ModernCamera/issues)

###  Contributors
- Kunogi: `@牧瀬紅莉栖#1570` on Discord
- Dimentox: `@Dimentox#1154` on Discord
- iZastic: `@iZastic#0365` on Discord

### Changelog
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

<details>

`1.1.0`
- Added first person support
- Added option to allow toggle or held mode for camera rotation
- Join button is now disabled for official servers instead of just doing nothing

`1.0.1`
- Fixed zoom, now you can go completely into 1st person also.

`1.0.0`
- Initial mod upload

</details>
