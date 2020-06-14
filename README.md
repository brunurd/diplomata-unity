### :warning::warning::warning: **This project is no more maintained and is archived.** :warning::warning::warning:
#### **_I strongly recommend as substitute the [Fungus][fungus-link] (now with lua as script language can be very extensible)._**

---

# Diplomata

[![openupm](https://img.shields.io/npm/v/com.lavaleakgames.diplomata?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.lavaleakgames.diplomata/)

<img align="right" src="https://raw.githubusercontent.com/lavaleak/diplomata-unity/master/Logo/DIPLOMATA-Logo_CC-BY-ND_by-Bruno-Araujo.png" alt="Diplomata Logo" title="Diplomata Logo by Bruno Araujo is licensed under a Creative Commons Attribution-NoDerivatives 4.0 International License." /><br/>

Diplomata is a Unity multi language dialogues content management system and editor extension inspired by [Twine](http://twinery.org/), like [Fungus](http://fungusgames.com/) and [Yarn / Yarn Spinner](https://github.com/InfiniteAmmoInc/Yarn), but **is not node based**.  

Diplomata manage optionally other contents of a game like characters, inventory, quests, animations and sprites.

Idealized for screenwriters, game designers, programmers and hobbyist writers, to configure and apply dialogues in any type of game.



---


![Screenshot](Screenshot.jpg)


**Features:**
- Text edition in an organized environment split by characters and contexts.
- Manage characters, contexts, and messages separately from the scene objects.
- Add multiple languages for your game.
- Fuzzy logic system to influence organically the characters with custom attributes.
- Use colors in the message cards to organize your messages in your own way.
- All text and preferences are saved in json files, making it easy to use external tools.
- Export text to screenplay format for actors understand the story.
- Use templates or create your own. (In development)
- Internal system to save and load game progress, you can use as is or side by side with your own persistence system.
- Choose and manage audio files to play when message show, taking into account the configured language.
- Manage animator attributes in any message.
- Choose and manage sprites to show in every message.
- Inventory integration with title and description in multiple languages.
- Create custom flags to authoring your game progress.

**Installation:**

- Install via OpenUPM

  The package is available on the [openupm registry](https://openupm.com). It's recommended to install it via [openupm-cli](https://github.com/openupm/openupm-cli).

  ```
  openupm add com.lavaleakgames.diplomata
  ```
- Install via Git URL

  Open *Packages/manifest.json* with your favorite text editor. Add the following line to the dependencies block.

      "dependencies": {
        "com.lavaleakgames.diplomata": "https://github.com/brunurd/diplomata-unity.git"
      }

---


**License:**


All files in this repository is licensed under a [MIT License](https://github.com/lavaleak/diplomata/blob/master/LICENSE.md).

[fungus-link]: https://fungusgames.com/
