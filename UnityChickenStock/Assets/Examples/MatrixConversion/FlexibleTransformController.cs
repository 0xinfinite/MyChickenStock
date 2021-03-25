using UnityEngine;

public class FlexibleTransformController : MonoBehaviour
{
    public FlexibleTransformParent parent;

    // Start is called before the first frame update
    void Start()
    {
        parent.InputTransform();
    }

    // Update is called once per frame
    void Update()
    {
       // if (Input.GetMouseButtonDown(1))
       // {
            parent.SyncTransform();
      //  }
    }
}

[System.Serializable]
public class FlexibleTransformParent
{
    public Transform parent;
    public FlexibleTransform[] child;

    public FlexibleTransform GetChild(int index)
    {
        return child[index];
    }

    public void InputTransform()
    {
        Matrix4x4 parentMatrix = Matrix4x4.TRS(parent.position, parent.rotation, parent.lossyScale);
        int childCount = child.Length;
        for (int i = 0; i < childCount; ++i)
        {
            FlexibleTransform childFlexible = GetChild(i);
            Transform childTransform = childFlexible.transform;

            Matrix4x4 m = MatrixConversion.GetLocalMatrix(parentMatrix, childTransform.position, childTransform.rotation, childTransform.lossyScale);

            childFlexible.position = MatrixConversion.PositionFromMatrix(m);
            childFlexible.rotation = MatrixConversion.RotationFromMatrix(m);
            childFlexible.scale = MatrixConversion.ScaleFromMatrix(m);
        }
    }

    public void SyncTransform()
    {
        int childCount = child.Length;
        Matrix4x4 parentMatrix = Matrix4x4.TRS(parent.position, parent.rotation, parent.lossyScale);
        for (int i = 0; i < childCount; ++i)
        {
            FlexibleTransform childFlexible = GetChild(i);
            Transform childTransform = childFlexible.transform;

            Matrix4x4 m = MatrixConversion.GetWorldMatrix(parentMatrix, childFlexible.position, childFlexible.rotation, childFlexible.scale);

            childTransform.position = MatrixConversion.PositionFromMatrix(m);
            childTransform.rotation = MatrixConversion.RotationFromMatrix(m);
            Vector3 scale = MatrixConversion.ScaleFromMatrix(m);
            childTransform.localScale = childTransform.parent==null ?scale:new Vector3(scale.x* childTransform.localScale.x,scale.y* childTransform.localScale.y,scale.z* childTransform.localScale.z);

        }
    }
}

[System.Serializable]
public class FlexibleTransform
{
    public Transform transform;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}

public static class MatrixConversion
{
    public static Vector3 PositionFromMatrix(Matrix4x4 m) { return m.GetColumn(3); }

    // Extract new local rotation
    public static Quaternion RotationFromMatrix(Matrix4x4 m)
    {
        return Quaternion.LookRotation(
            m.GetColumn(2),
            m.GetColumn(1)
            );
    }

    // Extract new local scale
    public static Vector3 ScaleFromMatrix(Matrix4x4 m)
    {
        return new Vector3(
            m.GetColumn(0).magnitude,
            m.GetColumn(1).magnitude,
            m.GetColumn(2).magnitude);
    }

    public static Matrix4x4 GetLocalMatrix(Matrix4x4 parentMatrix, Vector3 worldPosition, Quaternion worldRotation, Vector3 lossyScale)
    {
        Matrix4x4 childMatrix = Matrix4x4.TRS(worldPosition, worldRotation, lossyScale);
        return parentMatrix.inverse * childMatrix;
    }

    public static Matrix4x4 GetWorldMatrix(Matrix4x4 parentMatrix, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
    {
        Matrix4x4 childMatrix = Matrix4x4.TRS(localPosition, localRotation, localScale);
        return parentMatrix * childMatrix;
    }
}