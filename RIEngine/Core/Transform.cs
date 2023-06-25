using System.Collections;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using RIEngine.Mathematics;

namespace RIEngine.Core;

public class Transform : Component
{
    private Transform? _parent;

    public Transform? Parent
    {
        get => _parent;
        set
        {
            _parent = value;
            if (_parent?.Root != null)
            {
                Root = _parent.Root;

                Scale = Scale;
                Rotation = Rotation;
                Position = Position;
            }
        }
    }

    [JsonIgnore] private bool _isChanged;
    #region Position

    private Vector3 _position = Vector3.Zero;
    private Vector3 _localPosition = Vector3.Zero;

    [JsonIgnore] public Transform Root { get; private set; }

    /// <summary>
    /// World position.
    /// Setting this property will also set the local position.
    /// </summary>
    public Vector3 Position
    {
        get => _position;
        set
        {
            _isChanged = true;
            _position = value;
            _localPosition =
                Parent?.TranslatePoint(value) ?? value;
        }
    }

    /// <summary>
    /// Local position.
    /// Setting this property will also set the world position.
    /// </summary>
    public Vector3 LocalPosition
    {
        get => _localPosition;
        set
        {
            _isChanged= true;
            _localPosition = value;
            _position = Parent?.ReversePoint(value) ?? value;
        }
    }

    #endregion

    #region Rotation

    private Quaternion _rotation = Quaternion.Identity;
    private Quaternion _localRotation = Quaternion.Identity;

    public Quaternion Rotation
    {
        get => _rotation;
        set
        {
            _isChanged = true;
            _rotation = value.Normalized();
            if (Parent != null)
                _localRotation = Parent.Rotation.Inverted() * _rotation;
            else
                _localRotation = _rotation;
        }
    }

    public Quaternion LocalRotation
    {
        get => _localRotation;
        set
        {
            _isChanged = true;
            _localRotation = value.Normalized();
            if (Parent != null)
                _rotation = _localRotation * Parent.Rotation;
            else
                _rotation = _localRotation;
        }
    }

    #endregion

    #region Scale

    private Vector3 _scale = Vector3.One;
    private Vector3 _localScale = Vector3.One;

    public Vector3 Scale
    {
        get => _scale;
        set
        {
            _isChanged = true;
            _scale = value;
            if (Parent != null)
                _localScale = new Vector3(_scale.X / Parent.Scale.X, _scale.Y / Parent.Scale.Y,
                    _scale.Z / Parent.Scale.Z);
            else
                _localScale = _scale;
        }
    }

    public Vector3 LocalScale
    {
        get => _localScale;
        set
        {
            _isChanged = true;
            _localScale = value;
            if (Parent != null)
                _scale = new Vector3(_localScale.X * Parent.Scale.X, _localScale.Y * Parent.Scale.Y,
                    _localScale.Z * Parent.Scale.Z);
            else
                _scale = _localScale;
        }
    }

    #endregion

    #region Axis

    [JsonIgnore]
    public Vector3 Up
    {
        get => Rotation * Vector3.UnitY;
        set => Rotation = ExQuaternion.FromToRotation(Up, value);
    }

    [JsonIgnore]
    public Vector3 Right
    {
        get => Rotation * Vector3.UnitX;
        set => Rotation = ExQuaternion.FromToRotation(Right, value);
    }

    [JsonIgnore]
    public Vector3 Forward
    {
        get => Rotation * Vector3.UnitZ;
        set => Rotation = ExQuaternion.LookRotation(value, Vector3.UnitY);
    }

    #endregion

    #region Space Transform Matrix

    [JsonIgnore] public Matrix4 LocalToWorldMatrix;

    [JsonIgnore] public Matrix4 WorldToLocalMatrix => LocalToWorldMatrix.Inverted();


    private Vector3 WorldToLocalTranslate(Vector4 vec)
    {
        return (WorldToLocalMatrix * vec).Xyz;
    }

    private Vector3 LocalToWorldTranslate(Vector4 vec)
    {
        return (LocalToWorldMatrix * vec).Xyz;
    }

    public Vector3 TranslatePoint(Vector3 point)
    {
        return WorldToLocalTranslate(new Vector4(point, 1));
    }

    public Vector3 TranslateVector(Vector3 vector)
    {
        return WorldToLocalTranslate(new Vector4(vector, 0));
    }

    public Vector3 ReversePoint(Vector3 point)
    {
        return LocalToWorldTranslate(new Vector4(point, 1));
    }

    public Vector3 ReverseVector(Vector3 vector)
    {
        return LocalToWorldTranslate(new Vector4(vector, 0));
    }

    #endregion


    #region Constructor

    public Transform(RIObject riObject, Guid guid) : base(riObject, guid)
    {
        LocalToWorldMatrix = Matrix4.CreateTranslation(Position)  *
                             (Matrix4.CreateFromQuaternion(Rotation)* Matrix4.CreateScale(Scale));
    }

    public Transform(RIObject riObject) : base(riObject)
    {
        LocalToWorldMatrix = Matrix4.CreateTranslation(Position)  *
                          (Matrix4.CreateFromQuaternion(Rotation)* Matrix4.CreateScale(Scale));
    }

    #endregion

    #region Callbacks

    public void UpdateTransform()
    {
        if (!_isChanged) return;

        LocalToWorldMatrix = Matrix4.CreateScale(Scale) *
                             (Matrix4.CreateFromQuaternion(Rotation) * Matrix4.CreateTranslation(Position));
        Position = Position;
        Rotation = Rotation;
        Scale = Scale;
        _isChanged = false;

        foreach (var child in RIObject.Children)
        {
            child.Transform._isChanged = true;
        }
    }

    #endregion
}