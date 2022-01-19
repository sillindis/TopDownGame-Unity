using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferMap : MonoBehaviour
{
    //public string transferMapName; //�̵��� ���� �̸�
    public Transform target;

    private PlayerAction player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerAction>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            player.transform.position = target.transform.position;
            //player.currentMapName = transferMapName; ->transfer Scene
            //SceneManager.LoadScene(transferMapName); ->transfer Scene
        }
    }
}
