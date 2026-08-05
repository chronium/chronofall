# ImGui.Net patch set

The coordinator pins ImGui.Net independently and applies this ordered patch set
after fetching the ignored source checkout.

`0001-limit-bindings-and-runtime-surface.patch` keeps the generated Dear ImGui
and ImGuizmo bindings, removes the unused ImPlot and imnodes managed surfaces,
removes a redundant legacy `Unsafe` package reference, and prevents upstream
prebuilt `cimgui` binaries from competing with the coordinator-built SDL GPU
backend. The required `Evergine.Mathematics` ABI vector dependency remains at
the exact version declared by the pinned upstream project.
