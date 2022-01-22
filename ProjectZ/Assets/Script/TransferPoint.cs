using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferPoint : MonoBehaviour //->transfer Scene
{
    public string enterPoint;

    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();

        if (enterPoint== player.currentMapName)
        {
            player.transform.position=this.transform.position;
        }
    }
}
