# ROM Hasher

File-at-a-time auditing tool for ROM images. Validates and/or calculates hashes for ROM images. The primary intended use of ROM Hasher is for validating files before publishing patches or before applying them.

[Download the binary at RHDN](https://www.romhacking.net/utilities/1002/).

## Usage

Drag and drop a file into the window or use the toolbar to open a file. You may be prompted to select a console if the file type can not be determined by the extension nor the file contents. You can optionally also hash a second *modified* ROM.

For RHDN submissions, the text displayed on the right can be copied for ROM/ISO information. This includes the corresponding No-Intro database match, if found. (Additional databases may be installed, e.g. TOSEC).

Additional detailed information is also shown below.

## Databases

By default, ROM Hasher includes a copy of the No-Intro database. For more up-to-date or alternative databases, you may want to visit [No-Intro's DAT-o-Matic](http://datomatic.no-intro.org/) or [TOSEC](https://www.tosecdev.org/). Make sure you're obtaining Clr-Mame-Pro-formatted databases, and add them using the `Database Config` dialog found in the settings menu to add them.
