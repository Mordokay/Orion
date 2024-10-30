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
    public GameObject rotatingObject;   // The object that rotates around the player

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
        RotateAroundPlayer();

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

    void RotateAroundPlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            // Calculate direction from the player to the mouse hit point
            Vector3 direction = hit.point - transform.position;
            direction.y = 0; // Keep rotation on the XZ plane

            // Calculate the rotation angle from the forward direction
            float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

            // Rotate the object around the player
            Vector3 position = transform.position + Quaternion.Euler(0, -angle, 0) * Vector3.right * 0.8f;

            rotatingObject.transform.position = new Vector3(position.x, transform.position.y - 0.6f, position.z);
            rotatingObject.transform.rotation = Quaternion.LookRotation(-direction) * Quaternion.Euler(0, 180, 0);
        }
    }
}