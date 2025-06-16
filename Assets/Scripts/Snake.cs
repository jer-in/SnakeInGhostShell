using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    public Transform segmentPrefab;
    public Vector2Int direction = Vector2Int.right;
    public float speed = 20f;
    public float speedMultiplier = 1f;
    public int initialSize = 4;
    public bool moveThroughWalls = false;

    private readonly List<Transform> segments = new List<Transform>();
    private Vector2Int input;
    private float nextUpdate;

    public bool isPhasing = false;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        if (direction.x != 0f)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                input = Vector2Int.up;
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                input = Vector2Int.down;
            }
        }
        else if (direction.y != 0f)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                input = Vector2Int.right;
            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                input = Vector2Int.left;
            }
        }
    }

    private void FixedUpdate()
    {
        if (Time.time < nextUpdate) {
            return;
        }

        if (input != Vector2Int.zero) {
            direction = input;
        }

        for (int i = segments.Count - 1; i > 0; i--) {
            segments[i].position = segments[i - 1].position;
        }

        int x = Mathf.RoundToInt(transform.position.x) + direction.x;
        int y = Mathf.RoundToInt(transform.position.y) + direction.y;
        transform.position = new Vector2(x, y);

        nextUpdate = Time.time + (1f / (speed * speedMultiplier));

        if (direction == Vector2Int.up)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (direction == Vector2Int.down)
            transform.rotation = Quaternion.Euler(0, 0, 180);
        else if (direction == Vector2Int.left)
            transform.rotation = Quaternion.Euler(0, 0, 90);
        else if (direction == Vector2Int.right)
            transform.rotation = Quaternion.Euler(0, 0, -90);

        // Self-collision detection
        if (!isPhasing)
        {
            for (int i = 1; i < segments.Count; i++)
            {
                if (transform.position == segments[i].position)
                {
                    ResetState();
                    GameManager.Instance.ResetScore();
                    break;
                }
            }
        }
    }

    public void Grow()
    {
        Transform segment = Instantiate(segmentPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }

    public void ResetState()
    {
        direction = Vector2Int.right;
        transform.position = Vector3.zero;

        for (int i = 1; i < segments.Count; i++) {
            Destroy(segments[i].gameObject);
        }

        segments.Clear();
        segments.Add(transform);

        for (int i = 0; i < initialSize - 1; i++) {
            Grow();
        }

        isPhasing = false;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f); // Reset opacity
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("SnakeBody")) return;

        if (other.CompareTag("Food"))
        {
            Grow();
            //Destroy(other.gameObject);
            GameManager.Instance.AddFood();
            other.GetComponent<Food>().RandomizePosition(); // Move the food to new random location
        }
        else if (other.CompareTag("SpecialFood"))
        {
            Destroy(other.gameObject);
            StartCoroutine(EnablePhaseMode());
        }
        else if (other.CompareTag("Obstacle"))
        {
            ResetState();
            GameManager.Instance.ResetScore();
        }
        else if (other.CompareTag("Wall"))
        {
            if (moveThroughWalls)
            {
                Traverse(other.transform);
            }
            else
            {
                ResetState();
                GameManager.Instance.ResetScore();
            }
        }
    }

    private IEnumerator EnablePhaseMode()
    {
        isPhasing = true;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.5f); // Ghost transparency

        yield return new WaitForSeconds(7f);

        isPhasing = false;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f); // Restore
    }

    private void Traverse(Transform wall)
    {
        Vector3 position = transform.position;

        if (direction.x != 0f) {
            position.x = Mathf.RoundToInt(-wall.position.x + direction.x);
        } else if (direction.y != 0f) {
            position.y = Mathf.RoundToInt(-wall.position.y + direction.y);
        }

        transform.position = position;
    }

    public bool Occupies(int x, int y)
    {
        foreach (Transform segment in segments)
        {
            if (Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.y) == y)
            {
                return true;
            }
        }
        return false;
    }
}
