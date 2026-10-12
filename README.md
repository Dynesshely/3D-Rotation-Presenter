# 3D Rotation Presenter
 
 A little demo to draw a phone in different direction.

## Requirements

- .NET 10 SDK (Windows) — WPF, `net10.0-windows`

## Rotation cores

The rotation itself is pluggable: an `IRotationCore` turns the three input angles
(yaw about Z, pitch about X, roll about Y) into an `IRotation`, which is created once per
frame and then applied to every point of the phone.

| Implementation | Represents orientation as | Notes |
| --- | --- | --- |
| `EulerAngleRotationCore` | three angles, applied one axis at a time as `Ry(roll)·Rx(pitch)·Rz(yaw)` | the original algorithm, kept numerically identical |
| `QuaternionRotationCore` | a unit quaternion rotated by `q ⊗ v ⊗ q⁻¹` | same rotation, no three-angle degeneracy |

Both are mathematically identical. Pick one in the **Rotation Core** box, then press
**Verify Cores** to compare them numerically at the current angles.

The back face is detected from the transformed plane normal (`n = R·(0,0,1)`, front when
`n.Z > 0`) instead of comparing the individual angles, and **Camera Height** really does
move the camera at `(0, 0, z)`; a vertex that ends up behind the camera plane is dropped,
so setting the camera too close to the phone clips the far corners.

Preview:

![image](https://user-images.githubusercontent.com/50760269/215751965-7a9e4f2b-c997-4a20-873b-836111e63acc.png)
![image](https://user-images.githubusercontent.com/50760269/215752077-a3fca3c7-8c88-4445-a82e-6bc4f07cd0f7.png)
![image](https://user-images.githubusercontent.com/50760269/215752143-a7eeff7e-ffaf-4473-99ea-42caef025f61.png)


