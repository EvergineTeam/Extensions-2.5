XBUILD=/Applications/Xcode.app/Contents/Developer/usr/bin/xcodebuild
PROJECT_ROOT=.
PROJECT=$(PROJECT_ROOT)/VuforiaAdapter.xcodeproj
TARGET=VuforiaAdapter

all: libVuforiaAdapter.a

libVuforiaAdapter-armv7.a:
	$(XBUILD) -project $(PROJECT) -target $(TARGET) -sdk iphoneos -arch armv7 -configuration Release clean build
	-mv $(PROJECT_ROOT)/build/Release-iphoneos/lib$(TARGET).a $@

libVuforiaAdapter.a: libVuforiaAdapter-armv7.a
	lipo -create -output $@ $^

clean:
	-rm -f *.a *.dll