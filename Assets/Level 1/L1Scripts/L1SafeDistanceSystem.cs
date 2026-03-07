using UnityEngine;

public class L1SafeDistanceSystem : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public L1PlayerMovement movement;
    public Animator playerAnimator;
    public L1EnemyChase enemy;

    [Header("Distance")]
    public float safeDistance=500f;
    public float distanceMultiplier=0.1f;

    [Header("Spawn")]
    public GameObject robotPrefab;
    public GameObject portalPrefab;

    public Vector3 robotOffset=new Vector3(15f,0f,-39.5f);
    public Vector3 portalOffset=new Vector3(30f,0f,-39.5f);

    private bool completed;

    void Update()
    {
        if(completed)
            return;

        float distance=(player.position.x+300f)*distanceMultiplier;

        if(distance>=safeDistance)
        {
            CompleteLevel();
        }
    }

    void CompleteLevel()
    {
        completed=true;

        if(movement!=null)
            movement.stopMoving=true;

        if(playerAnimator!=null)
            playerAnimator.SetFloat("Speed",0);

        if(enemy!=null)
            enemy.stopChasing=true;

        SpawnRobot();
        SpawnPortal();

        Debug.Log("LEVEL COMPLETE");
    }

    void SpawnRobot()
    {
        if(robotPrefab==null)
            return;

        Vector3 pos=player.position+robotOffset;

        Instantiate(robotPrefab,pos,Quaternion.identity);
    }

    void SpawnPortal()
    {
        if(portalPrefab==null)
            return;

        Vector3 pos=player.position+portalOffset;

        Instantiate(portalPrefab,pos,Quaternion.identity);
    }
}