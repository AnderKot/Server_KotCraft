using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{


    /*
    List<MyMeshInfo> Pointers = StartMeshPointers.FindAll(pointer => (pointer.BlockPoint == point));
    int DeletedIndex = StartMeshPointers.IndexOf(Pointers[0]);
    //int CountDeletedID = 0;
    int DST = 0;
    int DSV = 0;
    bool Transparency = Block.IsTransparency(GetLocalBlockID(point));
    foreach (MyMeshInfo Pointer in Pointers)
    {
        if (!Transparency)
            NearBlocksSides.Remove(Pointer.SideID);

        Verticles.RemoveRange(Pointer.StartVerticlIndex - DSV, 4);
        MyUV.RemoveRange(Pointer.StartVerticlIndex - DSV, 4);
        MyUVasID.RemoveRange(Pointer.StartVerticlIndex - DSV, 4);
        Triangles.RemoveRange(Pointer.StartTrianglesIndex - DST, 6);
        List<int> TempTriangles = Triangles.GetRange(Pointer.StartTrianglesIndex - DST, Triangles.Count - (Pointer.StartTrianglesIndex - DST));
        Triangles.RemoveRange(Pointer.StartTrianglesIndex - DST, Triangles.Count - (Pointer.StartTrianglesIndex - DST));
        foreach (int triangles in TempTriangles)
        {
            Triangles.Add(triangles - 4);
        }

        DeletedIndex = StartMeshPointers.FindIndex(pointer => (pointer.BlockPoint == Pointer.BlockPoint) & (pointer.SideID == Pointer.SideID));
        StartMeshPointers.RemoveAt(DeletedIndex);
        List<MyMeshInfo> TempPointers = StartMeshPointers.GetRange(DeletedIndex, StartMeshPointers.Count - DeletedIndex);
        StartMeshPointers.RemoveRange(DeletedIndex, StartMeshPointers.Count - DeletedIndex);
        foreach (MyMeshInfo pointers in TempPointers)
        {
            StartMeshPointers.Add(new MyMeshInfo(pointers.BlockPoint, pointers.StartVerticlIndex - DSV, pointers.StartTrianglesIndex - DST, pointers.SideID));
        }

        //CountDeletedID++;
        DST += 6;
        DSV += 4;
    }
    */
    /*
    StartMeshPointers.RemoveRange(DeletedIndex, CountDeletedID);
    List<MyMeshInfo> TempPointers = StartMeshPointers.GetRange(DeletedIndex, StartMeshPointers.Count - DeletedIndex);
    StartMeshPointers.RemoveRange(DeletedIndex, StartMeshPointers.Count - DeletedIndex);
    foreach (MyMeshInfo pointers in TempPointers)
    {
        StartMeshPointers.Add(new MyMeshInfo(pointers.BlockPoint, pointers.StartVerticlIndex - (4 * CountDeletedID), pointers.StartTrianglesIndex - (6 * CountDeletedID), pointers.SideID));
    }
    */




    /*
RenderList.Add(CurrChank + (Vector3Int.forward * 20));
RenderList.Add(CurrChank + (Vector3Int.forward * 20) + (Vector3Int.right * 20));
RenderList.Add(CurrChank + (Vector3Int.forward * 20) + (Vector3Int.left * 20));

RenderList.Add(CurrChank + (Vector3Int.back * 20));
RenderList.Add(CurrChank + (Vector3Int.back * 20) + (Vector3Int.right * 20));
RenderList.Add(CurrChank + (Vector3Int.back * 20) + (Vector3Int.left * 20));

RenderList.Add(CurrChank + (Vector3Int.right * 20));
RenderList.Add(CurrChank + (Vector3Int.left * 20));
*/
}
