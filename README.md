To work on this, you will need a version of Mono patched with this:

https://github.com/mono/mono/pull/7100

Then, build this, and execute your sample Winforms app in 64 bit mode.

If you checked out this under /Users/miguel/Projects/CocoaDriver,
build this and then use this command:

```
export DRIVER=/Users/miguel/Projects/CocoaDriver
MONO_MWF_DRIVER=$DRIVER/CocoaDriver/bin/Debug/CocoaDriver.dll MONO_PATH=$DRIVER/CocoaHost/bin/Debug/CocoaHost.app/Contents/MonoBundle/ DYLD_LIBRARY_PATH=/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/ mono notepad.exe
```

My own version looks like this:

```
MONO_MWF_DRIVER=/Users/miguel/Dropbox/Projects/CocoaDriver/CocoaDriver/bin/Debug/CocoaDriver.dll MONO_PATH=/Users/miguel/Dropbox/Projects/CocoaDriver/CocoaHost/bin/Debug/CocoaHost.app/Contents/MonoBundle/ DYLD_LIBRARY_PATH=/Library/Frameworks/Xamarin.Mac.framework/Versions/Current//lib/ mono notepad.exe
```