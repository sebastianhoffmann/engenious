using System;
using engenious;

namespace FBXImporter
{
    public class FbxKeyFrame
    {
        public FbxKeyFrame(FBXImporter.FBXLibrary.AnimCurve curve)
        {
            Translation = curve.translation;
            Rotation = new Quaternion(curve.rotation.X,curve.rotation.Y,curve.rotation.Z,curve.rotation.W);
            Scaling = curve.scaling;
            Time = curve.time;
        }
        public Vector3 Translation{get;private set;}
        public Quaternion Rotation{get;private set;}
        public Vector3 Scaling{get;private set;}
        public float Time{get;private set;}
    }
}

