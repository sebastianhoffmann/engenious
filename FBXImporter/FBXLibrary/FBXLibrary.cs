using System;
using System.Runtime.InteropServices;
using engenious.Graphics;
using engenious;

namespace FBXImporter
{
    public class FBXLibrary : NativeLibrary
    {
        
        public FBXLibrary()
            :base("CBridge")
        {
            
        }
        public delegate IntPtr create_fbxcontextDel();
        public delegate void delete_fbxcontextDel(IntPtr manager);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl,CharSet=CharSet.Ansi)]
        public delegate IntPtr getLogDel();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl,CharSet=CharSet.Ansi)]
        public delegate IntPtr load_modelDel(IntPtr manager, string filename,string password = null);

        public delegate IntPtr get_nameDel(IntPtr obj) ;

        public delegate IntPtr get_root_nodeDel(IntPtr scene);

        public delegate int get_countDel(IntPtr obj);
        public delegate Vector3 get_geometric_transformationDel(IntPtr node);
        public delegate Matrix get_node_matrixDel(IntPtr node);
        public delegate IntPtr get_ElementDel(IntPtr obj, int index);
        public delegate int get_attribute_typeDel(IntPtr node);
        public delegate IntPtr get_typedNodeDel(IntPtr node);

        public delegate int get_vertexcountDel(IntPtr mesh);
        public delegate bool getVerticesDel(IntPtr mesh, engenious.Graphics.VertexPositionNormalTexture[] vertices);

        public delegate void set_transformDel(Matrix m);

        public delegate int get_anim_curve_keyCountDel(IntPtr node, IntPtr animLayer);

        [StructLayout(LayoutKind.Sequential,Pack=1)]
        public struct AnimCurve{
            public Vector3 translation;
            public Vector4 rotation;
            public Vector3 scaling;
            public float time;
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl,CharSet=CharSet.Ansi)]
        public delegate AnimCurve get_anim_curveDel(IntPtr node, IntPtr animLayer, int index);

        [NativeMethodAttribute()]
        public static create_fbxcontextDel create_fbxcontext;
        [NativeMethodAttribute()]
        public static delete_fbxcontextDel delete_fbxcontext;
        [NativeMethodAttribute("getLog")]
        private static getLogDel getLogInternal;
        [NativeMethodAttribute()]
        public static load_modelDel load_model;
        [NativeMethodAttribute("get_name")]
        public static get_nameDel get_name_internal;
        [NativeMethodAttribute()]
        public static get_root_nodeDel get_root_node;
        [NativeMethodAttribute()]
        public static get_countDel get_child_count;
        [NativeMethodAttribute()]
        public static get_node_matrixDel get_node_transform;
        [NativeMethodAttribute()]
        public static get_geometric_transformationDel get_geometric_translation;
        [NativeMethodAttribute()]
        public static get_geometric_transformationDel get_geometric_rotation;
        [NativeMethodAttribute()]
        public static get_geometric_transformationDel get_geometric_scaling;
        [NativeMethodAttribute()]
        public static get_ElementDel get_child;
        [NativeMethodAttribute()]
        public static get_attribute_typeDel get_attribute_type;
        [NativeMethodAttribute()]
        public static get_typedNodeDel get_mesh;
        [NativeMethodAttribute()]
        public static get_typedNodeDel get_skeleton;
        [NativeMethodAttribute()]
        public static get_vertexcountDel get_vertexcount;
        [NativeMethodAttribute()]
        public static getVerticesDel getVertices;
        [NativeMethodAttribute()]
        public static set_transformDel set_transform;
        [NativeMethodAttribute()]
        public static get_countDel get_anim_stack_count;
        [NativeMethodAttribute()]
        public static get_ElementDel get_anim_stack;
        [NativeMethodAttribute()]
        public static get_countDel get_anim_layer_count;
        [NativeMethodAttribute()]
        public static get_ElementDel get_anim_layer;


        [NativeMethodAttribute()]
        public static get_anim_curveDel get_anim_curve;
        [NativeMethodAttribute()]
        public static get_anim_curve_keyCountDel get_anim_curve_keyCount;

        public static string getLog()
        {
            return Marshal.PtrToStringAnsi(getLogInternal());
        }
        public static string get_name(IntPtr obj)
        {
            return Marshal.PtrToStringAnsi(get_name_internal(obj));
        }
    }
}

