# HolidayHat

[![Thunderstore Version](https://img.shields.io/thunderstore/v/OracleTeam/HolidayHat?style=for-the-badge)](https://thunderstore.io/c/peak/p/OracleTeam/HolidayHat/)
[![Thunderstore Version](https://img.shields.io/thunderstore/dt/OracleTeam/HolidayHat?style=for-the-badge)](https://thunderstore.io/c/peak/p/OracleTeam/HolidayHat/)
[![Thunderstore Likes](https://img.shields.io/thunderstore/likes/OracleTeam/HolidayHat?style=for-the-badge)](https://thunderstore.io/c/peak/p/OracleTeam/HolidayHat/)
[![GitHub Licences](https://img.shields.io/github/license/MiiMii1205/HolidayHat?style=for-the-badge)](https://github.com/MiiMii1205/HolidayHat)
[![GitHub Issues](https://img.shields.io/github/issues-raw/MiiMii1205/HolidayHat?style=for-the-badge)](https://github.com/MiiMii1205/HolidayHat/issues/new)

A dynamic hat that changes based on holidays.

_Compatible with [Mimic](https://thunderstore.io/c/peak/p/EMOKaMi/Mimic/), [SkinColorSliders](https://thunderstore.io/c/peak/p/Snosz/SkinColorSliders/) and [More Customizations
](https://thunderstore.io/c/peak/p/cretapark/More_Customizations/)_

## How it works ##

This mod adds a hat option in the passport called the "Holiday Hat". It shows up as a firework emoji.

This option activates different hats based on a secular holiday calendar.

For example, if the mod detects that today is Christmas (December 25th), then with the Holiday Hat active, your scout will wear a Santa hat.

To calculate the current holiday, the mod uses your own *boot time*.

So if you booted the game on December 25th (Christmas), then you'll be wearing the Santa hat.
It also locks you to that date until you reboot the game, and won't change during a run.

The mod supports several holidays, along with a few unique custom hats that appear on specific holidays.
All custom hats are also available as separate hat options in the passport should you want to wear them normally.

## Configure ##

The mod comes with configuration options that allow you to configure how the "Holiday Hat" option works:

- `Default Hat Index`: Holds the index of which hat to wear on normal days. Set it to the index of your favorite hat in the passport. (first hat starts at `0`). Can also be a modded hat, as long as it's at the same spot in the passport for everyone.

  If, for whatever reason, it points to the "Holiday Hat" option, it defaults to the scout cap. If the hat doesn't exist anymore (with a disabled hat mod for example) it will also default to that same scout hat.

To change this setting, you'll need to run the mod first for it to generate the file, then use your mod manager to edit it.
You can also use a mod like [ModConfig](https://thunderstore.io/c/peak/p/PEAKModding/ModConfig/) to change it in-game. 
_If you change this setting in-game, your hat will change for everyone at the same time, even in the middle of a run._

## Feedback ##

If you have any suggestions (like new hats ideas or comments about current hats or whatnot), feel free to post it [here](https://github.com/MiiMii1205/HolidayHat/issues/new), and we'll take a look.

The mod was also tested with a variety of different mod hats, but if there's a bug with your favorite cosmetic mod, please let us know [here](https://github.com/MiiMii1205/HolidayHat/issues/new) with a Thunderstore link to the cosmetic mod in question.
