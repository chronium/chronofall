# SimpleMesh Patches

## 0001-expose-animation-scale-and-interpolation.patch

Adds the minimum imported-animation metadata needed by the M1 experiment: public interpolation values, scale keyframes/channels, and interpolation retention on translation, rotation, and scale channels. ChronoFall's adapter remains responsible for rejecting anything other than LINEAR and for sampling animation data.

## 0002-use-invariant-culture-for-obj-floats.patch

Makes multi-value OBJ/MTL floating-point parsing culture-invariant. Without this narrow fix, a machine using a decimal-comma locale can interpret values such as `-1.25` as `-125`, making identical source files cook into different geometry.
