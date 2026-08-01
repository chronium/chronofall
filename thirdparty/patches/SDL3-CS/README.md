# SDL3-CS Coordinator Patch

`0001-disable-android-target-for-coordinator.patch` sets SDL3-CS's existing `CI_DONT_TARGET_ANDROID` switch in the ignored coordinator checkout. The bind-pose experiment consumes only the desktop `net8.0` binding and must not require Android or WebAssembly workloads during ordinary coordinator restore and build.

The patch changes build selection only. It does not modify generated bindings, native binaries, or SDL behavior.
