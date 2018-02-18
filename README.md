To work on this, you will need a version of Mono patched with this:

https://github.com/mono/mono/pull/7100

Then, build this, and execute your sample Winforms app in 64 bit mode.

If you checked out this under /Users/miguel/Projects/CocoaDriver:

```
export DRIVER=/Users/miguel/Projects/CocoaDriver
MONO_MWF_DRIVER=$DRIVER/CocoaDriver/bin/Debug/CocoaDriver.dll mono notepad.exe
```