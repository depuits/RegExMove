# RegExMove
Moving files matching the given regex into separate folders using one of the matched regex groups. The default regex option lets you sort a music folder into sub directories per artist if the files are named 'artist - song.mp3'.

## Usage

RegExMove [OPTIONS] + regex

If no regex is specified, `^(.+?) - (.+)\.(mp3|wav)$` is used.

```
Options:
  -s, --selector=VALUE       the regex group index to use for the folder name.
                               index 0 is the complete match.
                               (default=1).
                               this must be an integer.
  -v, --verbose              increase debug message verbosity
  -h, --help                 show this message and exit
```
