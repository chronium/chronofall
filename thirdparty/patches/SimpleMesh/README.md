# SimpleMesh Patches

## 0001-expose-animation-scale-and-interpolation.patch

Adds the minimum imported-animation metadata needed by the M1 experiment: public interpolation values, scale keyframes/channels, and interpolation retention on translation, rotation, and scale channels. ChronoFall's adapter remains responsible for rejecting anything other than LINEAR and for sampling animation data.
