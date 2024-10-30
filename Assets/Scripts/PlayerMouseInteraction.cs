using System.Collections.Generic;
using UnityEngine;

public class PlayerMouseInteraction : MonoBehaviour
{
    public GameObject cubePrefab;     // Assign a cube prefab in the inspector
    public GameObject projectilePrefab;     // Assign a cube prefab in the inspector
    public LayerMask groundLayer;     // Assign the Ground layer in the inspector
    private GameObject firstCube;     // First cube at initial click position
    private GameObject secondCube;    // Second cube that follows the mouse
    private List<GameObject> projectiles = new();    // projectiles
    private bool isDragging = false;

    [SerializeField] Material mouseDownMaterial;
    [SerializeField] Material mouseDragMaterial;
    [SerializeField] bool keepTrajectoryAfterProjectile;

    void Start()
    {
        gameObject.GetComponent<LineRenderer>().SetPositions(new List<Vector3>().ToArray());
        gameObject.GetComponent<LineRenderer>().positionCount = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCubePlacement();
        }

        if (isDragging && secondCube != null)
        {
            MoveSecondCube();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            GameObject projectile = Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity);
            Vector3[] positions = gameObject.GetComponent<QuadraticBezier>().allPositions.ToArray();
            projectile.GetComponent<ProjectileMovement>().positions = positions;

            projectiles.Add(projectile);

            if (!keepTrajectoryAfterProjectile)
            {
                gameObject.GetComponent<QuadraticBezier>().RemoveTrajectory();
                gameObject.GetComponent<QuadraticBezier>().Checkpoints = new();
            }
            RemoveObjects();
        }
    }

    void StartCubePlacement()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Perform the raycast using the ground layer mask
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            // Instantiate the first cube at the hit point
            firstCube = Instantiate(cubePrefab, new Vector3(hit.point.x, gameObject.transform.position.y, hit.point.z), Quaternion.identity);
            firstCube.GetComponent<Renderer>().material = mouseDownMaterial;

            // Instantiate the second cube at the same position
            secondCube = Instantiate(cubePrefab, new Vector3(hit.point.x, gameObject.transform.position.y, hit.point.z), Quaternion.identity);
            secondCube.GetComponent<Renderer>().material = mouseDragMaterial;

            // Set dragging to true
            isDragging = true;

            gameObject.GetComponent<QuadraticBezier>().Checkpoints = new List<GameObject> { gameObject, secondCube, firstCube };
        }
    }

    void MoveSecondCube()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Perform the raycast using the ground layer mask
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            // Move the second cube to follow the mouse position on the ground
            secondCube.transform.position = new Vector3(hit.point.x, gameObject.transform.position.y, hit.point.z);
        }
    }

    void RemoveObjects()
    {
        // Destroy both cubes and reset flags
        if (firstCube != null)
            Destroy(firstCube);

        if (secondCube != null)
            Destroy(secondCube);

        isDragging = false;
    }
}