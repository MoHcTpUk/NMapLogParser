# NMapLogParser
## Features

* Parsing NMap log files to a many different files by IP machine

## Options
* `-f, --file`       Required. Set file to parse
* `-r, --rewrite`    Rewrite exists reports

## Examples

Parsing `scanlog010521.txt`:
```
NMapLogParser.exe -f "C:\Users\User\Desktop\scanlogs\scanlog010521\scanlog010521.txt"
```

Parsing `scanlog010521.txt` and rewrite exists reports:
```
NMapLogParser.exe -f "C:\Users\User\Desktop\scanlogs\scanlog010521\scanlog010521.txt" -r
```
