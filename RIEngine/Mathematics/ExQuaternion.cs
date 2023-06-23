using OpenTK.Mathematics;

namespace RIEngine.Mathematics;

public static class ExQuaternion
{
    public static Quaternion FromToRotation(Vector3 from, Vector3 to)
    {
        Vector3 axis = Vector3.Cross(from, to);
        float angle = Vector3.CalculateAngle(from, to);
        return Quaternion.FromAxisAngle(axis, angle);
    }

    public static Quaternion LookRotation(Vector3 forward, Vector3 up)
    {
        var lookDirection = forward.Normalized();
        var upDirection = (up - forward * Vector3.Dot(up, forward)).Normalized();
        var rightDirection = Vector3.Cross(upDirection, lookDirection);
        
        return Quaternion.FromMatrix(new Matrix3(rightDirection, upDirection, lookDirection));
    }
}