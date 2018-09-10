using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Linq;

public class BlenderAssetProcessor : AssetPostprocessor
{
	//public void OnPostprocessModel( GameObject obj )
	//{
	//	//only perform corrections with blender files
	//	ModelImporter importer = assetImporter as ModelImporter;
	//	if( Path.GetExtension( importer.assetPath ) == ".blend" )
	//	{
	//		RotateObject( obj.transform );
	//	}
	//}

	////recursively rotate a object tree individualy
	//private void RotateObject( Transform obj )
	//{
	//	//if a meshFilter is attached, we rotate the vertex mesh data
	//	MeshFilter meshFilter = obj.GetComponent( typeof( MeshFilter ) ) as MeshFilter;
	//	if( meshFilter )
	//	{
	//		RotateMesh( meshFilter.sharedMesh );
	//	}

	//	obj.transform.rotation = Quaternion.identity;

	//	//do this too for all our children
	//	//Casting is done to get rid of implicit downcast errors
	//	foreach( Transform child in obj )
	//	{
	//		AdjustOffset( child );
	//		RotateObject( child );
	//	}
	//}

	////"rotate" the mesh data
	//private void RotateMesh( Mesh mesh )
	//{
	//	int index = 0;

	//	//switch all vertex z values with y values
	//	Vector3[] vertices = mesh.vertices;
	//	for( index = 0 ; index < vertices.Length ; index++ )
	//	{
	//		vertices[ index ] = new Vector3( vertices[ index ].x, vertices[ index ].z, vertices[ index ].y );
	//	}
	//	mesh.vertices = vertices;

	//	//for each submesh, we invert the order of vertices for all triangles
	//	//for some reason changing the vertex positions flips all the normals???
	//	for( int submesh = 0 ; submesh < mesh.subMeshCount ; submesh++ )
	//	{
	//		int[] triangles = mesh.GetTriangles( submesh );
	//		for( index = 0 ; index < triangles.Length ; index += 3 )
	//		{
	//			int intermediate = triangles[ index ];
	//			triangles[ index ] = triangles[ index + 2 ];
	//			triangles[ index + 2 ] = intermediate;
	//		}
	//		mesh.SetTriangles( triangles, submesh );
	//	}

	//	//recalculate other relevant mesh data
	//	mesh.RecalculateNormals();
	//	mesh.RecalculateBounds();
	//}

	//private void AdjustOffset( Transform obj )
	//{
	//	// adjust the local position to take into account the switching of the mesh axis
	//	obj.localPosition = new Vector3( obj.localPosition.x, obj.localPosition.z, obj.localPosition.y );
	//}
}