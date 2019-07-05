using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    //board symmetry axis
    private const float BoardCenter = 3.5f;

    public void MoveOtherSide()
    {
        var pos = transform.position;
        pos.z = - pos.z + 2 * BoardCenter;
        transform.position = pos;
        transform.Rotate(0, 180, 0, Space.World);
    }

    public void ToggleFrontView()
    {

    }
}
