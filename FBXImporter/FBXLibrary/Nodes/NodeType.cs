using System;

namespace FBXImporter
{
    public enum NodeType {
        eRootNode=-1,
        eUnknown=0, eNull, eMarker, eSkeleton,
        eMesh, eNurbs, ePatch, eCamera,
        eCameraStereo, eCameraSwitcher, eLight, eOpticalReference,
        eOpticalMarker, eNurbsCurve, eTrimNurbsSurface, eBoundary,
        eNurbsSurface, eShape, eLODGroup, eSubDiv,
        eCachedEffect, eLine
    }
}

