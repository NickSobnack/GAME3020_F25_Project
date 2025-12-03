using UnityEngine;

public interface IDeflect
{
    public void Deflect(Vector2 direction);

    public float ReturnSpeed { get; set; }
}
